using System.Collections;
using System.Linq;

using UnityEngine;

/// <summary>
/// A decoration light that flickers when a ghost is nearby.
/// Authors: Scott Clarke and Daniel Darnell.
/// </summary>
public class FlickerLight : MonoBehaviour
{
    /// <summary>
    /// The minimum probability of a flicker occurring on any frame.
    /// </summary>
    public float minFlickerChance;

    /// <summary>
    /// The maximum probability of a flicker occurring on any frame.
    /// </summary>
    public float maxFlickerChance;

    /// <summary>
    /// The minimum duration of a flicker (in seconds).
    /// </summary>
    public float minFlickerTime;

    /// <summary>
    /// The maximum duration of a flicker (in seconds).
    /// </summary>
    public float maxFlickerTime;

    /// <summary>
    /// The minimum reduction factor for the light's intensity during a flicker.
    /// </summary>
    public float minIntensityReduction;

    /// <summary>
    /// The maximum reduction factor for the light's intensity during a flicker.
    /// </summary>
    public float maxIntensityReduction;

    /// <summary>
    /// The range within with ghosts will increase the flicker chance of the light.
    /// </summary>
    public float flickerRange;

    /// <summary>
    /// The <see cref="Light"/> game object to which this script is attached.
    /// </summary>
    new private Light light;

    /// <summary>
    /// The original intensity of the light when it is not flickering.
    /// </summary>
    private float fullIntensity;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        light = GetComponent<Light>();
        fullIntensity = light.intensity;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (GameManager.IsPaused)
        {
            return;
        }
        
        if(light.enabled == true)
        {
            UpdateFlicker();
        }
    }

    /// <summary>
    /// Updates this light's intensity to make it flicker based on proximity to any ghosts.
    /// </summary>
    private void UpdateFlicker()
    {
        // Distance to nearest ghost
        float distance = FindObjectsOfType<Ghost>()
            .Min(x => Vector3.Distance(x.transform.position, transform.position));

        // If any ghost is within set range, flicker chance linearly decreases
        //  from max to min depending on its proximity
        float currentFlickerChance = (distance <= flickerRange)
            ? maxFlickerChance - distance * (maxFlickerChance - minFlickerChance) / flickerRange
            : minFlickerChance;

        if (Random.value <= currentFlickerChance)
        {
            // Flicker with random duration and intensity reduction
            StartCoroutine(Flicker(
                duration: Random.Range(minFlickerTime, maxFlickerTime),
                intensityReduction: Random.Range(minIntensityReduction, maxIntensityReduction)));
        }
    }

    /// <summary>
    /// Flickers the light for the specified duration (in seconds)
    /// by dividing the intensity by the specified reduction factor.
    /// </summary>
    /// <param name="duration">The duration (in seconds) to flicker the light.</param>
    /// <param name="intensityReduction">The factor by which to divide the light's intensity.</param>
    /// <returns>Yield returns an instruction to wait for the specified duration.</returns>
    private IEnumerator Flicker(float duration, float intensityReduction)
    {
        light.intensity /= intensityReduction;
        yield return new WaitForSeconds(duration);
        light.intensity = fullIntensity;
    }
}
