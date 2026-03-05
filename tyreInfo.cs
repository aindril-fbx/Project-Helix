using UnityEngine;

[CreateAssetMenu(fileName = "tyreInfo", menuName = "tyreInfo", order = 0)]
public class tyreInfo : ScriptableObject {

    public string tyreCompoundName;
    [Header("Front Wheels Forward Friction")]
    public float FF_extremumSlip;
    public float FF_extremumValue;
    public float FF_asymptoteSlip;
    public float FF_asymptoteValue;
    public float FF_stiffness;

    [Header("Front Wheels Sideways Friction")]
    public float FS_extremumSlip;
    public float FS_extremumValue;
    public float FS_asymptoteSlip;
    public float FS_asymptoteValue;
    public float FS_stiffness;

    [Header("Back Wheels Forward Friction")]
    public float BF_extremumSlip;
    public float BF_extremumValue;
    public float BF_asymptoteSlip;
    public float BF_asymptoteValue;
    public float BF_stiffness;

    [Header("Back Wheels Sideways Friction")]
    public float BS_extremumSlip;
    public float BS_extremumValue;
    public float BS_asymptoteSlip;
    public float BS_asymptoteValue;
    public float BS_stiffness;
    [Space]
    public float speedAdder;
    public float powerAdder;
    public float weightAdder;
    
}