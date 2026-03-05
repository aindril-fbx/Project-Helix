using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{

public class MudGaurdsLOL : MonoBehaviour
{
    public Transform WheelHub;
    public Transform WheelT;
    public Quaternion idk;
    public CarController carController;
    public Vector3 Offset;
    public bool IfSteeringWheel = false;

    private void Start()
    {
        idk = transform.localRotation;
    }
    // Update is called once per frame
    void Update()
    {
        WheelHub.position = WheelT.position + Offset;
        
        if(IfSteeringWheel == true)
        {
            WheelHub.localRotation = idk * Quaternion.Euler(0, -1f * carController.CurrentSteerAngle, 0);
            
        }
        
        
    }
}
}
