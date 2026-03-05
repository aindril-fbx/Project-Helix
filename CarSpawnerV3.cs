using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnerV3 : MonoBehaviour
{
    [SerializeField] private GameObject[] Carlist = new GameObject[4];
    public GameObject car;
    [SerializeField] private bool SetParent = true;
    
    
    private void Awake() {
        int SelectedCarID = PlayerPrefs.GetInt("SelectedCarID");
        car = Instantiate(Carlist[SelectedCarID], transform.position, transform.rotation);
        if(SetParent){
            car.transform.parent = this.transform;
        }
        car.transform.localScale = new Vector3(1f,1f,1f);
    }   
}
