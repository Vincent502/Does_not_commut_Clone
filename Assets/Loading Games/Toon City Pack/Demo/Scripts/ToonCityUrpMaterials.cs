using UnityEngine;

/// <summary>
/// URP shader helpers for Toon City Pack demo scripts.
/// Replaces Built-in "Standard" / "Unlit/Color" lookups that cause magenta materials in URP projects.
/// </summary>
public static class ToonCityUrpMaterials
{
    private static Shader urpLit;
    private static Shader urpUnlit;

    public static Shader Lit
    {
        get
        {
            if (urpLit == null)
                urpLit = Shader.Find("Universal Render Pipeline/Lit");
            return urpLit;
        }
    }

    public static Shader Unlit
    {
        get
        {
            if (urpUnlit == null)
                urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
            return urpUnlit;
        }
    }

    public static Color GetColor(Material mat)
    {
        if (mat.HasProperty("_BaseColor"))
            return mat.GetColor("_BaseColor");
        return mat.color;
    }

    public static void SetColor(Material mat, Color color)
    {
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else
            mat.color = color;
    }

    public static bool IsUnlit(Material mat)
    {
        return mat.shader != null && mat.shader.name.Contains("Unlit");
    }
}
