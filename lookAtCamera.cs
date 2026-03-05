using UnityEngine;
using Cinemachine;

public class lookAtCamera : MonoBehaviour {

    [SerializeField] private Animator lookAtAnim;
    [SerializeField] private float damp = 0f;
    [SerializeField] private Rigidbody rb; 
    [SerializeField] private CarController cc; 

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineFramingTransposer transposer;
    CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] private float Zdamp = 0.06f;
    
    float localspud;
    float positionCoeff = 0;

    float baseFov = 70;
    private void Start() {
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Invoke("SetUp",0.1f);
        baseFov = 70;
    }
    private void FixedUpdate() {

        if(rb != null){
            localspud = Mathf.Round(transform.InverseTransformVector(rb.linearVelocity).z *1.8f);
        }else{
            localspud = 0f;
        }
        
        if(localspud < -10f){
            positionCoeff = Mathf.Lerp(positionCoeff, 1f, damp);
            if(positionCoeff > 0.999f){
                positionCoeff = 1f;
            }
        }else{
            positionCoeff = Mathf.Lerp(positionCoeff, 0f, damp);
            if(positionCoeff < 0.001f){
                positionCoeff = 0f;
            }
        }
        lookAtAnim.SetFloat("Position", positionCoeff);
        //Debug.Log(positionCoeff);
        if(cc != null){
            if(cc.IM.Throttle > 0f && !cc.neutral){
                transposer.m_ZDamping = Mathf.Clamp(Mathf.Lerp(transposer.m_ZDamping,cc.Pitchvalue * Zdamp,0.015f),0.01f,0.1f);
            }else{
                transposer.m_ZDamping = Mathf.Lerp(transposer.m_ZDamping,0.01f,0.015f);
            }
            noise.m_FrequencyGain = cc.currentSpud/25f + 0.1f;
            cinemachineVirtualCamera.m_Lens.FieldOfView = baseFov + Mathf.Clamp(cc.currentSpud/15f,0f,10f);
        }
    }

    private void SetUp () {
        rb = this.transform.parent.GetComponent<Rigidbody>();
        cc = this.transform.parent.GetComponent<CarController>();
    }
}