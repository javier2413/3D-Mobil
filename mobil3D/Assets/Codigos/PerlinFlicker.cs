using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinFlicker : MonoBehaviour
{
    public Light lightSource;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 1.0f;

    private float timeOffset;

    void Start()
    {
        if (lightSource == null) lightSource = GetComponent<Light>();
        timeOffset = Random.Range(0, 100);
    }

    void Update()
    {
        float noiseValue = Mathf.PerlinNoise(Time.time * flickerSpeed, timeOffset);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noiseValue);
        lightSource.intensity = intensity;
    }
}
