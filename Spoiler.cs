using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spoiler : MonoBehaviour
{
    public PlayerControls controls;
    [SerializeField] private float deployCoeff = 0f;
    [SerializeField] private float deploySpeed = 1f;
    public Animator SpoilerAnim;
    public bool Active = false;
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float downForce_ = 1000f;
    [SerializeField] private CarController CC;
    private void Awake() {
        controls = new PlayerControls();
        controls.Gameplay.Special.performed += ctx => Active = !Active;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(Active){
            deployCoeff = Mathf.Lerp(deployCoeff,0f,deploySpeed);
        }else{
            deployCoeff = Mathf.Lerp(deployCoeff,1f,deploySpeed);
        }
        SpoilerAnim.SetFloat("Deploy",deployCoeff);
        //downForce();
    }

    void downForce(){
        if(Active){
            rb.AddForce(-transform.up * downForce_ * CC.currentSpud * CC.currentSpud);
        }
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
