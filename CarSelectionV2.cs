using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CarSelectionV2 : MonoBehaviour
{
    [SerializeField] private int currentCarIndex = 0;
    [SerializeField] private int currentMapIndex = 0;
    [SerializeField] private mapInfo[] MapList;
    [SerializeField] private GameObject[] map3dStuff;
    public Transform[] Carlist = new Transform[2];
    [SerializeField] private bool inGameplay = false;
    public LoadingScreenV2 loadingScreen;
    public Slider slider;

    [SerializeField] private bool needStats = true;
    [Header("Stats:")]
    [SerializeField] private CarStats CS;
    public TextMeshProUGUI CarName;
    public TextMeshProUGUI CarType;
    public TextMeshProUGUI MapName;
    public MeshRenderer stageBG;
    public Material billboardMat;
    public Slider powerSlider;
    public Slider speedSlider;
    public Slider weightSlider;

    [Header("Values")]
    [SerializeField] private string name;
    [SerializeField] private string type;
    [SerializeField] private string Grade;

    [SerializeField] private float Power;
    [SerializeField] private float Speed;
    [SerializeField] private float Weight;

    
    private void FixedUpdate() {

        // value assignment

        CS = Carlist[currentCarIndex].GetComponent<CarStats>();
        if(CS != null){
            name = CS.CarName;
            type = CS.Type;
            Grade = CS.grade;
            Power = CS.power;
            Speed = CS.speed;
            Weight = CS.weight;
        }
        // value display
        if(needStats){
            CarName.text = CS.CarName;
            CarType.text = $"//{CS.Type} | {CS.grade}";
            powerSlider.value = CS.power;
            speedSlider.value = CS.speed;
            weightSlider.value = CS.weight;
        }

    }

    public void LoadLevel(int lvlIndex){
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.SetSceneIndex(lvlIndex);
    }


    private void Awake() {
        Carlist = new Transform[transform.childCount];
        Debug.Log(transform.childCount);
        for(int i = 0; i <= transform.childCount-1; i++) {
            Carlist[i] = transform.GetChild(i);
        }
    }
    private void Start() {
        if(PlayerPrefs.GetInt("MetricUnit",0) == 0){
            speedSlider.maxValue = 500f;
        }else{
            speedSlider.maxValue = 330f;
        }
        int SelectedCarID = PlayerPrefs.GetInt("SelectedCarID",0);
        currentMapIndex = PlayerPrefs.GetInt("SelectedMapID",0);
        if(inGameplay == true){
            Carlist[SelectedCarID].gameObject.SetActive(true);  
            currentCarIndex = SelectedCarID;
        }
    }
    public void Select() {
        PlayerPrefs.SetInt("SelectedCarID",currentCarIndex);
        //LoadLevel(1);
    }
    public void Modify(){
        PlayerPrefs.SetInt("SelectedCarID",currentCarIndex);
        LoadLevel(2);
    }
    public void Next () {
        if(currentCarIndex<transform.childCount-1){
            currentCarIndex += 1;
        }else{
            currentCarIndex = 0;
        }
        for(int i = 0; i < Carlist.Length; i++) {
            Carlist[i].gameObject.SetActive(false);
            Carlist[currentCarIndex].gameObject.SetActive(true);
        }
    }
    public void Previous () {
        if(currentCarIndex>0){
            currentCarIndex -= 1;
        }else{
            currentCarIndex = transform.childCount-1;
        }
        for(int i = 0; i < Carlist.Length; i++) {
            Carlist[i].gameObject.SetActive(false);
            Carlist[currentCarIndex].gameObject.SetActive(true);
        }
    }

    public void NextMap(){
        if(currentMapIndex < MapList.Length - 1){
            currentMapIndex ++;
        }else{
            currentMapIndex = 0;
        }
        refreshMapPreview();
    }

    public void PreMap(){
        if(currentMapIndex > 0){
            currentMapIndex --;
        }else{
            currentMapIndex = MapList.Length - 1;
        }
        refreshMapPreview();
    }

    public void LoadMap() {
        PlayerPrefs.SetInt("SelectedMapID",currentMapIndex);
        LoadLevel(MapList[currentMapIndex].sceneIndex);
    }

    public void refreshMapPreview(){
        var materialsCopy = stageBG.materials;
        materialsCopy[0] = MapList[currentMapIndex].preview;
        stageBG.materials = materialsCopy;
        MapName.text = MapList[currentMapIndex].M_Name;
        for(int i = 0; i < map3dStuff.Length; i++) {
            if(i == currentMapIndex){
                map3dStuff[i].SetActive(true);
            }else{
                map3dStuff[i].SetActive(false);
            }
        }
    }

    public void backKrow(){
        var materialsCopy = stageBG.materials;
        materialsCopy[0] = billboardMat;
        stageBG.materials = materialsCopy;
        for(int i = 0; i < map3dStuff.Length; i++) {
            if(i == map3dStuff.Length-1){
                map3dStuff[i].SetActive(true);
            }else{
                map3dStuff[i].SetActive(false);
            }
        }
    }
}
