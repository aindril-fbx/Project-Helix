using UnityEngine;

[CreateAssetMenu(fileName = "inputType", menuName = "TrackX/inputType", order = 0)]
public class inputType : ScriptableObject {
    public int inputType_; // 0 = button, 1 = tilt, 2 = sliders;
}