using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStats : MonoBehaviour
{
    [SerializeField] private WheelCollider[] wheelColliders;
    
    public string CarName = "lol";
    public string Type = "Race";
    public string grade = "A";
    public float power = 1000f;
    public float speed = 200f;
    public float boost = 10f;
    public float weight = 100f;
    public string drivetype_ = "RWD";
    public Vector3 fixedCamPos;

    public bool metric = false;
    
    public CarController cc;
    public Rigidbody rb;
    [SerializeField] private Nos nos;
    [SerializeField] private InputManager IM;
    

    [Header("Material Stuff: ")]
    public MeshRenderer carBodyMesh;
    public MeshRenderer spoilerMesh;
    public MeshRenderer[] wheelMesh;
    public int Primary;
    public int Secondary;
    public int Rims;
    public bool hasWheelSecondary = false;
    public int RimSecondary = 2;
    public string[] Colors = new string[3];
    public float[] MetallicValues = new float[3];
    public float[] GlossyValues = new float[3];

    Color P_RGB;
    Color S_RGB;
    Color R_RGB;

    bool yes = false;
    public bool updateMaterials = false;

    [Header("Default Upgrade Values")]
    [SerializeField] private int Def_Boost;
    [SerializeField] private int Def_tyreType;
    [SerializeField] private int Def_tune;
    [SerializeField] private int Def_wieghtReduction;
    [SerializeField] private int Def_suspension;

    [Header("Upgrade Values")]
    public int boost_type;
    public int tyreCompound;
    public int tune;
    public int weightReduction;
    public int suspension;
    
    [Header("Upgrade Items")]
    [SerializeField] private BoostInfo[] boostCurves;
    [SerializeField] private tyreInfo[] tyreType;
    [SerializeField] private int[] gearNums;
    [SerializeField] private wieghtInfo[] weightReduce;
    [SerializeField] private suspensionInfo[] suspensionType;

    [Header("Owned Items")]
    public int[] ownedBoost = new int[5];
    public int[] ownedTyreCompound = new int[4];
    public int[] ownedTune = new int[4];
    public int[] ownedWeightReduction = new int[3];
    public int[] ownedSuspension = new int[4];

    [Header("Suspension Heights")]
    [SerializeField] private float[] susHeights = new float[4];
    public float[] susHeightsAdder = new float[4];
     
    public WheelCollider[] wheelColliderReturn(){
        return wheelColliders;
    }
    //tempValues
    float tempWieght;
    float tempSpeed;
    float tempPower;

    [Header("BIAS")]
    [Range(1f,5f)][SerializeField] private float speedBias = 0.5f;
    [Range(1f,5f)][SerializeField] private float powerBias = 1f;

    #region 
    public BoostInfo[] getBoostInfos(){
        return boostCurves;
    }

    public tyreInfo[] getTyreInfos(){
        return tyreType;
    }
    #endregion

    private void Start() {
        cc = GetComponent<CarController>();
        IM = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        nos = GetComponent<Nos>();
        tempPower = power;
        //cc.maxSpeed = speed;
        tempSpeed = speed;
        for(int i = 0; i < wheelColliders.Length; i++) {
            susHeights[i] = wheelColliders[i].suspensionDistance;
        }
        //rb.mass = weight;
        tempWieght = weight;
        boost = Mathf.Round(100/(cc.BoostFactor + 1));
        if(cc.AWD){
            drivetype_ = "AWD";
        }else{
            drivetype_ = "RWD";
        }

        LoadData();
        Debug.Log(ColorUtility.TryParseHtmlString("#" + Colors[0], out P_RGB));
        ColorUtility.TryParseHtmlString("#" + Colors[1], out S_RGB);
        ColorUtility.TryParseHtmlString("#" + Colors[2], out R_RGB);

        if(yes){
            carBodyMesh.materials[Primary].SetColor("_BaseColor", P_RGB);
            carBodyMesh.materials[Secondary].SetColor("_BaseColor", S_RGB);
            foreach(MeshRenderer MR in wheelMesh) {
                if(hasWheelSecondary) MR.materials[RimSecondary].SetColor("_BaseColor", S_RGB);
                MR.materials[Rims].SetColor("_BaseColor", R_RGB);
            }

            carBodyMesh.materials[Primary].SetFloat("_Smoothness", GlossyValues[0]);
            carBodyMesh.materials[Secondary].SetFloat("_Smoothness", GlossyValues[1]);
            foreach(MeshRenderer MR in wheelMesh) {
                if(hasWheelSecondary) MR.materials[RimSecondary].SetFloat("_Smoothness", GlossyValues[1]);
                MR.materials[Rims].SetFloat("_Smoothness", GlossyValues[2]);
            }

            carBodyMesh.materials[Primary].SetFloat("_Metallic", MetallicValues[0]);
            carBodyMesh.materials[Secondary].SetFloat("_Metallic", MetallicValues[1]);
            foreach(MeshRenderer MR in wheelMesh) {
                if(hasWheelSecondary) MR.materials[RimSecondary].SetFloat("_Metallic", MetallicValues[1]);
                MR.materials[Rims].SetFloat("_Metallic", MetallicValues[2]);
            }
            if(spoilerMesh != null){
                spoilerMesh.materials[0].SetColor("_BaseColor", P_RGB);
                spoilerMesh.materials[0].SetFloat("_Smoothness", GlossyValues[0]);
                spoilerMesh.materials[0].SetFloat("_Metallic", MetallicValues[0]);
            }

            for(int i = 0; i < wheelColliders.Length; i++) {
                wheelColliders[i].suspensionDistance = Mathf.Clamp(susHeights[i] + susHeightsAdder[i],0.02f,2f);
            }
        }
        setValues();
        
    }
    [SerializeField] private float springMulti = 1f;
    public void setValues(){
        
        //Apply WheelCollider values
        for(int i = 0; i < wheelColliders.Length; i++) {
            WheelFrictionCurve wfcF = wheelColliders[i].forwardFriction;
            WheelFrictionCurve wfcS = wheelColliders[i].sidewaysFriction;
            if(i <= 1){
                wfcF.extremumSlip = tyreType[tyreCompound].FF_extremumSlip;
                wfcF.extremumValue = tyreType[tyreCompound].FF_extremumValue;
                wfcF.asymptoteSlip = tyreType[tyreCompound].FF_asymptoteSlip;
                wfcF.asymptoteValue = tyreType[tyreCompound].FF_asymptoteValue;
                wfcF.stiffness = tyreType[tyreCompound].FF_stiffness;
                wheelColliders[i].forwardFriction = wfcF;
                wfcS.extremumSlip = tyreType[tyreCompound].FS_extremumSlip;
                wfcS.extremumValue = tyreType[tyreCompound].FS_extremumValue;
                wfcS.asymptoteSlip = tyreType[tyreCompound].FS_asymptoteSlip;
                wfcS.asymptoteValue = tyreType[tyreCompound].FS_asymptoteValue;
                wfcS.stiffness = tyreType[tyreCompound].FS_stiffness;
                wheelColliders[i].sidewaysFriction = wfcS;
            }else{
                wfcF.extremumSlip = tyreType[tyreCompound].BF_extremumSlip;
                wfcF.extremumValue = tyreType[tyreCompound].BF_extremumValue;
                wfcF.asymptoteSlip = tyreType[tyreCompound].BF_asymptoteSlip;
                wfcF.asymptoteValue = tyreType[tyreCompound].BF_asymptoteValue;
                wfcF.stiffness = tyreType[tyreCompound].BF_stiffness;
                wheelColliders[i].forwardFriction = wfcF;
                wfcS.extremumSlip = tyreType[tyreCompound].BS_extremumSlip;
                wfcS.extremumValue = tyreType[tyreCompound].BS_extremumValue;
                wfcS.asymptoteSlip = tyreType[tyreCompound].BS_asymptoteSlip;
                wfcS.asymptoteValue = tyreType[tyreCompound].BS_asymptoteValue;
                wfcS.stiffness = tyreType[tyreCompound].BS_stiffness;
                wheelColliders[i].sidewaysFriction = wfcS;
            }
        }
        

        //Apply Boost Values
        cc.boostCurve = boostCurves[boost_type].boostCurve;

        //Apply WheelCollider suspension valeus
        foreach(WheelCollider w in wheelColliders) {
            JointSpring SS = w.suspensionSpring;
            SS.spring = suspensionType[suspension].spring * springMulti;
            SS.damper = suspensionType[suspension].damping * springMulti;
            w.suspensionSpring = SS;
        }
        IM.useDriftSteering = suspensionType[suspension].driftSteering;

        rb.mass = calculateWeight(); 
        cc.motorForcelol = calculatePower();
        cc.maxSpeed = calculateSpeed();
        nos.setMotorForce(calculatePower());
    }

    public float calculateWeight(){
        return tempWieght + suspensionType[suspension].weightAdder 
                          + boostCurves[boost_type].weightAdder 
                          + tyreType[tyreCompound].weightAdder 
                          + weightReduce[weightReduction].weightAdder;
    }

    public float calculateSpeed(){
        return tempSpeed + (suspensionType[suspension].speedAdder
                         + boostCurves[boost_type].speedAdder
                         + tyreType[tyreCompound].speedAdder
                         + weightReduce[weightReduction].speedAdder) * speedBias;
    }

    public float calculatePower(){
        return tempPower + (suspensionType[suspension].powerAdder
                         + boostCurves[boost_type].powerAdder
                         + tyreType[tyreCompound].powerAdder
                         + weightReduce[weightReduction].powerAdder) * powerBias;
    }

    private void FixedUpdate() {
        if(updateMaterials){
            Colors[0] = ColorUtility.ToHtmlStringRGB(carBodyMesh.materials[Primary].GetColor("_BaseColor"));
            Colors[1] = ColorUtility.ToHtmlStringRGB(carBodyMesh.materials[Secondary].GetColor("_BaseColor"));
            Colors[2] = ColorUtility.ToHtmlStringRGB(wheelMesh[0].materials[Rims].GetColor("_BaseColor"));

            MetallicValues[0] = carBodyMesh.materials[Primary].GetFloat("_Metallic");
            MetallicValues[1] = carBodyMesh.materials[Secondary].GetFloat("_Metallic");
            MetallicValues[2] = wheelMesh[0].materials[Rims].GetFloat("_Metallic");

            GlossyValues[0] = carBodyMesh.materials[Primary].GetFloat("_Smoothness");
            GlossyValues[1] = carBodyMesh.materials[Secondary].GetFloat("_Smoothness");
            GlossyValues[2] = wheelMesh[0].materials[Rims].GetFloat("_Smoothness");
            
            for(int i = 0; i < wheelColliders.Length; i++) {
                wheelColliders[i].suspensionDistance = susHeights[i] + susHeightsAdder[i];
            }
        }else{
            return;
        }

    }

    public void SaveDataPls(){
        SaveSystem.SaveData(this, CarName.Replace(" ", string.Empty));
        Debug.Log("Data Saved!");
    }

    public void LoadData() {
        CarData data = SaveSystem.LoadCar(CarName.Replace(" ", string.Empty));

        if(data != null){
            Colors[0] = data.Colors_[0];
            Colors[1] = data.Colors_[1];
            Colors[2] = data.Colors_[2];
        
            MetallicValues[0] = data.MetallicValues_[0];
            MetallicValues[1] = data.MetallicValues_[1];
            MetallicValues[2] = data.MetallicValues_[2];

            GlossyValues[0] = data.GlossyValues_[0];
            GlossyValues[1] = data.GlossyValues_[1];
            GlossyValues[2] = data.GlossyValues_[2];

            if(data.boost_type != null){
                boost_type = data.boost_type;
            }else{
                boost_type = Def_Boost;
            }
            if(data.tyreCompound != null){
                Debug.Log("Not null");
                tyreCompound = data.tyreCompound;
            }else{
                Debug.Log("Null");
                tyreCompound = Def_tyreType;
            }
            if(data.tune != null){
                tune = data.tune;
            }else{
                tune = Def_tune;
            }
            if(data.weightReduction != null){
                weightReduction = data.weightReduction;
            }else{
                weightReduction = Def_wieghtReduction;
            }
            if(data.suspension != null){
                suspension = data.suspension;
            }else{
                suspension = Def_suspension;
            }

            if(data.ownedBoost != null){
                ownedBoost = data.ownedBoost;
            }
            if(data.ownedTyreCompound != null){
                ownedTyreCompound = data.ownedTyreCompound;
            }
            if(data.ownedTune != null){
                ownedTune = data.ownedTune;
            }
            if(data.ownedWeightReduction != null){
                ownedWeightReduction = data.ownedWeightReduction;
            }
            if(data.ownedSuspension != null){
                ownedSuspension = data.ownedSuspension;
            }
            if(data.susAdders != null){
                Debug.Log("Loaded susAdders");
                susHeightsAdder = data.susAdders;
            }
            Debug.Log("Data Loaded!");
            yes = true;
        }else{
            boost_type = Def_Boost;
            tyreCompound = Def_tyreType;
            tune = Def_tune;
            weightReduction = Def_wieghtReduction;
            suspension = Def_suspension;
        }

        ownedBoost[Def_Boost] = 1;
        ownedSuspension[Def_suspension] = 1;
        ownedTune[Def_tune] = 1;
        ownedTyreCompound[Def_tyreType] = 1;
        ownedWeightReduction[Def_wieghtReduction] = 1;
    }

}
