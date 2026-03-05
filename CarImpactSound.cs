using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CarImpactSound : MonoBehaviour
{

    public AudioSource[] CrashSound;
    public float LowCrashCoe = 10f;
    public float ImpactCoe = 50f; 
    bool yes = false;
    public Rigidbody rb;

    Vector3 acceleration;
    Vector3 lastVelocity;

    [SerializeField] private Animator RingingAnimator;
    [SerializeField] private Volume hitEffectVolume;
    public float delayToReset = 5f;
    float delayTime = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision col)
    { 
        if (col.relativeVelocity.magnitude > LowCrashCoe)
        {
            MakeSound(Random.Range(0,3));
        }

        if (col.relativeVelocity.magnitude > ImpactCoe)
        {
            MakeSound(Random.Range(2,4));
        }
    }

    private void LateUpdate() {
        acceleration = (rb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = rb.linearVelocity;
        float x = Mathf.Clamp01(acceleration.magnitude/7500f);
        if(acceleration.magnitude > 1000f){
            hit_(x);
            Debug.Log(x);
        }
        if(delayTime < delayToReset)
        {
            delayTime += Time.deltaTime;
        }
        else
        {
            reset_();
        }
    }

    void MakeSound(int SoundIndex)
    {
        for(int i = 0; i < CrashSound.Length; i++) {
            if(CrashSound[i].isPlaying){
                return;
            }
        }
        if(!CrashSound[SoundIndex].isPlaying){
            CrashSound[SoundIndex].Play();
        }
    }

    void hit_(float x){
        if(hitEffectVolume == null)
        {
            return;
        }
        hitEffectVolume.weight += x;
        delayTime = 0;
        //Invoke("reset_",x*4f);
    }

    void reset_(){
        if(hitEffectVolume == null)
        {
            return;
        }
        hitEffectVolume.weight = Mathf.Lerp(hitEffectVolume.weight, 0f, 0.05f);
        if(hitEffectVolume.weight <= 0.01f){
            hitEffectVolume.weight = 0f;
            delayTime = 0f;
        }
    }
}
