using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
       cam = GameObject.FindWithTag("MainCamera").transform; 
    }
    
    private void OnEnable() {
       cam = GameObject.FindWithTag("MainCamera").transform; 
        
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(cam == null) return;
        transform.LookAt(transform.position + cam.forward);
    }
}
