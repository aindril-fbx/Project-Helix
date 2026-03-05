using UnityEngine;
using UnityEngine.UI;

public class AuxInputButtons : MonoBehaviour {
    [SerializeField] private Button[] inputButtons;
    [SerializeField] private Button actionButton;
    [SerializeField] private Slider maskSlider;

    public float rate = 10f;
    bool active = false;

    private void Start() {
        actionButton = GetComponent<Button>();
        actionButton.onClick.AddListener(()=>{
            active = !active;
        });
        foreach(Button b in inputButtons) {
            b.onClick.AddListener(()=>{
                for(int i = 0; i < inputButtons.Length; i++) {
                    active = false;
                }
            }); 
        }
    }

    private void Update() {
        if(active){
            maskSlider.value = Mathf.Lerp(maskSlider.value,1f,Time.deltaTime*rate);
        }else{
            maskSlider.value = Mathf.Lerp(maskSlider.value,0f,Time.deltaTime*rate);
        }
    }
}