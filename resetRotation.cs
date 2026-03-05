using UnityEngine;

public class resetRotation : MonoBehaviour {
    
    private void Update() {
        transform.eulerAngles = Vector3.zero;
    }
}