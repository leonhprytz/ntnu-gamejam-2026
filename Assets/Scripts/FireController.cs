using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum FireState
{
    Small,
    Medium,
    Large,
}

public class FireController : MonoBehaviour
{
    private Light2D light;

    public FireState state = FireState.Small;
    public float[] radii = new float[] { 1.85F, 3.2F, 5.0F };
    public float flickerVariation = 0.2F;
    public float flickerSpeed = 2;

    private void updateLightRadius()
    {
        float flickerRange = flickerVariation * radii[(int)state];
        float variation =
            Mathf.PerlinNoise1D(flickerSpeed * Time.time) * flickerRange - flickerRange / 2;

        light.pointLightOuterRadius = radii[(int)state] + variation;
    }

    public void increaseState()
    {
        switch (this.state)
        {
            case (FireState.Small):
                state = FireState.Medium;
                break;
            case (FireState.Medium):
                state = FireState.Large;
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        updateLightRadius();
    }
}
