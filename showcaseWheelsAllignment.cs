using UnityEngine;

public class showcaseWheelsAllignment : MonoBehaviour {
    [SerializeField] private WheelCollider[] wheelColliders;
    public float steerAngle = 40f;
    public float motorTorque = 100f;
    private CarSelectionV2 CSV2;
    private void Start() {
        CSV2 = GetComponent<CarSelectionV2>();
    }

    private void FixedUpdate() {
        for(int i = 0; i < CSV2.Carlist.Length; i++) {
            if(CSV2.Carlist[i].gameObject.activeSelf){
                wheelColliders = CSV2.Carlist[i].GetComponent<CarStats>().wheelColliderReturn();
                CSV2.Carlist[i].GetComponent<WheelLocation>().showcase = true;
            }
        }

        wheelColliders[0].steerAngle = steerAngle;
        wheelColliders[1].steerAngle = steerAngle;
    }
}