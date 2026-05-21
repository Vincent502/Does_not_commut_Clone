using UnityEngine;

/// <summary>
/// Toggles street lamp mesh material and point lights.
/// Updated for URP — the original script used Built-in "Standard" and "Unlit/Color" shaders.
/// </summary>
public class StreetLight : MonoBehaviour
{
    public Light[] lights;
    public bool isOn;

    private Shader defaultShader;
    private Color defaultColor;
    private MeshRenderer mr;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        defaultShader = mr.materials[1].shader;
        defaultColor  = ToonCityUrpMaterials.GetColor(mr.materials[1]);
        SetLight(isOn);
    }

    public void SetLight(bool isOn)
    {
        this.isOn = isOn;
        Material lampMat = mr.materials[1];

        if (isOn)
        {
            if (ToonCityUrpMaterials.Unlit != null)
                lampMat.shader = ToonCityUrpMaterials.Unlit;
            ToonCityUrpMaterials.SetColor(lampMat, lights[0].color);
        }
        else
        {
            lampMat.shader = defaultShader;
            ToonCityUrpMaterials.SetColor(lampMat, defaultColor);
        }

        foreach (Light l in lights)
            l.gameObject.SetActive(isOn);
    }
}
