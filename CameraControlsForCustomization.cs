using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraControlsForCustomization : MonoBehaviour
{
    [SerializeField] private Slider viewSlider;
    [SerializeField] private GameObject carHolder;
    [SerializeField] private float smoothing = 5f; 
    [SerializeField] private FixedTouchField FTF;
    [Range(0f,5f)]
    [SerializeField] private float sens = 1f;

    // Update is called once per frame
    void Update()
    {
        if(viewSlider.value >= viewSlider.maxValue){
            viewSlider.value = viewSlider.minValue;
        }else if(viewSlider.value <= viewSlider.minValue){
            viewSlider.value = viewSlider.maxValue;
        }
        viewSlider.value = Mathf.Lerp(viewSlider.value,viewSlider.value + FTF.TouchDist.x * -1f * sens, .2f);
        carHolder.transform.rotation = Quaternion.Slerp(carHolder.transform.rotation , Quaternion.Euler(0f,viewSlider.value,0f), Time.deltaTime * smoothing);
    }
}
