using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
public class CarController : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool isMph = true;

    public string GetMetricUnit()
    {
        return isMph ? "MPH" : "KMH";
    }
    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [Range(0.01f, 5f)]
    float wmass;
    [SerializeField] private float[] GearRatio = new float[4];
    public bool AWD = true;
    [SerializeField] private float angularDragCoeff = 1f;
    [SerializeField] private bool Debug_ = true;
    [SerializeField] private bool downforce;
    public Rigidbody rb;
    public InputManager IM;
    [Header("Bias Values")]
    [Range(1f, 5f)]
    [SerializeField] private float boostBias = 1f;

    [Space]
    [Header("Motor Values")]
    [SerializeField] private AnimationCurve enginePower;
    public AnimationCurve boostCurve;
    public float motorForcelol = 1000f;
    [SerializeField] private float HandBrakeboost = 1f;
    [SerializeField] private float maxWheelRPM = 15000f;
    [SerializeField] private float breakForcelol = 2000000f;
    [SerializeField] private float downForce_f = 100f;
    public float stearingAnglelol = 20f;
    public float CurrentSteerAngle;
    float driftBias = 0f;
    [SerializeField] private float Dampning = 0.1f;
    [SerializeField] private DriftAngle DA;
    public float currentsteervalue = 0f;
    public float maxSpeed = 200f;
    public float minSpeed = -40f;
    const float SpeedMeasurePeriod = 0.1f;
    private int m_SpeedAccumulator = 0;
    private float m_SpeedNextPeriod = 0;
    //[SerializeField] private GameObject[] UI_canvas;
    [SerializeField] public int NoOfGears = 8;
    public float BoostFactor = 0f;
    [SerializeField] private float m_RevRangeBoundary = 1f;
    public int m_GearNum;
    private float m_GearFactor;
    public float Revs;
    [SerializeField] private Image revdisplay;
    [SerializeField] private Gradient revGradient;

    public Gradient getRevGradient()
    {
        //Debug.Log("Got Gradient");
        return revGradient;
    }
    private float CurrentRevs;
    [SerializeField] private float RedLine = 5f;
    public float Pitchvalue;
    [Range(1f, 5f)]
    public float PitchSenstivity;
    [Range(0.01f, 30f)]
    public float Interpolation = 5f;
    [Range(0f, 1f)]
    public float m_SteerHelper = 0.5f;
    public float steerHelpCoe = 1f;
    float iSteer;
    public float currentSpud;
    float localVelocity;
    float currentD;
    float m_OldRotation;
    public TextMeshProUGUI SpeedDisplay;
    [SerializeField] private TextMeshProUGUI MetricUnitLabel;
    public TextMeshProUGUI gearNumDisplay;
    [Header("Engine")]
    public AudioSource EngineSound;
    public AudioSource GearShift;
    [Space]
    [Range(0.0f, 1.0f)]
    public float[] GearRatios;
    [Space]
    [Range(0.0f, 200.0f)]
    public float[] GearThresholds;
    float GearShiftRatio;
    [SerializeField] private float upperREV = 1f;
    [SerializeField] private float lowerREV = 0.7f;
    public AudioSource BackFireSound;
    [Header("Traction Control")]
    [SerializeField] private float m_SlipLimit;
    [SerializeField] private float m_CurrentTorque;
    [Range(0, 1)][SerializeField] private float m_TractionControl;

    [Header("Tail Lights")]
    [SerializeField] private bool HasLights = true;
    [SerializeField] private MeshRenderer TailLights;
    [SerializeField] private Material[] TailLight_M;
    [Header("Air Trails")]
    [SerializeField] private TrailRenderer[] airTrails;
    [SerializeField] private ParticleSystem[] m_ExhaustParticles;
    [SerializeField] private ParticleSystem[] airParticles;

    [Header("Brake Discs")]
    [SerializeField] private bool hasBrakeDiscs;
    [SerializeField] private Renderer[] brakeMeshes;
    [SerializeField] private Color brakeColor;
    float increaseTemp;

    [Header("Lights")]
    public bool nightTime = false;
    [SerializeField] private Material[] HeadLight_M;
    [SerializeField] private MeshRenderer headLightsMesh;
    [SerializeField] private GameObject[] headLights;
    [SerializeField] private GameObject[] tailLights;

    [SerializeField] private AudioSource brakeSound;
    public ComboDisplay CDisplay;
    public float powerRatio = 0.5f;
    #endregion

    bool notReverse = false;

    bool engineLerp = true;

    // Start is called before the first frame update
    void Start()
    {
        if (nightTime)
        {
            IM.light_ = true;
        }
        else
        {
            IM.light_ = false;
        }
        DA = GetComponent<DriftAngle>();
        rb = GetComponent<Rigidbody>();
        iSteer = steerHelpCoe;
        EngineSound.Play();
        GearShiftRatio = 2.8f * motorForcelol / 10f;
        m_CurrentTorque = motorForcelol - (m_TractionControl * motorForcelol);
        if (PlayerPrefs.GetInt("MetricUnit") == 0)
        {
            isMph = false;
        }
        else
        {
            isMph = true;
        }
        wmass = m_WheelColliders[0].mass;
    }


    // Update is called once per frame
    void Update()
    {
        if(EngineSound.isPlaying == false)
        {
            EngineSound.Play();
        }
        if(BackFireSound.isPlaying == false)
        {
            BackFireSound.Play();
        }
        if(brakeSound.isPlaying == false)
        {
            brakeSound.Play();
        }
        m_SteerHelper = 10 / (currentSpud + 1f);
        Lights(HasLights);
        currentSpud = Mathf.Round(Mathf.Abs(rb.linearVelocity.magnitude) * 1.55f);
        localVelocity = Mathf.Round(transform.InverseTransformVector(rb.linearVelocity).z * 1.55f);
        float revValue = CurrentRevs / 2.5f;
        revdisplay.color = revGradient.Evaluate(Revs);
        notReverse = localVelocity * IM.Throttle > -10f ? true : false;
        if (!notReverse)
        {
            brakeSound.volume = 0.08f * Mathf.Clamp(localVelocity * -IM.Throttle / 120f, 0f, 1f);
        }
        else
        {
            brakeSound.volume = 0f;
        }
        if (localVelocity > 100f)
        {
            foreach (TrailRenderer TR in airTrails)
            {
                TR.emitting = true;
            }
        }
        else
        {
            foreach (TrailRenderer TR in airTrails)
            {
                TR.emitting = false;
            }
        }
        if (localVelocity > 180f)
        {
            foreach (ParticleSystem Ps in airParticles)
            {
                Ps.Play();
            }
        }
        else
        {
            foreach (ParticleSystem Ps in airParticles)
            {
                Ps.Stop();
            }
        }
        currentsteervalue = Mathf.Lerp(currentsteervalue, IM.Steer, Dampning * Time.deltaTime);
        m_SpeedAccumulator++;
        if (Time.realtimeSinceStartup > m_SpeedNextPeriod)
        {
            m_SpeedNextPeriod += SpeedMeasurePeriod;
            float changeUnit = isMph ? 1f : 1.6f;
            SpeedDisplay.text = Mathf.Round(currentSpud * changeUnit) + "";
            if (isMph)
            {
                MetricUnitLabel.text = "MPH";
            }
            else
            {
                MetricUnitLabel.text = "KMPH";
            }
            gearNumDisplay.text = calculateGearText();

        }
        EngineSound.pitch = CurrentRevs;
        EngineSound.volume = Mathf.Lerp(EngineSound.volume, Mathf.Clamp(Mathf.Abs(IM.Throttle) + 0.3f, 0, 1), 0.02f);
        Pitchvalue = Mathf.Lerp(Pitchvalue, Revs * 1.3f * PitchSenstivity * Mathf.Abs(Mathf.Round(IM.Throttle)), 5 * Time.deltaTime);

        if (IM.light_)
        {
            foreach (GameObject L in headLights)
            {
                L.SetActive(true);
                if (headLightsMesh.sharedMaterial != HeadLight_M[1])
                {
                    headLightsMesh.sharedMaterial = HeadLight_M[1];
                }
            }
        }
        else
        {
            if (headLightsMesh == null) return;
            foreach (GameObject L in headLights)
            {
                L.SetActive(false);
                if (headLightsMesh.sharedMaterial != HeadLight_M[0])
                {
                    headLightsMesh.sharedMaterial = HeadLight_M[0];
                }
            }
        }
        //float gearNum;
    }
    public bool neutral;
    public string calculateGearText()
    {
        if (Mathf.Abs(IM.Throttle) > 0.1f || IM.burnOutBool && currentSpud > 1f)
        {
            if (localVelocity > -0.001f)
            {
                if (m_GearNum == 0f)
                {
                    return neutral ? "N" : "1";
                }
                else
                {
                    return neutral ? "N" : m_GearNum + 1 + "";
                }
            }
            else
            {
                return "R";
            }
        }
        else if (neutral)
        {
            return "N";
        }
        else
        {
            return m_GearNum + 1 + "";
        }
        return "";
    }
    private void FixedUpdate()
    {
        if (Mathf.Abs(IM.Throttle) < 0.01f)
        {
            engineLerp = false;
        }
        WheelTorque();
        CalculateDownFoce(downforce);
        CapSpeed();
        //CalculateGearFactor();
        CalculateRevs();
        GearChanging();
        Steering();
        SteerHelper();
        rb.angularDamping = currentSpud * angularDragCoeff / 100f;
        CurrentRevs = Mathf.Clamp(mappedValue(Revs, 0f, 1f, 0.3f, 2.5f), 0.3f, 2.5f);

        float y = currentSpud / maxSpeed;
        if ((localVelocity * IM.Throttle < -0.1f) && localVelocity > 20f)
        {
            increaseTemp = Mathf.Lerp(increaseTemp, 8f, 0.0005f * y + 0.005f);
        }
        else
        {
            increaseTemp = Mathf.Lerp(increaseTemp, -2f, 0.0003f * y + 0.002f);
        }
        if (hasBrakeDiscs)
        {
            for (int i = 0; i < brakeMeshes.Length; i++)
            {
                if (i < 2)
                {
                    brakeMeshes[i].material.SetColor("_EmissionColor", Mathf.Clamp(increaseTemp, 0f, 6f) * brakeColor);
                }else{
                    brakeMeshes[i].material.SetColor("_EmissionColor", Mathf.Clamp(increaseTemp * 0.1f, 0f, 6f) * brakeColor);
                }
            }
        }

    }

    float mappedValue(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float ratio = (value - fromMin) / (fromMax - fromMin);
        float mappedVal = ratio * (toMax - toMin) + toMin;
        return mappedVal;
    }

    float hBoost = 1f;
    Vector3 lastVelocity;
    float accelerationThreshold = 73f;
    float accTemp;
    float accelLimiter = 1f;
    void WheelTorque()
    {
        if (IM.burnOutBool)
        {
            Debug.Log("Burn out");
        }
        Vector3 accelaration = (transform.InverseTransformVector(rb.linearVelocity) - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = transform.InverseTransformVector(rb.linearVelocity);
        if (!(localVelocity * IM.Throttle > -1f))
        {
            rb.AddForce(rb.linearVelocity * 1000f * rb.mass * 0.00005f * IM.Throttle);
            //Debug.Log(rb.velocity * 1000f * rb.mass * 0.00005f * IM.Throttle);
        }
        accTemp = accelerationThreshold * (1 - currentSpud / maxSpeed);
        if (Mathf.Abs(accelaration.z) > accTemp)
        {
            accelLimiter = Mathf.Lerp(accelLimiter, 0.7f, 0.5f);
        }
        else
        {
            accelLimiter = Mathf.Lerp(accelLimiter, 1f, 1f);
        }
        //Debug.Log(Mathf.Abs(accelaration.z) + "   " + accTemp + "   " + accelLimiter);
        float x = BoostFactor * CurrentRevs;
        float y = currentSpud / maxSpeed;
        float breakCoeff = localVelocity * IM.Throttle;
        float breakForce = IM.HandBrake ? breakForcelol : 0f;

        float driftBoost = Mathf.Clamp((1f-Mathf.Abs(Vector3.Dot(rb.linearVelocity.normalized, transform.forward))) * 4f, 1f, 2f);
        if (AWD)
        {
            if (currentSpud * IM.Throttle > -0.2f)
            {
                for (int i = 0; i < m_WheelColliders.Length; i++)
                {
                    if ((Mathf.Abs(IM.Throttle) >= 0.1f) && (m_WheelColliders[i].rpm < maxWheelRPM))
                    {
                        if (i < 2)
                        {
                            m_WheelColliders[i].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol + x * 1000) * enginePower.Evaluate(y) / (m_GearNum + 1) * IM.Throttle * (1f - powerRatio) * driftBoost * boostBias * accelLimiter * 4f * boostCurve.Evaluate(Revs), -motorForcelol * 3.5f, motorForcelol * 3.5f));
                        }
                        else
                        {
                            m_WheelColliders[i].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol + x * 1000) * enginePower.Evaluate(y) / (m_GearNum + 1) * IM.Throttle * powerRatio * driftBoost * boostBias * accelLimiter * 4f * boostCurve.Evaluate(Revs), -motorForcelol * 3.5f, motorForcelol * 3.5f));
                        }
                    }
                    else
                    {
                        m_WheelColliders[i].motorTorque = 0f;
                    }
                    //Debug.Log(Mathf.Clamp(motorForcelol/(m_GearNum + 1) * IM.Throttle  * 8f, -motorForcelol*3.3f , motorForcelol*3.3f));
                }
            }
            else
            {
                if ((Mathf.Abs(IM.Throttle) >= 0.1f) && ((m_WheelColliders[3].rpm < maxWheelRPM) && (m_WheelColliders[2].rpm < maxWheelRPM)))
                {
                    m_WheelColliders[3].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol * 3.5f + x * 1000) * enginePower.Evaluate(y) / (m_GearNum + 1) * IM.Throttle * boostBias * accelLimiter * 8f, -motorForcelol * 8f, motorForcelol * 8f));
                    m_WheelColliders[2].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol * 3.5f + x * 1000) * enginePower.Evaluate(y) / (m_GearNum + 1) * IM.Throttle * boostBias * accelLimiter * 8f, -motorForcelol * 8f, motorForcelol * 8f));
                }
                else
                {
                    m_WheelColliders[3].motorTorque = 0f;
                    m_WheelColliders[2].motorTorque = 0f;
                }
            }

        }
        else
        {
            if ((Mathf.Abs(IM.Throttle) >= 0.05f) && ((m_WheelColliders[3].rpm < maxWheelRPM) && (m_WheelColliders[2].rpm < maxWheelRPM)))
            {
                if (notReverse)
                {
                    m_WheelColliders[3].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol * 3.5f + x * 1000) * enginePower.Evaluate(y) / (m_GearNum + 4) * IM.Throttle * driftBoost * boostBias * accelLimiter * 4f * hBoost * boostCurve.Evaluate(Revs), -motorForcelol * 5f, motorForcelol * 8f));
                    m_WheelColliders[2].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol * 3.5f + x * 1000) * enginePower.Evaluate(y) / (m_GearNum + 4) * IM.Throttle * driftBoost * boostBias * accelLimiter * 4f * hBoost * boostCurve.Evaluate(Revs), -motorForcelol * 5f, motorForcelol * 8f));
                }
                else
                {
                    m_WheelColliders[3].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol * 3.5f + x * 1000) / (m_GearNum + 4) * IM.Throttle * boostBias * accelLimiter * 8f, -motorForcelol * 8f, motorForcelol * 8f));
                    m_WheelColliders[2].motorTorque = Mathf.Round(Mathf.Clamp((motorForcelol * 3.5f + x * 1000) / (m_GearNum + 4) * IM.Throttle * boostBias * accelLimiter * 8f, -motorForcelol * 8f, motorForcelol * 8f));
                }
            }
            else
            {
                m_WheelColliders[3].motorTorque = 0f;
                m_WheelColliders[2].motorTorque = 0f;
            }
        }

        if (IM.burnOutBool && currentSpud < 15f)
        {
            float brakeF = (float)((int)Random.Range(0.85f, 10f));
            rb.AddForce(transform.forward * 2000f);
            m_WheelColliders[0].brakeTorque = brakeF;
            m_WheelColliders[1].brakeTorque = brakeF;
            if (m_WheelColliders[2].rpm < 5000f && m_WheelColliders[3].rpm < 5000f)
            {
                m_WheelColliders[2].motorTorque = 10000f;
                m_WheelColliders[3].motorTorque = 10000f;
            }
            rb.AddTorque(transform.up * IM.Steer * 20000f);


        }
        else if (Mathf.Abs(IM.Throttle) > 0.1f)
        {
            m_WheelColliders[0].brakeTorque = 0f;
            m_WheelColliders[1].brakeTorque = 0f;
        }
        
        m_WheelColliders[3].brakeTorque = breakForce;
        m_WheelColliders[2].brakeTorque = breakForce;

        foreach (WheelCollider w in m_WheelColliders)
        {
            if (currentSpud >= 1f)
            {
                w.mass = wmass;
            }
            else
            {
                w.mass = 50f;
            }
        }
        if(currentSpud < 5f && currentSpud > -5f && !IM.HandBrake && Mathf.Abs(IM.ThrottleValue) < 0.2f){
            m_WheelColliders[3].brakeTorque = 100000f;
            m_WheelColliders[2].brakeTorque = 100000f;
        }
        //Debug.Log(" RPM(1): " +  m_WheelColliders[0].rpm + " RPM(2): " +  m_WheelColliders[1].rpm + " RPM(3): "  + m_WheelColliders[2].rpm + " RPM(4): " + m_WheelColliders[3].rpm);
    }
    float gearChangeDelay = .5f;
    float gearChangeTime = 0f;

    private void GearChanging()
    {
        float f = Mathf.Abs(currentSpud / GearShiftRatio) * 1.3f;
        float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
        float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;
        float RV = 0;
        gearChangeTime += Time.deltaTime;

        if (gearChangeTime * 5f < gearChangeDelay)
        {
            neutral = true;
        }
        else
        {
            neutral = false;
        }
        if (m_GearNum > 0 && f < downgearlimit && gearChangeTime > gearChangeDelay)
        {
            gearChangeTime = 0f;
            engineLerp = false;
            if (Revs < lowerREV)
            {
                Revs += 0.2f;
            }
            else
            {
                Revs = lowerREV;
            }
            Pitchvalue = 0.2f;
            m_GearNum--;
            //Vibrator.Vibrate(50);
            if (!GearShift.isPlaying)
            {
                GearShift.Play();
            }

            RV = Random.Range(0, 10);
            if (!IM.Nos_ && RV > 8)
            {
                for (int i = 0; i < m_ExhaustParticles.Length; i++)
                {
                    m_ExhaustParticles[i].Clear();
                }
                for (int i = 0; i < m_ExhaustParticles.Length; i++)
                {
                    m_ExhaustParticles[i].Play();
                }
            }
            if (m_GearNum > 2)
            {

                Pitchvalue *= 1.1f;
            }
            BackFireSound.volume = .6f;
        }

        if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)) && gearChangeTime > gearChangeDelay)
        {
            gearChangeTime = 0f;
            engineLerp = false;
            if (Revs < lowerREV)
            {
                Revs += 0.2f;
            }
            else
            {
                Revs = lowerREV;
            }
            Pitchvalue = 0.2f;
            m_GearNum++;
            //Vibrator.Vibrate(50);
            if (!GearShift.isPlaying)
            {
                GearShift.Play();
            }
            RV = Random.Range(0, 10);
            if (!IM.Nos_ && RV > 8)
            {
                for (int i = 0; i < m_ExhaustParticles.Length; i++)
                {
                    m_ExhaustParticles[i].Clear();
                }
                for (int i = 0; i < m_ExhaustParticles.Length; i++)
                {
                    m_ExhaustParticles[i].Play();
                }
            }
            if (m_GearNum > 2)
            {

                Pitchvalue *= 1.1f;
            }
            BackFireSound.volume = .6f;
        }
    }


    void Steering()
    {

        driftBias = Mathf.Lerp(driftBias, DA.Angle, Dampning * 0.02f);
        CurrentSteerAngle = stearingAnglelol * currentsteervalue;
        m_WheelColliders[0].steerAngle = CalcSteeringAngle();
        m_WheelColliders[1].steerAngle = CalcSteeringAngle();
        if (IM.useDriftSteering)
        {
            steerHelpCoe = 0f;
        }
        else
        {
            steerHelpCoe = iSteer;
        }
        //rb.AddTorque(transform.up * Mathf.Clamp(motorForcelol/currentSpud * currentsteervalue * 1000f,-10000f,10000f));
    }
    public float CalcSteeringAngle()
    {
        float angle = Mathf.Round(Mathf.Clamp((stearingAnglelol * currentsteervalue * 50f / (currentSpud + 10)), -stearingAnglelol, stearingAnglelol));
        float x = Mathf.Round(Mathf.Clamp(1 / currentSpud, -Dampning, Dampning));
        if (currentSpud > 13f)
        {
            return IM.useDriftSteering ? Mathf.Clamp(Mathf.Lerp(CurrentSteerAngle, angle, Dampning * 0.02f * x * x * x) + driftBias * 0.7f, -75f, 75f) : Mathf.Clamp(Mathf.Lerp(CurrentSteerAngle, angle, Dampning * 0.02f), -80f, 80f);
        }
        else
        {
            return Mathf.Clamp(Mathf.Lerp(CurrentSteerAngle, angle, Dampning * 0.02f), -80f, 80f);
        }
    }
    void CalculateDownFoce(bool yes)
    {
        if (yes)
        {
            currentD = Mathf.Clamp(-downForce_f * currentSpud * currentSpud * 0.1f / 10f, -25000f, 0f);
            //Debug.Log(currentD);
            rb.AddForce(transform.up * currentD);
        }
    }
    void Lights(bool yes)
    {
        if (yes && TailLights != null)
        {
            if (IM.Throttle > -0.1f)
            {
                if (IM.light_)
                {
                    if (TailLights.sharedMaterial != TailLight_M[2])
                    {
                        TailLights.sharedMaterial = TailLight_M[2];
                    }
                }
                else
                {
                    if (TailLights.sharedMaterial != TailLight_M[1])
                    {
                        TailLights.sharedMaterial = TailLight_M[1];
                    }
                }
                foreach (GameObject L in tailLights)
                {
                    L.SetActive(false);
                }
            }
            else
            {
                if (TailLights.sharedMaterial != TailLight_M[0])
                {
                    TailLights.sharedMaterial = TailLight_M[0];
                }

                foreach (GameObject L in tailLights)
                {
                    L.SetActive(true);
                }
            }
        }
    }
    float factorDifference;
    float target;
    private void CalculateGearFactor()
    {
        float f = (1 / (float)NoOfGears);
        var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(currentSpud / GearShiftRatio));
        target = Mathf.InverseLerp(m_GearNum / 10f, (m_GearNum + 1) / 10f, Mathf.Abs(currentSpud / GearShiftRatio));
        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * Interpolation);
        factorDifference = targetGearFactor - m_GearFactor;
        //Debug.Log(targetGearFactor);
    }
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }

    float velocity_ = 0f;
    [SerializeField] private AnimationCurve RPMCurve;
    float load;
    float z;
    
    private void CalculateRevs()
    {
        CalculateGearFactor();
        float wheelSpeed = m_WheelColliders[2].rpm * m_WheelColliders[2].radius;
        var gearNumFactor = m_GearNum / (float)NoOfGears;
        var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
        float y = currentSpud / maxSpeed;
        load = wheelSpeed > 1500f || wheelSpeed < 10f || IM.HandBrake ? 1f : 0.1f;
        z = Mathf.Lerp(z, Mathf.Abs(IM.Throttle) > 0.1f ? 0f : target * 0.5f - Mathf.Clamp(10 - m_GearNum, 0f, 0.8f) / 10f, Interpolation);
        if (engineLerp)
        {
            float targetRev = Mathf.Abs(IM.Throttle - 0.3f * m_GearFactor);
            if (!notReverse)
            {
                targetRev = Mathf.Abs(0.3f * m_GearFactor);
            }
            Revs = Mathf.Lerp(Revs, targetRev, Mathf.Abs(GearRatios[m_GearNum] * RPMCurve.Evaluate(Revs) * load));
        }
        else
        {
            Revs = Mathf.Lerp(Revs, z, (1f - RPMCurve.Evaluate(Revs)) * load);
        }
        if (Revs > upperREV && m_GearNum != NoOfGears - 2 && m_GearNum < 3)
        {
            Vibrator.Vibrate(100);
            BackFireSound.volume = 1f;
            engineLerp = false;
            float R = Random.Range(0, m_ExhaustParticles.Length);
            float f = Random.Range(0f, 10f);
            float pitch_ = Random.Range(0.2f, 1.1f);
            BackFireSound.pitch = pitch_;
            if (f > 7f)
            {
                for (int i = 0; i < m_ExhaustParticles.Length; i++)
                {
                    if (i <= R)
                    {
                        m_ExhaustParticles[i].Play();
                    }
                }
            }
        }

        if (IM.burnOutBool)
        {
            Revs = Mathf.Lerp(Revs, Mathf.Abs(0.85f * m_GearFactor), Mathf.Abs(GearRatios[m_GearNum] * RPMCurve.Evaluate(Revs) * load));
        }

        if (Revs < lowerREV - Random.Range(0.1f, 0.3f))
        {
            engineLerp = true;
            BackFireSound.volume = Mathf.Lerp(BackFireSound.volume, 0f, 0.3f);
        }
    }
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
    private void CapSpeed()
    {
        float speed = rb.linearVelocity.magnitude;
        speed *= 1.55f;
        if (speed > maxSpeed)
        {
            //rb.velocity = (maxSpeed/1.55f) * rb.velocity.normalized;
            accelLimiter = 0.05f;
        }
        if (speed < minSpeed)
        {
            //rb.velocity = (minSpeed/-1.55f) * rb.velocity.normalized;
            accelLimiter = 0.05f;
        }
        // needs work

    }

    private void SteerHelper()
    {

        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper * steerHelpCoe;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rb.linearVelocity = velRotation * rb.linearVelocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
        Debug.DrawRay(transform.position, rb.linearVelocity.normalized * 10f, Color.blue);
    }
    void OnGUI()
    {
        float pos = 30;
        if (Debug_)
        {

            GUI.Label(new Rect(10, pos, 100, 20), "Throttle: ");
            pos += 25;
            GUI.HorizontalSlider(new Rect(10, pos, 100, 10), IM.Throttle, -1, 1);
            pos += 25;
            GUI.Label(new Rect(10, pos, 100, 20), "Steering Angle: ");
            pos += 25;
            GUI.HorizontalSlider(new Rect(10, pos, 100, 10), currentsteervalue, -1, 1);
            pos += 25;
            GUI.Label(new Rect(10, pos, 300, 20), "Revs: " + Revs);
            pos += 25;
            GUI.HorizontalSlider(new Rect(10, pos, 100, 10), Revs, 0, 1);
            pos += 25;
            GUI.HorizontalSlider(new Rect(10, pos, 100, 10), target, 0, 1);


        }
    }

    void TractionControl()
    {

        WheelHit wheelHit;
        for (int i = 0; i < 4; i++)
        {
            m_WheelColliders[i].GetGroundHit(out wheelHit);
            AdjustTorque(wheelHit.forwardSlip);
        }
    }
    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
        {
            m_CurrentTorque -= 10 * m_TractionControl;
        }
        else
        {
            m_CurrentTorque += 10 * m_TractionControl;
            if (m_CurrentTorque > motorForcelol)
            {
                m_CurrentTorque = motorForcelol;
            }
        }
    }


}
