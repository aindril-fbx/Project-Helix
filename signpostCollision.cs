using UnityEngine;

public class signpostCollision : MonoBehaviour {
    [SerializeField] private Rigidbody rb;
    [SerializeField] float threshold = 2f;

    private void OnCollisionEnter(Collision other) {
        if(other.relativeVelocity.magnitude > threshold){
            rb.isKinematic = false;
        }
    }
}