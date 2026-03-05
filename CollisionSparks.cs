using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSparks : MonoBehaviour
{
    [SerializeField] private float sparkCoeff = 1f;
    [SerializeField] private GameObject sparkEffect;

    void OnCollisionEnter(Collision col){

        if(col.relativeVelocity.magnitude > sparkCoeff){
            foreach(ContactPoint contact in col.contacts) {
                GameObject.Instantiate(sparkEffect,contact.point,Quaternion.identity);
                
            }
        }
    }
}
