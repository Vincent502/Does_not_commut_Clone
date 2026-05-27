using UnityEditor;
using UnityEngine;

/// <summary>
/// Converts Toon City Pack materials from Built-in (Standard) shaders to URP.
/// Fixes magenta/pink materials when using Universal Render Pipeline.
/// </summary>
public static class ToonCityMaterialUpgrader
{
    private const string PackMaterialsFolder = "Assets/Loading Games/Toon City Pack";

    [MenuItem("Tools/Fix Toon City Materials (URP)")]
    public static void ConvertAllMaterials()
    {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");

        if (urpLit == null || urpUnlit == null)
        {
            EditorUtility.DisplayDialog(
                "URP shaders not found",
                "Make sure the Universal RP package is installed and the project uses URP.",
                "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { PackMaterialsFolder });
        int converted = 0;
        int skipped = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            string shaderName = mat.shader != null ? mat.shader.name : "";

            if (shaderName.StartsWith("Universal Render Pipeline/"))
            {
                skipped++;
                continue;
            }

            ConvertMaterial(mat, urpLit, urpUnlit);
            EditorUtility.SetDirty(mat);
            converted++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[ToonCityMaterialUpgrader] Converted {converted} material(s), skipped {skipped} already on URP.");
        EditorUtility.DisplayDialog(
            "Materials converted",
            $"Converted {converted} material(s) to URP.\nSkipped {skipped} already compatible.",
            "OK");
    }

    private static void ConvertMaterial(Material mat, Shader urpLit, Shader urpUnlit)
    {
        Texture mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
        Color color = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
        float smoothness = mat.HasProperty("_Glossiness") ? mat.GetFloat("_Glossiness") : 0.5f;
        float metallic = mat.HasProperty("_Metallic") ? mat.GetFloat("_Metallic") : 0f;
        Color emission = mat.HasProperty("_EmissionColor") ? mat.GetColor("_EmissionColor") : Color.black;

        string shaderName = mat.shader.name;
        bool useUnlit = shaderName.Contains("Unlit")
                     || shaderName.Contains("Legacy Shaders")
                     || shaderName.Contains("Mobile/Unlit");

        mat.shader = useUnlit ? urpUnlit : urpLit;

        if (mat.HasProperty("_BaseMap") && mainTex != null)
            mat.SetTexture("_BaseMap", mainTex);

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);

        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", smoothness);

        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", metallic);

        if (mat.HasProperty("_EmissionColor") && emission.maxColorComponent > 0f)
        {
            mat.SetColor("_EmissionColor", emission);
            mat.EnableKeyword("_EMISSION");
        }
    }
}
