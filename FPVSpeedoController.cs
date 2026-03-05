using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPVSpeedoController : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI speedo_t;
    [SerializeField] private TextMeshProUGUI units_t;
    [SerializeField] private TextMeshProUGUI gear_t;
    [SerializeField] private TextMeshProUGUI _t;
    [SerializeField] private Slider nosBar;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image revfillImage;
    private Gradient revGradient;
    [SerializeField] private Slider revBar;

    [SerializeField] private GameObject car;

    [SerializeField] private CarController CC;
    [SerializeField] private Nos N;
    [SerializeField] private CanvasControlsEvent CCE;
    [SerializeField] private GameObject displayParent;
    private void OnEnable() {
        car = transform.root.gameObject;
        CC = car.GetComponent<CarController>();
        N = car.GetComponent<Nos>();
        nosBar.maxValue = 100f;
        revGradient = CC.getRevGradient();
        displayParent = this.gameObject.transform.GetChild(0).gameObject;
    }
    private void Update() {
        gear_t.text = CC.calculateGearText();
        speedo_t.text = CC.GetMetricUnit() == "MPH" ? CC.currentSpud + "" : Mathf.Round(CC.currentSpud * 1.6f) + "";
        units_t.text = CC.GetMetricUnit();
        revBar.value = CC.Revs;
        nosBar.value = N.GetNosValue();
        revfillImage.color = CC.getRevGradient().Evaluate(revBar.normalizedValue);
        fillImage.color = N.FuelGradient.Evaluate(nosBar.normalizedValue);
        displayParent.SetActive(CCE.firstpersonUI);
    }
}