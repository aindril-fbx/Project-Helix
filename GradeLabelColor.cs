using UnityEngine;
using TMPro;

public class GradeLabelColor : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Color[] gradeColors;

    private void LateUpdate() {
        if(label.text == "C"){
            label.color = gradeColors[0];
        }else if(label.text == "B"){
            label.color = gradeColors[1];
        }else if(label.text == "A"){
            label.color = gradeColors[2];
        }else if(label.text == "S"){
            label.color = gradeColors[3];
        }else if(label.text == "SS"){
            label.color = gradeColors[4];
        }
    }
}