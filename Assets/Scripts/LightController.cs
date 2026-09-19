using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    public Light2D light;
    public float minIntensity = 0.85F;
    public float maxIntensity = 0.65F;
    public float flickerIntensity = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        float intensity =
            (Mathf.PerlinNoise1D(Time.time * flickerIntensity)) * (maxIntensity - minIntensity)
            + minIntensity;
        light.intensity = intensity;
    }
}
