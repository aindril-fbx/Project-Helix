using UnityEngine;

public class fogControl : MonoBehaviour
{

    [SerializeField] private float maxDensityValue;
    [SerializeField] private float minDensityValue;
    [SerializeField] private float timeInterval = 120f;
    float tempTimeInterval;
    float t = 0f;
    [SerializeField] private float density = 0f;
    float currentDensity = 0f;

    [SerializeField] ParticleSystem rainParticleSystem;

    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Color minTintColor = Color.white;
    [SerializeField] private Color maxTintColor = Color.black;
    [SerializeField] private Light directionalLight;
    [SerializeField] private AnimationCurve tintCurve;

    [SerializeField] private Material[] roadMaterial;

    int shouldChange = 10;
    private void Start()
    {
        foreach (Material m in roadMaterial)
        {
            m.SetFloat("_rainFactor", 0f);
        }
        int shouldRain = Random.Range(0, 10);
        shouldChange = Random.Range(0, 10);
        density = minDensityValue;
        if (shouldRain < 1)
        {
            density = Random.Range(minDensityValue, maxDensityValue);
            Debug.Log("Starting Rain");
            Debug.Log(shouldChange + " , " + shouldRain);
        }
        currentDensity = density;
        RenderSettings.fog = true;
        RenderSettings.fogDensity = density;
        tempTimeInterval = timeInterval;
        skyboxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyboxMaterial;
        directionalLight = GameObject.FindWithTag("Sun").GetComponent<Light>();
        var emission = rainParticleSystem.emission;
        emission.rateOverTime = 0f;
        emission.rateOverDistance = 0f;

    }

    float bias;
    [SerializeField] private float maxExposure = .8f;
    private void Update()
    {
        density = Mathf.Clamp(density, minDensityValue, maxDensityValue);
        bias = currentDensity / maxDensityValue;
        skyboxMaterial.SetFloat("_FogEnd", bias);
        skyboxMaterial.SetFloat("_Exposure", Mathf.Clamp(bias * maxExposure, 0.39f, maxExposure));
        skyboxMaterial.SetColor("_Tint", Color.Lerp(minTintColor, maxTintColor, tintCurve.Evaluate(bias)));
        skyboxMaterial.SetFloat("_FogIntens", bias);
        directionalLight.intensity = Mathf.Lerp(2.4f, .49f, tintCurve.Evaluate(bias));
        var emission = rainParticleSystem.emission;
        if (RenderSettings.fogDensity > (minDensityValue + maxDensityValue) / 2)
        {
            emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, 80000f * density, Time.deltaTime * 0.1f);
            foreach (Material m in roadMaterial)
            {
                m.SetFloat("_rainFactor", Mathf.Clamp(m.GetFloat("_rainFactor") + 0.0005f, 0f, 1f));
            }
        }
        else
        {
            emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, 0f, Time.deltaTime * 0.5f);
            foreach (Material m in roadMaterial)
            {
                m.SetFloat("_rainFactor", Mathf.Clamp(m.GetFloat("_rainFactor") - 0.0001f, 0f, 1f));
            }
        }
        emission.rateOverDistance = emission.rateOverTime.constant / 70f;
        if (Mathf.Abs(RenderSettings.fogDensity - density) > 0.0001f)
        {
            currentDensity = Mathf.Lerp(currentDensity, density, Time.deltaTime * 0.02f);
            RenderSettings.fogDensity = Mathf.Floor(currentDensity * 10000f) / 10000f;
        }
        tempTimeInterval = currentDensity > maxDensityValue / 2 ? timeInterval / 2 : timeInterval;
        t += Time.deltaTime;
        if (t > tempTimeInterval && shouldChange < 3)
        {
            shouldChange = Random.Range(0, 10);
            density = Random.Range(minDensityValue, maxDensityValue);
            t = 0f;

        }

        //Debug.Log(density + " , " + RenderSettings.fogDensity);
    }
}