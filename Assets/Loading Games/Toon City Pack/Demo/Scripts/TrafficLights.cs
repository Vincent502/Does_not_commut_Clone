using UnityEngine;

public enum LightColor { Red, Yellow, Green, None }

/// <summary>
/// Controls traffic light bulb materials (green / yellow / red).
/// Updated for URP — the original script used Built-in "Standard" and "Unlit/Color" shaders.
/// </summary>
public class TrafficLights : MonoBehaviour
{
    public LightColor activeLight;

    private MeshRenderer mr;
    private Shader[] defaultShaders;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        defaultShaders = new Shader[mr.materials.Length];

        // mat 1 : green, mat 2 : yellow, mat 3 : red
        for (int i = 1; i < 4; i++)
            defaultShaders[i] = mr.materials[i].shader;

        SetLight(activeLight);
    }

    public void SetLight(LightColor color)
    {
        int activeIndex = 0;
        switch (color)
        {
            case LightColor.Green:  activeIndex = 1; break;
            case LightColor.Yellow: activeIndex = 2; break;
            case LightColor.Red:    activeIndex = 3; break;
        }

        for (int i = 1; i < 4; i++)
        {
            mr.materials[i].shader = activeIndex == i
                ? ToonCityUrpMaterials.Unlit
                : defaultShaders[i];
        }
    }
}
