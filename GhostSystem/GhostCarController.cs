using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

[RequireComponent(typeof(GhostPlayer))]
[RequireComponent(typeof(Rigidbody))]
public class GhostCarController : MonoBehaviour {
    [Range(-1, 1)]public float Throttle;
    [Range(-1, 1)]public float Steer;
    public bool HandBrake;
    public bool Nos_;
    #region Variables
    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [Range (0.01f , 5f)]
    float wmass;
    [SerializeField] private float[] GearRatio = new float[4];
    public bool AWD = true;
    [SerializeField] private float angularDragCoeff = 1f;
    [SerializeField] private bool Debug_ = true;
    [SerializeField] private bool downforce;
    public Rigidbody rb;
    [Header("Motor Values")]
    [SerializeField] private AnimationCurve enginePower;
    public float motorForcelol = 1000f;
    [SerializeField] private float maxWheelRPM = 15000f;
    [SerializeField] private float breakForcelol = 2000000f;
    [SerializeField] private float downForce_f = 100f;
    public float stearingAnglelol = 20f;
    public float CurrentSteerAngle;
    float driftBias = 0f;
    [SerializeField] private float Dampning = 0.1f;
    public float currentsteervalue = 0f;
    public float maxSpeed = 200f;
    public float minSpeed = -40f;
    const float SpeedMeasurePeriod = 0.1f;
    private int m_SpeedAccumulator = 0;
    private float m_SpeedNextPeriod = 0;
    [SerializeField] public int NoOfGears = 8;
    public float BoostFactor = 0f;
    [SerializeField] private float m_RevRangeBoundary = 1f;
    public int m_GearNum;
    private float m_GearFactor;
    public float Revs;
    private float CurrentRevs;
    [SerializeField] private float RedLine = 5f;
    public float Pitchvalue;
    [Range (1f , 5f)]
    public float PitchSenstivity;
    [Range (0.01f , 30f)]
    public float Interpolation = 5f;
    [Range(0f, 1f)]
    public float m_SteerHelper = 0.5f;
    public float steerHelpCoe = 1f;
    float iSteer;
    public float currentSpud;
    float localVelocity;
    float currentD;
    float m_OldRotation;

    [Header("Engine")]
    public AudioSource EngineSound;
    public AudioSource GearShift;
    [Space]
    [Range(0.0f, 1.0f)]
    public float[] GearRatios;
    [Space]
    [Range(0.0f , 200.0f)]
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

    [SerializeField] private ParticleSystem[] m_ExhaustParticles;

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
    #endregion

    bool engineLerp = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        iSteer = steerHelpCoe;
        EngineSound.Play();
        GearShiftRatio = 2.8f *motorForcelol/10f;
        m_CurrentTorque = motorForcelol - (m_TractionControl*motorForcelol);
        wmass = m_WheelColliders[0].mass;
    }

    // Update is called once per frame
    void Update()
    {
        m_SteerHelper = 10/(currentSpud+1f);
        Lights(HasLights);
        currentSpud = Mathf.Round((Mathf.Abs(rb.linearVelocity.magnitude) * 1.55f));
        localVelocity = Mathf.Round((transform.InverseTransformVector(rb.linearVelocity).z *1.55f));
        float revValue = CurrentRevs/2.5f;
        currentsteervalue = Mathf.Lerp( currentsteervalue , Steer , Dampning * Time.deltaTime);
        EngineSound.pitch = CurrentRevs;
        EngineSound.volume = Mathf.Lerp(EngineSound.volume,Mathf.Clamp(Mathf.Abs(Throttle) + 0.3f,0,1), 0.02f);
        Pitchvalue = Mathf.Lerp( Pitchvalue , Revs * 1.3f * PitchSenstivity * Mathf.Abs(Mathf.Round(Throttle)), 5 * Time.deltaTime);
        
        if(nightTime){
            foreach (GameObject L in headLights)
            {
                L.SetActive(true);
                if(headLightsMesh.material != HeadLight_M[1]){
                    headLightsMesh.material = HeadLight_M[1];
                }
            }
        }else{
            foreach (GameObject L in headLights)
            {
                L.SetActive(false);
                if(headLightsMesh.material != HeadLight_M[0]){
                    headLightsMesh.material = HeadLight_M[0];
                }
            }
        }
        //float gearNum;
    }
    private void FixedUpdate()
    {
        if(Mathf.Abs(Throttle) < 0.01f){
            engineLerp = false;
        }
        WheelTorque();
        CalculateDownFoce(downforce);
        CapSpeed();
        CalculateGearFactor();
        CalculateRevs();
        GearChanging();
        Steering();
        SteerHelper();
        rb.angularDamping = currentSpud*angularDragCoeff/100f;
        CurrentRevs = Mathf.Clamp(Pitchvalue , 0.3f , 2.5f);

        float y = currentSpud/maxSpeed;
        if((localVelocity * Throttle < -0.1f) && localVelocity > 10f){
            increaseTemp = Mathf.Lerp(increaseTemp,8f,0.002f*y + 0.01f);
        }else{
            increaseTemp = Mathf.Lerp(increaseTemp,-2f,0.002f*y + 0.005f);
        }
        if(hasBrakeDiscs){
            foreach(MeshRenderer m in brakeMeshes) {
                    m.material.SetColor("_EmissionColor",Mathf.Clamp(increaseTemp,0f,10f) * brakeColor);
            }
        }
        
    }
    void WheelTorque(){
        float x = BoostFactor * CurrentRevs;
        float y = currentSpud/maxSpeed;
        float breakCoeff = localVelocity * Throttle;
        float breakForce = HandBrake ? breakForcelol : 0f;
        if(AWD){
            if((currentSpud+1)*Throttle > 0f){
                foreach(WheelCollider w in m_WheelColliders){
                    if((Mathf.Abs(Throttle) >= 0.1f)  && (w.rpm < maxWheelRPM)){
                        w.motorTorque = Mathf.Round(Mathf.Clamp(motorForcelol*enginePower.Evaluate(y) * Throttle  * 6f, -motorForcelol*3.5f , motorForcelol*3.5f));
                    }else{
                        w.motorTorque = 0f;
                    }
                }
            }
             
        }else {
            if((Mathf.Abs(Throttle) >= 0.1f) && ((m_WheelColliders[3].rpm < maxWheelRPM) && (m_WheelColliders[2].rpm < maxWheelRPM))){
                m_WheelColliders[3].motorTorque = Mathf.Round(Mathf.Clamp(motorForcelol* Throttle  * 8f, -motorForcelol *5f , motorForcelol*8f));
                m_WheelColliders[2].motorTorque = Mathf.Round(Mathf.Clamp(motorForcelol* Throttle  * 8f, -motorForcelol *5f , motorForcelol*8f));   
            }else{
                m_WheelColliders[3].motorTorque = 0f;
                m_WheelColliders[2].motorTorque = 0f;
            }
        }
        m_WheelColliders[3].brakeTorque = breakForce;
        m_WheelColliders[2].brakeTorque = breakForce;

        foreach(WheelCollider w in m_WheelColliders){
            if(currentSpud >= 1f){
                w.mass = wmass;
            }else{
                w.mass = 50f;
            }
        }

        //Debug.Log(" RPM(1): " +  m_WheelColliders[0].rpm + " RPM(2): " +  m_WheelColliders[1].rpm + " RPM(3): "  + m_WheelColliders[2].rpm + " RPM(4): " + m_WheelColliders[3].rpm);
    }
    private void GearChanging()
        {
            float f = Mathf.Abs(currentSpud/GearShiftRatio) * 1.3f;
            float upgearlimit = (1/(float) NoOfGears) * (m_GearNum + 1);
            float downgearlimit = (1/(float) NoOfGears) * m_GearNum;
            float RV = 0;

            
            if (m_GearNum > 0 && f < downgearlimit )
            {   
                engineLerp = false;
                Revs = lowerREV;
                Pitchvalue = 0.2f;
                m_GearNum--;
                //Vibrator.Vibrate(50);
                if(!GearShift.isPlaying){
                    GearShift.Play();
                }
                
                RV = Random.Range(0,10);
                if(!Nos_ && RV > 8) 
                {
                    BackFireSound.volume = .6f;
                    for (int i = 0; i < m_ExhaustParticles.Length; i++)
                    {
                        m_ExhaustParticles[i].Clear();
                    }
                    for (int i = 0; i < m_ExhaustParticles.Length; i++)
                    {
                        m_ExhaustParticles[i].Play();
                    }
                }
                if(m_GearNum > 2)
                {

                    Pitchvalue *= 1.1f;
                }
            
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)) )
            {
                engineLerp = false;
                Revs = lowerREV;
                Pitchvalue = 0.2f;
                m_GearNum++;
                //Vibrator.Vibrate(50);
                if(!GearShift.isPlaying){
                    GearShift.Play();
                }
                RV = Random.Range(0,10);
                if(!Nos_ && RV > 8) 
                {
                    BackFireSound.volume = .6f;
                    for (int i = 0; i < m_ExhaustParticles.Length; i++)
                    {
                        m_ExhaustParticles[i].Clear();
                    }
                    for (int i = 0; i < m_ExhaustParticles.Length; i++)
                    {
                        m_ExhaustParticles[i].Play();
                    }
                }
                if(m_GearNum > 2)
                {

                    Pitchvalue *= 1.1f;
                }
                
            }
        }


    void Steering(){

        CurrentSteerAngle = stearingAnglelol * currentsteervalue;
        m_WheelColliders[0].steerAngle = CurrentSteerAngle;
        m_WheelColliders[1].steerAngle = CurrentSteerAngle;
        steerHelpCoe = 0f;
        //rb.AddTorque(transform.up * Mathf.Clamp(motorForcelol/currentSpud * currentsteervalue * 1000f,-10000f,10000f));
    }
    void CalculateDownFoce(bool yes){
        if(yes){
            currentD = -downForce_f * currentSpud * currentSpud * 0.02f / 10f;
            rb.AddForce(transform.up * currentD);
        }
    }
    void Lights(bool yes){
        if(yes && TailLights != null){
                if(Throttle > -0.1f){
                    if(nightTime){
                        if(TailLights.sharedMaterial != TailLight_M[2]){
                            TailLights.sharedMaterial = TailLight_M[2];
                        }
                    }else{
                        if(TailLights.sharedMaterial != TailLight_M[1]){
                            TailLights.sharedMaterial = TailLight_M[1];
                        }
                    }
                    foreach (GameObject L in tailLights)
                    {
                        L.SetActive(false);
                    }
                }else{
                    if(TailLights.sharedMaterial != TailLight_M[0]){
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
    private void CalculateGearFactor()
        {
            float f = (1/(float) NoOfGears);
            var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(currentSpud/GearShiftRatio));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime* Interpolation);
            factorDifference = targetGearFactor - m_GearFactor;
        }
    private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor)*(1 - factor);
        }

    float velocity_ = 0f;
    [SerializeField] private AnimationCurve RPMCurve;
    private void CalculateRevs()
    {
            CalculateGearFactor();
            var gearNumFactor = m_GearNum/(float) NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            float y = currentSpud/maxSpeed;
            if(engineLerp){
                Revs = Mathf.Lerp(Revs,Mathf.Abs(Throttle - 0.3f * m_GearFactor),Mathf.Abs(GearRatios[m_GearNum] * RPMCurve.Evaluate(Revs)));
            }else{
                Revs = Mathf.Lerp(Revs,0f,(1f - RPMCurve.Evaluate(Revs)));
            }
            if(Revs > upperREV && m_GearNum != NoOfGears - 2 && m_GearNum < 3){
                BackFireSound.volume = .6f;
                engineLerp = false;
                float R = Random.Range(0,m_ExhaustParticles.Length);
                float pitch_ = Random.Range(0.2f,1.1f);
                BackFireSound.pitch = pitch_;
                for (int i = 0; i < m_ExhaustParticles.Length; i++)
                {
                    if(i <= R){
                        m_ExhaustParticles[i].Play();
                    }
                }


            }

            if(Revs < lowerREV - Random.Range(0.1f,0.3f)){
                engineLerp = true;
                BackFireSound.volume = Mathf.Lerp(BackFireSound.volume,0f,0.3f);
            }
            //Revs = Mathf.SmoothDamp(Revs,0.1f + Mathf.Abs(IM.Throttle),ref velocity_,1/(GearRatios[m_GearNum]/(1f + m_GearNum)));
            //Revs = Mathf.Lerp(Revs,0.1f + Mathf.Abs(IM.Throttle),GearRatios[m_GearNum]/(1f + m_GearNum));
            //Revs = Mathf.Lerp(Revs,ULerp(revsRangeMin, revsRangeMax, m_GearFactor), 0.8f)/Mathf.Clamp(enginePower.Evaluate(y),0.85f,1f);
    }
    private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }
    private void CapSpeed()
    {
            float speed = rb.linearVelocity.magnitude;
            speed *= 1.55f;
            if (speed > maxSpeed)
            rb.linearVelocity = (maxSpeed/1.55f) * rb.linearVelocity.normalized;
            if(speed < minSpeed)
            rb.linearVelocity = (minSpeed/-1.55f) * rb.linearVelocity.normalized;

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

    void TractionControl(){
        
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