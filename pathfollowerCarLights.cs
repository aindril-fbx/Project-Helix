using UnityEngine;

public class pathfollowerCarLights : MonoBehaviour
{
    [Header("Tail Lights")]
    [SerializeField] private bool HasLights = true;
    [SerializeField] private MeshRenderer TailLights;
    [SerializeField] private Material[] TailLight_M;

    [SerializeField] private CarPathFollower CPF;
    [SerializeField] private float bias = 1f;

    private void Start() {
        CPF = GetComponent<CarPathFollower>();
    }

    private void FixedUpdate() {
        if (HasLights) {
            if (CPF.currentSpud > bias * CPF.tempSpeedLimit) {
                TailLights.material = TailLight_M[1];
            } else {
                TailLights.material = TailLight_M[0];
            }
        }
    }
}