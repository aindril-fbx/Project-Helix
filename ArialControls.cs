using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArialControls : MonoBehaviour
{
    [SerializeField] private InputManager IM;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float xForce = 10f;
    [SerializeField] private float yForce = 10f;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private float GroundCheckRadius = 0.5f;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        IM = GetComponent<InputManager>();
    }
    void FixedUpdate()
    {
        bool isGrounded;
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckRadius , GroundLayer);
        if(!isGrounded){
            rb.AddTorque(transform.right * xForce * IM.Throttle * 100f);
            rb.AddTorque(transform.forward * yForce * -IM.Steer * 100f);

        }
    }
}
