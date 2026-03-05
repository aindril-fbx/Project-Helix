using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using UnityEngine.Audio;


public class Nos : MonoBehaviour
{
    [SerializeField] private VisualEffect[] nos;
    [SerializeField] private ParticleSystem[] nos_ps;
    [SerializeField] private InputManager IM;
    [SerializeField] private CarController CC;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Slider NosBar;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float NosValue;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float depletionRate;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float ReplenishRate;
    [SerializeField] private float ReplenishDelay = 1.5f;
    public Gradient FuelGradient;
    [SerializeField] private Image fillImage;
    public bool replenish;
    [SerializeField] private AudioSource nosSound;
    float timer_;

    private float motorF = 500f;

    public float GetNosValue(){
        return NosValue;
    }
    public void setMotorForce(float force){
        motorF = force;
    }
    [SerializeField] private float NosTorquelol = 1200f;
    private void Start()
    {
        IM = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        CC = GetComponent<CarController>();
        NosValue = 100f;
    }
    // Update is called once per frame
    void Update()
    {
        NosBar.value = NosValue; 
        fillImage.color = FuelGradient.Evaluate(NosBar.normalizedValue);
        if(IM.Nos_ && (NosValue > 0f)){
            timer_ = 0f;
            foreach (VisualEffect v in nos){
                v.Play();
            }
            foreach (ParticleSystem v in nos_ps){
                v.Play();
            }
            CC.motorForcelol = NosTorquelol;
            if(!nosSound.isPlaying){
                nosSound.Play();
            }
            replenish = false;
            NosValue = Mathf.Clamp(NosValue , 0 , 100) - depletionRate * Time.deltaTime;
        }else{
            foreach (VisualEffect v in nos){
                v.Stop();
            }
            foreach (ParticleSystem v in nos_ps){
                v.Stop();
            }
           
            nosSound.Stop();
            CC.motorForcelol = motorF;
            //if(!replenish){RA.Play();}
            timer_ += Time.deltaTime;
        }
        if(replenish){
            NosValue = Mathf.Clamp(NosValue , 0 , 100) + ReplenishRate * Time.deltaTime;
        }

        if(timer_ >= ReplenishDelay){
            replenish = true;
        }else{
            replenish = false;
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.layer == 9)
        {
            NosValue = Mathf.Clamp(NosValue + 30f, 0f,100f);
        }
    }
    
}
