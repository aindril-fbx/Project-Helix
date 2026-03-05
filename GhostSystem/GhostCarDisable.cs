using UnityEngine;

public class GhostCarDisable : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "FinishLine"){
            this.gameObject.SetActive(false);
        }
    }
}