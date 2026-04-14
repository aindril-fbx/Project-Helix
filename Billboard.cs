using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;
    private void Start()
    {
       cam = GameObject.FindWithTag("MainCamera").transform; 
    }
    
    private void OnEnable() {
       cam = GameObject.FindWithTag("MainCamera").transform; 
        
    }
    void LateUpdate()
    {
        if(cam == null) return;
        transform.LookAt(transform.position + cam.forward);
    }
}
