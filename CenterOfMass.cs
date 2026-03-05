using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMass : MonoBehaviour
    
{
    public Vector3 Centerofmass;
    
    public bool Awake;
    protected Rigidbody r;
    

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.centerOfMass = Centerofmass;
        r.WakeUp();
        Awake = !r.IsSleeping();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * Centerofmass, 0.1f);



    }
}
