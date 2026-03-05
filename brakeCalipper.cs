using UnityEngine;

public class brakeCalipper : MonoBehaviour {

    [SerializeField] private WheelCollider[] wheelColliders;
    [SerializeField] private Transform[] brakeCallipers;
    [SerializeField] private Vector3 Offset;
    void Update()
    {
        for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                wheelColliders[i].GetWorldPose(out position, out quat);
                if(i<=1){
                    brakeCallipers[i].transform.position = position + Offset;
                }else{
                    brakeCallipers[i].transform.position = position - Offset;
                }
                brakeCallipers[i].localRotation = Quaternion.Euler(0f, wheelColliders[i].steerAngle,0f);
               
            }
    }
}