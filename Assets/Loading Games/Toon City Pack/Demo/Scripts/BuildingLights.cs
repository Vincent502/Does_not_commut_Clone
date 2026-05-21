using UnityEngine;

/// <summary>
/// Toggles building window lights on/off.
/// Updated for URP — the original script used Built-in "Standard" shaders which appear magenta in URP.
/// </summary>
public class BuildingLights : MonoBehaviour
{
    public int windowMaterialIndex;
    public Color lightColor;
    public bool areLightsOn;

    private Color defaultColor;
    private Shader defaultShader;
    private MeshRenderer mr;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        Material windowMat = mr.materials[windowMaterialIndex];

        defaultShader = windowMat.shader;
        defaultColor = ToonCityUrpMaterials.GetColor(windowMat);
        SetLights(areLightsOn);
    }

    public void SetLights(bool isOn)
    {
        Material windowMat = mr.materials[windowMaterialIndex];

        if (isOn)
        {
            if (ToonCityUrpMaterials.Unlit != null)
                windowMat.shader = ToonCityUrpMaterials.Unlit;
            ToonCityUrpMaterials.SetColor(windowMat, lightColor);
        }
        else
        {
            windowMat.shader = defaultShader;
            ToonCityUrpMaterials.SetColor(windowMat, defaultColor);
        }
    }
}
