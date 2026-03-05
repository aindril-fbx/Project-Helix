using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghost", menuName = "Ghost", order = 0)]
public class Ghost : ScriptableObject {
    public bool isRecord;
    public bool isReplay;
    public float recordFrequency;

    public List<float> timeStamp;
    public List<float> throttle;
    public List<float> steering;
    public List<bool> handBrake;
    public List<Vector3> position;
    public List<Quaternion> rotation;

    public void ResetData(){
        timeStamp.Clear();
        throttle.Clear();
        steering.Clear();
        handBrake.Clear();
        position.Clear();
        rotation.Clear();
    }
}