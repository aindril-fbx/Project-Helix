using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float radius = 3f;
    public float force = 300f;

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rib = nearbyObject.GetComponent<Rigidbody>();
            if (rib != null)
            {
                //float objectspeed = Mathf.Abs(transform.InverseTransformVector(rib.velocity).z) * 10f;
                rib.AddForce(Vector3.up * force / rib.mass, ForceMode.VelocityChange);
               
            }
        }
    }
}
