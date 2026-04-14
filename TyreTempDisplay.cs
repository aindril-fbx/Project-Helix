using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TyreTempDisplay : MonoBehaviour
{
    [SerializeField] private InputManager IM;
    [SerializeField] private TyreTemp[] TT;
    [SerializeField] private Image[] TempImage;
    [SerializeField] private Slider[] TempSlider;
    [SerializeField] private Gradient tempGradient;
    [SerializeField] private GameObject tempUI;
    [SerializeField] private GameObject SpeedDisplay;
    [SerializeField] private GameObject Driftangle;

    // Start is called before the first frame update
    void Start()
    {
        IM = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IM.showDisplay){
            tempUI.SetActive(true);
        }else{
            tempUI.SetActive(false);
        }
        if(Time.timeScale > 0.1f){
            SpeedDisplay.SetActive(IM.showSpeed ? true : false);
            Driftangle.SetActive(IM.showAngle ? true : false);
        }else{
            Driftangle.SetActive(false);
        }
        for (int i = 0; i < 4; i++)
        {
            TempSlider[i].maxValue = TT[i].SmokeCoe;
            TempSlider[i].value = TT[i].wheelTemp;
            TempImage[i].color = tempGradient.Evaluate(TempSlider[i].normalizedValue);
        }

        if(Time.timeScale < 0.1f){
            SpeedDisplay.SetActive(false);
            Driftangle.SetActive(false);
            tempUI.SetActive(false);
        }
    }
}
