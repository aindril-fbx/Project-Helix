using UnityEngine;

public class DfinishLine : MonoBehaviour {
    [SerializeField] private driftEvent DF;
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Car"){
            DF.eventStarted = false;
            DF.endEvent();
        }
    }
}