using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticSpoiler : MonoBehaviour
{
    public PlayerControls controls;
    [SerializeField] private Animator SpoilerAnim;
    [SerializeField] private bool Activate;
    [SerializeField] private bool Brake;
    public float deployCoeff = 0f;
    public float brakeCoeff = 0f;
    public float speed = 1f;
    [SerializeField] private float downForce = 20f;
    [SerializeField] private float dragAdd = .02f;
    private float dragvalue = 0f;
    public float ThrottleValue;
    [SerializeField] private Rigidbody rb;
    private float currentSpud = 0f;
    public bool Active = false;


    private void Awake() {
        
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        dragvalue = rb.linearDamping;
        controls.Gameplay.Accelarate.performed += ctx => ThrottleValue = ctx.ReadValue<float>();
        controls.Gameplay.Accelarate.canceled += ctx => ThrottleValue = 0f;
        controls.Gameplay.Special.performed += ctx => Active = !Active; 
    }

    void FixedUpdate()
    {
        currentSpud = Mathf.Round(rb.linearVelocity.magnitude * 1.8f);

        if((ThrottleValue < 0) && (currentSpud > 40f)){
            Brake = true;
        }else{
            Brake = false;
        }

        if(Active){
            Activate = true;
            rb.AddForce(downForce * currentSpud *currentSpud * -transform.up);
            rb.linearDamping = dragvalue + dragAdd;
        }else{
            Activate = false;
            rb.linearDamping = dragvalue;
        }


        if(Activate){
            deployCoeff = Mathf.Lerp(deployCoeff,0f,speed); 
        }else{
            deployCoeff = Mathf.Lerp(deployCoeff,1f,speed);
        }

        if(Brake && Activate){
            brakeCoeff = Mathf.Lerp(brakeCoeff,1f,speed);
            rb.linearDamping = dragvalue + 3f * dragAdd;
        }else{
            brakeCoeff = Mathf.Lerp(brakeCoeff,0f,speed);
            rb.linearDamping = dragvalue;
        }
        SpoilerAnim.SetFloat("Deploy",deployCoeff);
        SpoilerAnim.SetFloat("Brake",brakeCoeff);
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
