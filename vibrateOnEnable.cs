using UnityEngine;

public class vibrateOnEnable : MonoBehaviour {
    [SerializeField] private int vibrateAmount = 50;

    private void OnEnable() {
        Vibrator.Vibrate(vibrateAmount);
    }
}