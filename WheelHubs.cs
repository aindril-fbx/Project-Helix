using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelHubs : MonoBehaviour
{
    public Transform WheelHub;
    public Transform WheelT;
    public float Bias = 1f;
    public CarController carController;
    public Vector3 Offset;
    public bool IfSteeringWheel = false;

    // Update is called once per frame
    void Update()
    {
        WheelHub.position = WheelT.position + Offset;
        
        if(IfSteeringWheel == true)
        {
            WheelHub.localRotation = Quaternion.Euler(0, Bias * carController.stearingAnglelol * WheelT.localRotation.y,0);
            
        }
    }
}
