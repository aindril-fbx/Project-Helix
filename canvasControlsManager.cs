using UnityEngine;

public class canvasControlsManager : MonoBehaviour {
    [SerializeField] private CanvasControlsEvent CCE;
    [SerializeField] private GameObject canvas;

    [SerializeField] private GameObject[] inputContols;
    [SerializeField] private inputType IT;
    
    private void OnEnable() {
        inputContols[0].SetActive(true);
        inputContols[1].SetActive(false);
    }
    private void FixedUpdate() {
        if(!canvas.activeSelf == CCE.showControls){
            canvas.SetActive(CCE.showControls);
        }

        for(int i = 0; i < inputContols.Length; i++) {
            if(i == IT.inputType_){
                inputContols[i].SetActive(true);
            }else{
                inputContols[i].SetActive(false);
            }
        }
    }
}