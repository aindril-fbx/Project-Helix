using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Customization : MonoBehaviour {

    [SerializeField] private GameObject ExitButton;
    [SerializeField] private GameObject BackButton;
    [SerializeField] private TextMeshProUGUI moneyDisplay;
    [SerializeField] private TextMeshProUGUI moneyChangeDisplay;
    [SerializeField] private GameObject CustomizationUI;
    [SerializeField] private GameObject UpgradeUI;
    [SerializeField] private GameObject modeSelectUI;
    [SerializeField] private GameObject CarStatsUI;
    [SerializeField] private GameObject ScreenHeader;
    [SerializeField] private Slider hueSlider;
    [SerializeField] private Slider saturationSlider;
    [SerializeField] private Slider valueSlider;

    [SerializeField] private Animator camAnimator;
    [SerializeField] private Slider rotationSlider;
    [SerializeField] private GameObject touchPanel;

    [SerializeField] private Slider metallicSlider;
    [SerializeField] private Slider glossySlider;
    [SerializeField] private TMP_InputField HexCodeDisplay;

    [SerializeField] private float[] HSVvalues = new float[3];
    [SerializeField] private Color RGB;
    [SerializeField] private Image colorDisplay;

    Color color;
    [SerializeField] private string HexCode = "HexCode";

    [Header("Materials")]
    [SerializeField] private CarNameDisplay CD;
    [SerializeField] private CarStats cs;
    [SerializeField] private Material[] carMaterials;
    private Material[] wheelRims = new Material[4];
    private Material[] wheelSecondary = new Material[4];
    public int currentMaterial = 0;
    bool yes = false;

    [SerializeField] private bool Customizing_ = true;
    [SerializeField] private enum customizationState{
        Customize,
        None,
        Upgrade
    };
    [SerializeField] private customizationState CState = customizationState.None;

    [Header("Value Labels")]
    [SerializeField] private TextMeshProUGUI[] valueLabels;

    [Header("Upgrade Window")]
    [SerializeField] private int currentItemType = 0;
    [SerializeField] private TextMeshProUGUI upgradeLabel_Display;
    [SerializeField] private string[] upgradeLabels;
    [SerializeField] private GameObject[] upgradeWindow;
    [SerializeField] private Slider upgradeItemSlider;
    [SerializeField] private Transform partChangeEffect;
    [SerializeField] private Vector3 ONeffectSetPosition;
    [SerializeField] private Vector3 OFFeffectSetPosition;

    [Header("Buy Parts Screen")]
    [SerializeField] private GameObject buyPartScreen;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button cancelButton;

    [Header("Ride Height")]
    [SerializeField] private Slider frontWheelSlider;
    [SerializeField] private Slider rearWheelSlider;

    [Header("Audio")]
    [SerializeField] private AudioSource selectAudioSouce;
    [SerializeField] private AudioSource installAudioSource;

    [Header("Theme")]
    [SerializeField] private Button themeButton;
    [SerializeField] private Material bgMat;
    [SerializeField] private GameObject[] themeImage;
    bool theme = false; // false is dark and true is light

    private void Start() {
        OFFeffectSetPosition = partChangeEffect.position;
        modeSelectUI.transform.LeanMoveLocal(new Vector2(0f,0f),0.2f).setEaseInQuart().setOnComplete(OnComplete).delay = 0.1f;
        upgradeWindow[currentItemType].SetActive(true);
        upgradeItemSlider.maxValue = (float)upgradeWindow.Length - 0.1f;
        camAnimator.SetFloat("Blend",0.5f);
        rotationSlider.value = 90f;
        AddListenertoButtons();
        moneyDisplay.text = playerStats.instance.playerMoney_().ToString("n0");
        SetCustomizationState(0);

        bgMat.SetFloat("_Theme",0f);
        themeButton.onClick.AddListener(()=> {
            themeImage[0].SetActive(!themeImage[0].activeSelf);
            themeImage[1].SetActive(!themeImage[1].activeSelf);
            theme = !theme;
        });
    }
    private void FixedUpdate() {
        cs = CD.CS;
        for(int i = 0; i < cs.wheelMesh.Length; i++) {
            wheelRims[i] = cs.wheelMesh[i].materials[cs.Rims];
        }
        if(cs.hasWheelSecondary){
            for(int i = 0; i < cs.wheelMesh.Length; i++) {
                wheelSecondary[i] = cs.wheelMesh[i].materials[cs.RimSecondary];
            }
        }
        carMaterials[0] = cs.carBodyMesh.materials[cs.Primary];
        carMaterials[1] = cs.carBodyMesh.materials[cs.Secondary];
        carMaterials[2] = cs.wheelMesh[0].materials[cs.Rims];

        if(theme){
            bgMat.SetFloat("_Theme",Mathf.Lerp(bgMat.GetFloat("_Theme"),1f,0.1f));
        }else{
            bgMat.SetFloat("_Theme",Mathf.Lerp(bgMat.GetFloat("_Theme"),0f,0.1f));
        }
    }

    private void Update() {
        switch (CState){
            case customizationState.None:
                ExitButton.SetActive(true);
                BackButton.SetActive(false);
                camAnimator.SetFloat("Blend",Mathf.Lerp(camAnimator.GetFloat("Blend"),0.5f,0.1f));
                rotationSlider.value = Mathf.Lerp(rotationSlider.value,90f,0.1f);
                break;
            case customizationState.Customize:
                ExitButton.SetActive(false);
                BackButton.SetActive(true);
                camAnimator.SetFloat("Blend",Mathf.Lerp(camAnimator.GetFloat("Blend"),0f,0.1f));
                break;
            case customizationState.Upgrade:
                ExitButton.SetActive(false);
                BackButton.SetActive(true);
                camAnimator.SetFloat("Blend",Mathf.Lerp(camAnimator.GetFloat("Blend"),1f,0.1f));
                rotationSlider.value = Mathf.Lerp(rotationSlider.value,137f,0.2f);
                break;
        }
    }

    public void ChangeValue() {
        cs.updateMaterials = true;
        if(yes){

            HSVvalues[0] = hueSlider.value;
            HSVvalues[1] = saturationSlider.value;
            HSVvalues[2] = valueSlider.value;

            RGB = Color.HSVToRGB(HSVvalues[0], HSVvalues[1], HSVvalues[2]);
            HexCode = ColorUtility.ToHtmlStringRGB(RGB);
            HexCodeDisplay.text = HexCode;
            colorDisplay.color = RGB;

            carMaterials[currentMaterial].SetColor("_BaseColor", RGB);
            carMaterials[currentMaterial].SetFloat("_Smoothness", glossySlider.value);
            carMaterials[currentMaterial].SetFloat("_Metallic", metallicSlider.value);

            if(cs.hasWheelSecondary){
                if(currentMaterial == 1){
                    foreach(Material m in wheelSecondary) {
                        m.SetColor("_BaseColor", RGB);
                        m.SetFloat("_Smoothness", glossySlider.value);
                        m.SetFloat("_Metallic", metallicSlider.value);
                    }
                }
            }

            if(currentMaterial == 2){
                foreach(Material m in wheelRims) {
                    m.SetColor("_BaseColor", RGB);
                    m.SetFloat("_Smoothness", glossySlider.value);
                    m.SetFloat("_Metallic", metallicSlider.value);
                }
            }

            if(cs.spoilerMesh != null && currentMaterial == 0){
                cs.spoilerMesh.materials[0].SetColor("_BaseColor", RGB);
                cs.spoilerMesh.materials[0].SetFloat("_Smoothness", glossySlider.value);
                cs.spoilerMesh.materials[0].SetFloat("_Metallic", metallicSlider.value);
            }else if(currentMaterial == 1 && cs.Primary == cs.Secondary && cs.spoilerMesh != null){
                cs.spoilerMesh.materials[0].SetColor("_BaseColor", RGB);
                cs.spoilerMesh.materials[0].SetFloat("_Smoothness", glossySlider.value);
                cs.spoilerMesh.materials[0].SetFloat("_Metallic", metallicSlider.value);
            }
        }


    }

    public void ChangeHexValue(){
        Color hexColor;
        float H,S,V;
        if(ColorUtility.TryParseHtmlString("#" + HexCodeDisplay.text, out hexColor)){
            Color.RGBToHSV(hexColor, out H,out S,out V);
            hueSlider.value = H;
            saturationSlider.value = S;
            valueSlider.value = V;
        }

    }

    public void changeRideHeight(){
        if(yes){
            cs.susHeightsAdder[0] = frontWheelSlider.value;
            cs.susHeightsAdder[1] = frontWheelSlider.value;
            cs.susHeightsAdder[2] = rearWheelSlider.value;
            cs.susHeightsAdder[3] = rearWheelSlider.value;
        }
    }

    public void ChangeMaterialIndex(int x) {
        currentMaterial = x;
    }

    public void OnComplete() {
        yes = false;
        RGB = carMaterials[currentMaterial].GetColor("_BaseColor");
        float H,S,V;
        Color.RGBToHSV(RGB, out H, out S, out V);

        hueSlider.value = H;
        saturationSlider.value = S;
        valueSlider.value = V;
        metallicSlider.value = carMaterials[currentMaterial].GetFloat("_Metallic");
        glossySlider.value = carMaterials[currentMaterial].GetFloat("_Smoothness");

        colorDisplay.color = RGB;
        HexCode = ColorUtility.ToHtmlStringRGB(RGB);
        HexCodeDisplay.text = HexCode;

        UpdateValueLabels();
        

        for(int i = 0; i < cs.ownedBoost.Length; i++) {
            if(cs.ownedBoost[i] == 1){
                boostButtons[i].GetComponent<upgradeItemState>().SetStateToOwned();
            }
            boostButtons[cs.boost_type].GetComponent<upgradeItemState>().SetStateToInstalled();
        }
        for(int i = 0; i < cs.ownedTyreCompound.Length; i++) {
            if(cs.ownedTyreCompound[i] == 1){
                tyreButtons[i].GetComponent<upgradeItemState>().SetStateToOwned();
            }
            tyreButtons[cs.tyreCompound].GetComponent<upgradeItemState>().SetStateToInstalled();
        }
        for(int i = 0; i < cs.ownedSuspension.Length; i++) {
            if(cs.ownedSuspension[i] == 1){
                susButtons[i].GetComponent<upgradeItemState>().SetStateToOwned();
            }
            susButtons[cs.suspension].GetComponent<upgradeItemState>().SetStateToInstalled();
        }
        for(int i = 0; i < cs.ownedWeightReduction.Length; i++) {
            if(cs.ownedWeightReduction[i] == 1){
                weightButtons[i].GetComponent<upgradeItemState>().SetStateToOwned();
            }
            weightButtons[cs.weightReduction].GetComponent<upgradeItemState>().SetStateToInstalled();
        }
        frontWheelSlider.value = cs.susHeightsAdder[0];
        rearWheelSlider.value = cs.susHeightsAdder[2];
        yes = true;
    }

    public void Save() {
        cs.SaveDataPls();
    }

    void UpdateValueLabels(){

        if(PlayerPrefs.GetInt("MetricUnit") == 0){
            valueLabels[6].text = cs.calculateSpeed()*1.6f + " KMPH";
        }else{
            valueLabels[6].text = cs.calculateSpeed() + " MPH";
        }
        valueLabels[0].text = cs.CarName;
        valueLabels[1].text = cs.grade;
        valueLabels[2].text = cs.calculatePower() + "";
        valueLabels[3].text = cs.boost + " PS";
        valueLabels[4].text = cs.drivetype_;
        valueLabels[5].text = cs.calculateWeight()*2.2f + " KG";
        valueLabels[7].text = tyreButtons[cs.tyreCompound].name;

    }

    int power;
    int boost;
    int weight;
    int suspension;
    int tyreCompound;

    void showcaseValueLabels(){
        valueLabels[2].text = cs.calculatePower() + "";
        valueLabels[3].text = cs.boost + " PS";
        valueLabels[4].text = cs.drivetype_;
        valueLabels[5].text = cs.calculateWeight()*2.2f + " KG";
        valueLabels[7].text = tyreButtons[cs.tyreCompound].name;
    }

    float calculatePower_Showcase(){ 
        return 0f;
    }

    float calculateSpeed_Showcase(){
        return 0f;
    }

    float calculateWeight_Showcase(){
        return 0f;
    }

    public void SetCustomizationState(int x){
        if(x == 0){
            CState = customizationState.None;
            touchPanel.SetActive(false);
            Debug.Log("Initialize None State");
            modeSelectUI.transform.LeanMoveLocal(new Vector2(0f,0f),0.2f).setEaseInQuart().delay = 0.2f;
            UpgradeUI.transform.LeanMoveLocal(new Vector2(0f,-200f),0.2f).setEaseInQuart().setOnComplete(OnComplete);
            CustomizationUI.transform.LeanMoveLocal(new Vector2(0f,-200f),0.2f).setEaseInQuart().setOnComplete(OnComplete);
            CarStatsUI.transform.LeanMoveLocal(new Vector2(-2300f,22.718f),0.2f).setEaseInQuart();

        }else if(x == 1){
            CState = customizationState.Customize;
            touchPanel.SetActive(true);
            Debug.Log("Initialize Customize");
            ScreenHeader.transform.LeanMoveLocal(new Vector2(0f,0f),0.2f).setEaseInQuart();
            modeSelectUI.transform.LeanMoveLocal(new Vector2(0f,-200f),0.2f).setEaseInQuart().setOnComplete(OnComplete);
            UpgradeUI.transform.LeanMoveLocal(new Vector2(0f,-200f),0.2f).setEaseInQuart().setOnComplete(OnComplete);
            CustomizationUI.transform.LeanMoveLocal(new Vector2(0f,0f),0.2f).setEaseInQuart().delay = 0.2f;
            CarStatsUI.transform.LeanMoveLocal(new Vector2(-2300f,22.718f),0.2f).setEaseInQuart();
        }else if(x == 2){
            CState = customizationState.Upgrade;
            touchPanel.SetActive(false);
            Debug.Log("Initialize Upgrade");
            ScreenHeader.transform.LeanMoveLocal(new Vector2(0f,70.0829f),0.2f).setEaseInQuart();
            modeSelectUI.transform.LeanMoveLocal(new Vector2(0f,-200f),0.2f).setEaseInQuart().setOnComplete(OnComplete);
            CustomizationUI.transform.LeanMoveLocal(new Vector2(0f,-200f),0.2f).setEaseInQuart().setOnComplete(OnComplete);
            UpgradeUI.transform.LeanMoveLocal(new Vector2(0f,0f),0.2f).setEaseInQuart().delay = 0.2f;
            CarStatsUI.transform.LeanMoveLocal(new Vector2(-652f,22.718f),0.2f).setEaseInQuart();
        }
    }


    public void ChangeUpgradeWindow(){
        for(int i = 0; i < upgradeWindow.Length; i++) {
            if((int)upgradeItemSlider.value == i){
                upgradeWindow[i].SetActive(true);
                upgradeLabel_Display.text = upgradeLabels[i];
            }else{
                upgradeWindow[i].SetActive(false);
            }
        }
    }

    [SerializeField] private Button[] tyreButtons;
    [SerializeField] private Button[] boostButtons;
    [SerializeField] private Button[] susButtons;
    [SerializeField] private Button[] weightButtons;
    [SerializeField] private Color notSelectedColor;
    [SerializeField] private Color selectedColor;
    void AddListenertoButtons(){
        foreach(Button b in tyreButtons) {
            b.onClick.AddListener(()=>{
                for(int i = 0; i < tyreButtons.Length; i++) {
                    if(tyreButtons[i] == b){
                        upgradeItemState UIS = tyreButtons[i].GetComponent<upgradeItemState>();
                        Debug.Log("Tyre of Type: "+i);
                        tyreButtons[i].gameObject.GetComponent<Shadow>().effectColor = selectedColor;
                        if(UIS.selected){
                            if(UIS.itemState == upgradeItemState.ItemState.NotOwned){
                                BuyPart(0,i);
                            }else{
                            //UIS.changeItemState();
                                if(UIS.itemState == upgradeItemState.ItemState.Owned){
                                    changeInstalledPart(0,i);
                                }
                            }
                        }
                        selectAudioSouce.Play();
                        UIS.selected = true;
                    }else{
                        tyreButtons[i].GetComponent<upgradeItemState>().selected = false;
                        tyreButtons[i].gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
                    }
                }
            });
        }

        foreach(Button b in boostButtons) {
            b.onClick.AddListener(()=>{
                for(int i = 0; i < boostButtons.Length; i++) {
                    if(boostButtons[i] == b){
                        upgradeItemState UIS = boostButtons[i].GetComponent<upgradeItemState>();
                        Debug.Log("Boost of Type: "+i);
                        boostButtons[i].gameObject.GetComponent<Shadow>().effectColor = selectedColor;
                        if(UIS.selected){
                            if(UIS.itemState == upgradeItemState.ItemState.NotOwned){
                                BuyPart(1,i);
                            }else{
                            //UIS.changeItemState();
                                if(UIS.itemState == upgradeItemState.ItemState.Owned){
                                    changeInstalledPart(1,i);
                                }
                            }
                        }
                        selectAudioSouce.Play();
                        UIS.selected = true;
                    }else{
                        boostButtons[i].GetComponent<upgradeItemState>().selected = false;
                        boostButtons[i].gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
                    }
                }
            });
        }

        foreach(Button b in susButtons) {
            b.onClick.AddListener(()=>{
                for(int i = 0; i < susButtons.Length; i++) {
                    if(susButtons[i] == b){
                        upgradeItemState UIS = susButtons[i].GetComponent<upgradeItemState>();
                        Debug.Log("Suspension of Type: "+i);
                        susButtons[i].gameObject.GetComponent<Shadow>().effectColor = selectedColor;
                        if(UIS.selected){
                            if(UIS.itemState == upgradeItemState.ItemState.NotOwned){
                                BuyPart(2,i);
                            }else{
                            //UIS.changeItemState();
                                if(UIS.itemState == upgradeItemState.ItemState.Owned){
                                    changeInstalledPart(2,i);
                                }
                            }
                        }
                        selectAudioSouce.Play();
                        UIS.selected = true;
                    }else{
                        susButtons[i].GetComponent<upgradeItemState>().selected = false;
                        susButtons[i].gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
                    }
                }
            });
        }

        foreach(Button b in weightButtons) {
            b.onClick.AddListener(()=>{
                for(int i = 0; i < weightButtons.Length; i++) {
                    if(weightButtons[i] == b){
                        upgradeItemState UIS = weightButtons[i].GetComponent<upgradeItemState>();
                        Debug.Log("weight of Type: "+i);
                        weightButtons[i].gameObject.GetComponent<Shadow>().effectColor = selectedColor;
                        if(UIS.selected){
                            if(UIS.itemState == upgradeItemState.ItemState.NotOwned){
                                BuyPart(3,i);
                            }else{
                            //UIS.changeItemState();
                                if(UIS.itemState == upgradeItemState.ItemState.Owned){
                                    changeInstalledPart(3,i);
                                }
                            }
                        }
                        selectAudioSouce.Play();
                        UIS.selected = true;
                    }else{
                        weightButtons[i].GetComponent<upgradeItemState>().selected = false;
                        weightButtons[i].gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
                    }
                }
            });
        }
    }
    
    public void Deselect(){
        foreach(Button b in susButtons){
            b.GetComponent<upgradeItemState>().selected = false;
            b.gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
        }
        foreach(Button b in tyreButtons){
            b.GetComponent<upgradeItemState>().selected = false;
            b.gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
        }
        foreach(Button b in boostButtons){
            b.GetComponent<upgradeItemState>().selected = false;
            b.gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
        }
        foreach(Button b in weightButtons){
            b.GetComponent<upgradeItemState>().selected = false;
            b.gameObject.GetComponent<Shadow>().effectColor = notSelectedColor;
        }
    }

    void BuyPart(int partOfType, int partId) {
        bool transaction = false;
        buyPartScreen.SetActive(true);
        buyButton.onClick.AddListener(()=>{
            if(!transaction) {
                Debug.Log("Brought Item of Type: " + partOfType + " of ID: " + partId);
                buyPartScreen.SetActive(false);
                if(partOfType == 0) {
                    tyreButtons[partId].GetComponent<upgradeItemState>().changeItemState();
                    playerStats.instance.transaction(-tyreButtons[partId].GetComponent<upgradeItemState>().getPrice());
                    cs.ownedTyreCompound[partId] = 1;
                    Save();
                    moneyChangeDisplay.text = (-tyreButtons[partId].GetComponent<upgradeItemState>().getPrice()).ToString("n0");
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().LeanAlpha(0f,0.5f).setEaseInQuart().delay = 1f;
                }else if(partOfType == 1){
                    boostButtons[partId].GetComponent<upgradeItemState>().changeItemState();
                    playerStats.instance.transaction(-boostButtons[partId].GetComponent<upgradeItemState>().getPrice());
                    cs.ownedBoost[partId] = 1;
                    Save();
                    moneyChangeDisplay.text = (-boostButtons[partId].GetComponent<upgradeItemState>().getPrice()).ToString("n0");
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().LeanAlpha(0f,0.5f).setEaseInQuart().delay = 1f;
                }else if(partOfType == 2){
                    susButtons[partId].GetComponent<upgradeItemState>().changeItemState();
                    playerStats.instance.transaction(-susButtons[partId].GetComponent<upgradeItemState>().getPrice());
                    cs.ownedSuspension[partId] = 1;
                    Save();
                    moneyChangeDisplay.text = (-susButtons[partId].GetComponent<upgradeItemState>().getPrice()).ToString("n0");
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().LeanAlpha(0f,0.5f).setEaseInQuart().delay = 1f;
                }else if(partOfType == 3){
                    weightButtons[partId].GetComponent<upgradeItemState>().changeItemState();
                    playerStats.instance.transaction(-susButtons[partId].GetComponent<upgradeItemState>().getPrice());
                    cs.ownedWeightReduction[partId] = 1;
                    Save();
                    moneyChangeDisplay.text = (-susButtons[partId].GetComponent<upgradeItemState>().getPrice()).ToString("n0");
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                    moneyChangeDisplay.transform.parent.gameObject.GetComponent<CanvasGroup>().LeanAlpha(0f,0.5f).setEaseInQuart().delay = 1f;
                }
                moneyDisplay.text = playerStats.instance.playerMoney_().ToString("n0");
                transaction = true;
            }
        });
        cancelButton.onClick.AddListener(()=>{
            if(!transaction) {
                Debug.Log("Canceled buy process");
                buyPartScreen.SetActive(false);
                transaction = true;
            }

        });
    }

    void changeInstalledPart(int partOfType, int partId) {
        installAudioSource.Play();
        LeanTween.cancel(partChangeEffect.gameObject);
        effectComplete();
        partChangeEffect.LeanMoveLocal(ONeffectSetPosition,2f).setEaseOutQuart().setOnComplete(effectComplete);
        if(partOfType == 0) {
            for(int i = 0; i < tyreButtons.Length; i++) {
                upgradeItemState UIS = tyreButtons[i].GetComponent<upgradeItemState>();
                if(i == partId){
                    if(UIS.itemState == upgradeItemState.ItemState.Owned){
                        cs.tyreCompound = partId;
                        cs.SaveDataPls();
                        UIS.SetStateToInstalled();
                    }
                }else{
                    if(UIS.itemState == upgradeItemState.ItemState.Installed){
                        UIS.SetStateToOwned();
                    }
                }
            }
        }else if(partOfType == 1){
            for(int i = 0; i < boostButtons.Length; i++) {
                upgradeItemState UIS = boostButtons[i].GetComponent<upgradeItemState>();
                if(i == partId){
                    if(UIS.itemState == upgradeItemState.ItemState.Owned){
                        cs.boost_type = partId;
                        cs.SaveDataPls();
                        UIS.SetStateToInstalled();
                    }
                }else{
                    if(UIS.itemState == upgradeItemState.ItemState.Installed){
                        UIS.SetStateToOwned();
                    }
                }
            }
        }else if(partOfType == 2){
            for(int i = 0; i < susButtons.Length; i++) {
                upgradeItemState UIS = susButtons[i].GetComponent<upgradeItemState>();
                if(i == partId){
                    if(UIS.itemState == upgradeItemState.ItemState.Owned){
                        cs.suspension = partId;
                        cs.SaveDataPls();
                        UIS.SetStateToInstalled();
                    }
                }else{
                    if(UIS.itemState == upgradeItemState.ItemState.Installed){
                        UIS.SetStateToOwned();
                    }
                }
            }
        }else if(partOfType == 3){
            for(int i = 0; i < weightButtons.Length; i++) {
                upgradeItemState UIS = weightButtons[i].GetComponent<upgradeItemState>();
                if(i == partId){
                    if(UIS.itemState == upgradeItemState.ItemState.Owned){
                        cs.weightReduction = partId;
                        cs.SaveDataPls();
                        UIS.SetStateToInstalled();
                    }
                }else{
                    if(UIS.itemState == upgradeItemState.ItemState.Installed){
                        UIS.SetStateToOwned();
                    }
                }
            }
        }
        UpdateValueLabels();
        Deselect();
    }

    void effectComplete(){
        partChangeEffect.position = OFFeffectSetPosition;
    }
}