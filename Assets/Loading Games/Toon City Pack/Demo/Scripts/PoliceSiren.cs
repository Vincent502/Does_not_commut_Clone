using UnityEngine;

/// <summary>
/// Alternates police car siren lights (blue / red materials).
/// Updated for URP — the original script used Built-in "Standard" and "Unlit/Color" shaders.
/// </summary>
public class PoliceSiren : MonoBehaviour
{
    public GameObject blueLight;
    public GameObject redLight;
    public bool isSirenOn;
    public float colorInterval;

    private float timer;
    private MeshRenderer mr;
    private Shader defaultShaderBlue;
    private Shader defaultShaderRed;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        // index 3 : blue, index 4 : red
        defaultShaderBlue = mr.materials[3].shader;
        defaultShaderRed  = mr.materials[4].shader;
    }

    private void Update()
    {
        if (!isSirenOn) return;

        if (timer > colorInterval)
        {
            bool isBlueUnlit = ToonCityUrpMaterials.IsUnlit(mr.materials[3]);

            blueLight.SetActive(!isBlueUnlit);
            redLight.SetActive(isBlueUnlit);

            mr.materials[3].shader = isBlueUnlit ? defaultShaderBlue : ToonCityUrpMaterials.Unlit;
            mr.materials[4].shader = isBlueUnlit ? ToonCityUrpMaterials.Unlit : defaultShaderRed;

            timer = 0;
        }

        timer += Time.deltaTime;
    }
}
