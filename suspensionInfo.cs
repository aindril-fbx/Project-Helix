using UnityEngine;

[CreateAssetMenu(fileName = "suspensionInfo", menuName = "suspensionInfo", order = 0)]
public class suspensionInfo : ScriptableObject {
    public float spring;
    public float damping;
    public bool driftSteering;

    public float speedAdder;
    public float powerAdder;
    public float weightAdder;
    
}