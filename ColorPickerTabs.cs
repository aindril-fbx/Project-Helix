using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerTabs : MonoBehaviour
{
    [SerializeField] private GameObject[] tabs;
    [SerializeField] private GameObject[] windows;
    [SerializeField] public int currentTab = 0;

    public void ChangeTabIndex(int x) {
        currentTab = x;
    }

    private void FixedUpdate() {
        for(int i = 0; i < tabs.Length; i++) {
            if(i == currentTab){
                tabs[i].transform.GetChild(0).gameObject.SetActive(true);
                if(currentTab < 3){
                    windows[0].SetActive(true);
                    windows[1].SetActive(false);
                }else{
                    windows[0].SetActive(false);
                    windows[1].SetActive(true);
                }
            }else{
                tabs[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        
    }
    
    
}
