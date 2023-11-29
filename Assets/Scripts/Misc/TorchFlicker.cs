using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class TorchFlicker : MonoBehaviour
{
    [SerializeField] Light2D torchLight;
    [Tooltip("The base intensity of the torch light")]
    [SerializeField] private float baseIntensity = 1.0f;
    [Tooltip("The minimum intensity during flicker")]
    [SerializeField] private float minFlickerIntensity = 0.8f;
    [Tooltip("The maximum intensity during flicker")]
    [SerializeField] private float maxFlickerIntensity = 1.2f;
    [Tooltip("The speed at which the torch light flickers")]
    [SerializeField] private float flickerSpeed = 1.0f; 

    private float currentIntensity;

    private void Start()
    {
        if (torchLight == null)
        {
            torchLight = GetComponent<Light2D>();
        }

        currentIntensity = baseIntensity;
        torchLight.intensity = currentIntensity;

        // Start the flickering coroutine
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            // Calculate a flicker intensity between minFlickerIntensity and maxFlickerIntensity
            float flickerIntensity = Random.Range(minFlickerIntensity, maxFlickerIntensity);

            // Smoothly interpolate the current intensity towards the flicker intensity
            while (Mathf.Abs(currentIntensity - flickerIntensity) > 0.05f)
            {
                currentIntensity = Mathf.Lerp(currentIntensity, flickerIntensity, flickerSpeed * Time.deltaTime);
                torchLight.intensity = currentIntensity;
                yield return null;
            }

            // Wait for a short random time before changing intensity again
            float flickerDelay = Random.Range(0.1f, 0.3f);
            yield return new WaitForSeconds(flickerDelay);
        }
    }
}