using UnityEngine;

public class waypointSetter : MonoBehaviour
{
    [SerializeField] private MiniMapPathVisualizer pathVisualizer; // Reference to the path visualizer script
    public Transform targetTransform; // Reference to the target transform

    private void Start() {
        pathVisualizer = GetComponentInChildren<MiniMapPathVisualizer>();
    }

    void Update(){
        if(targetTransform != null){
            pathVisualizer.targetTransform = targetTransform; // Set the target transform in the path visualizer
        }
        targetTransform = null; // Reset targetTransform to null after assigning it to the path visualizer
    }
}