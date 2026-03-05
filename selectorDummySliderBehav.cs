using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class selectorDummySliderBehav : MonoBehaviour {
    [SerializeField] private Slider dummySlider;
    [SerializeField] private Slider actualSlider;

    [SerializeField] private TextMeshProUGUI blackText;
    [SerializeField] private TextMeshProUGUI whiteText;

    
    private void LateUpdate() {
        dummySlider.maxValue = actualSlider.maxValue;
        dummySlider.value = actualSlider.value;

        blackText.transform.position = whiteText.transform.position;
        blackText.text = whiteText.text;
    }

}