using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarNameDisplay : MonoBehaviour
{
    public CarStats CS;
    [SerializeField] private CarSpawnerV3 spawner;

    [SerializeField] private string name;
    [SerializeField] private TextMeshPro NameDisplay;

    private void FixedUpdate() {
        CS = spawner.car.GetComponent<CarStats>();
        name = CS.CarName;
        NameDisplay.text = name;
    }
}
