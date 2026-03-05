using UnityEngine;

public class MainMenuRemarkAnimation : MonoBehaviour {
    
    [SerializeField] private Vector2 OnPosition;
    [SerializeField] private Vector2 OFFPosition;
    [SerializeField] private float Duration = 0.2f;
    bool on = false;
    public void ChangeState() {
        if(!on){
            this.transform.LeanMoveLocal(OnPosition,Duration).setEaseInQuart();
            on = true;
        }else{
            this.transform.LeanMoveLocal(OFFPosition,Duration).setEaseInQuart();
            on = false;
        }
    }
}