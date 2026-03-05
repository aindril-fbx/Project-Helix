using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CarSelection : MonoBehaviour {
    
    [SerializeField] private int currentCarIndex = 0;
    [SerializeField] private Transform[] Carlist = new Transform[2];
    [SerializeField] private bool inGameplay = false;

    private void Awake() {
        Carlist = new Transform[transform.childCount];
        Debug.Log(transform.childCount);
        for(int i = 0; i <= transform.childCount; i++) {
            Carlist[i] = transform.GetChild(i);
        }
    }
    private void Start() {
        int SelectedCarID = PlayerPrefs.GetInt("SelectedCarID");
        if(inGameplay == true){
            Carlist[SelectedCarID].gameObject.SetActive(true);  
            currentCarIndex = SelectedCarID;
        }
    }
    public void Select() {
        PlayerPrefs.SetInt("SelectedCarID",currentCarIndex);
        //SceneManager.LoadScene(2);
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
}  