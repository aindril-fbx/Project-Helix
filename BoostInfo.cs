using UnityEngine;

[CreateAssetMenu(fileName = "BoostInfo", menuName = "BoostInfo", order = 0)]
public class BoostInfo : ScriptableObject {
    public AnimationCurve boostCurve;
    public float speedAdder;
    public float powerAdder;
    public float weightAdder;
}