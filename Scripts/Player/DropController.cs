using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    private Vector2 target;

    void Start()
    {
        Vector2 pos = transform.position;
        target = pos + Random.insideUnitCircle / 2;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 5);
        }
    }

    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    private float particleEmissionMin = 10.0f;
    private float particleEmissionMax = 150.0f;

    [SerializeField]
    private ParticleSystem fire;
    private ParticleSystem.EmissionModule fireEmission;

    [SerializeField]
    private ParticleSystem smoke;
    private ParticleSystem.EmissionModule smokeEmission;

    [SerializeField]
    private ParticleSystem rain;
    private ParticleSystem.EmissionModule rainEmission;


    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
        fireEmission = fire.emission;
        smokeEmission = smoke.emission;
        rainEmission = rain.emission;
    }

    private void Update()
    {
        time += timeRate * Time.deltaTime;
        if (time >= 1.0f)
            time = 0.0f;

        sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;

        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = sunIntensity.Evaluate(time);

        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(true);

        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(true);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);

        if (time > 0.0f && time < 0.5f)
        {
            fireEmission.rateOverTime = Mathf.Lerp(particleEmissionMin, particleEmissionMax, time / 0.5f);
            smokeEmission.rateOverTime = Mathf.Lerp(particleEmissionMin, particleEmissionMax, time / 0.5f);
        }
        else
        {
            fireEmission.rateOverTime = Mathf.Lerp(particleEmissionMax, particleEmissionMin, time / 0.75f);
            smokeEmission.rateOverTime = Mathf.Lerp(particleEmissionMax, particleEmissionMin, time / 0.75f);
        }

        if (time > 0.75f && time < 0.875f)
        {
            rainEmission.rateOverTime = Mathf.Lerp(particleEmissionMin, particleEmissionMax, time / 0.875f);
        }
        if (time > 0.875f && time < 1f)
        {
            rainEmission.rateOverTime = Mathf.Lerp(particleEmissionMax, particleEmissionMin, time / 1f);
        }

        if (time > 0.0f && time < 0.75f)
        {
            if (!fire.isPlaying)
                fire.Play();
            if (!smoke.isPlaying)
                smoke.Play();
            if (rain.isPlaying)
                rain.Stop();
        }
        else
        {
            if (!rain.isPlaying)
                rain.Play();
            if (fire.isPlaying)
                fire.Stop();
            if (smoke.isPlaying)
                smoke.Stop();
        }
    }
}
