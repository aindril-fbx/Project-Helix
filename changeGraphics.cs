using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class changeGraphics : MonoBehaviour
{
    [SerializeField] private RenderPipelineAsset[] RPA; // 0: Very Low, 1: Low, 2: Midium, 3: High
    [SerializeField] private Slider shadowQualitySlider;


    [SerializeField] private GameObject[] videoSettingsObjects;
    [SerializeField] private GameObject[] audioSettingsObjects;

    [SerializeField] private AudioMixer MasterVolume;
    [SerializeField] private AudioMixer ImpactVolume;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;

    [SerializeField] private Toggle MetricUnits;
    [SerializeField] private TextMeshProUGUI metricUnitLabel;

    [SerializeField] private Transform knobParent;
    [SerializeField] private float angle = 40f;

    [Header("Input Type")]
    bool inputType = true; // true: Keyboard, false: tilt
    [SerializeField] TextMeshProUGUI inputTypeLabel;
    [SerializeField] private inputType IT;

    [Header("Tabs")]
    [SerializeField] private Button[] tabButtons;


    private void Start()
    {
        if (IT.inputType_ == 0)
        {
            inputTypeLabel.text = "Buttons";
        }
        else
        {
            inputTypeLabel.text = "Tilt";
        }
        int x = PlayerPrefs.GetInt("QualityOption");
        QualitySettings.renderPipeline = RPA[x];
        shadowQualitySlider.value = x;
        if (PlayerPrefs.GetInt("MetricUnit", 0) == 0)
        {
            MetricUnits.isOn = false;
            metricUnitLabel.text = "KMPH";
            knobParent.LeanRotate(new Vector3(0f, 0f, angle), 0.2f).setEaseInQuart();
        }
        else
        {
            MetricUnits.isOn = true;
            knobParent.LeanRotate(new Vector3(0f, 0f, -angle), 0.2f).setEaseInQuart();
            metricUnitLabel.text = "MPH";
        }
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0);
        setVolume();

        foreach (Button b in tabButtons)
        {
            b.onClick.AddListener(() =>
            {
                ChangeTab(int.Parse(b.gameObject.name));
            });
        }
    }
    public void ChangeSetting()
    {
        QualitySettings.renderPipeline = RPA[(int)shadowQualitySlider.value];
    }

    public void Save()
    {
        //this.gameObject.SetActive(false);
        PlayerPrefs.SetInt("QualityOption", (int)shadowQualitySlider.value);
        PlayerPrefs.SetFloat("MasterVolume", MasterSlider.value);
        if (MetricUnits.isOn)
        {
            PlayerPrefs.SetInt("MetricUnit", 1);
            metricUnitLabel.text = "MPH";
        }
        else
        {
            PlayerPrefs.SetInt("MetricUnit", 0);
            metricUnitLabel.text = "KMPH";
        }
    }

    public void ChangeInputType()
    {
        inputType = IT.inputType_ == 0 ? true : false;
        if (inputType)
        {
            IT.inputType_ = 1;
            PlayerPrefs.SetInt("InputType", 1);
            inputTypeLabel.text = "Tilt";
        }
        else
        {
            IT.inputType_ = 0;
            PlayerPrefs.SetInt("InputType", 0);
            inputTypeLabel.text = "Buttons";
        }
    }

    public void ChangeMetricLabel()
    {
        if (metricUnitLabel.text == "MPH")
        {
            metricUnitLabel.text = "KMPH";
            knobParent.LeanRotate(new Vector3(0f, 0f, angle), 0.2f).setEaseInQuart();
        }
        else
        {
            metricUnitLabel.text = "MPH";
            knobParent.LeanRotate(new Vector3(0f, 0f, -angle), 0.2f).setEaseInQuart();
        }
    }

    [SerializeField] private Color disabledColor;
    [SerializeField] private Color enabledColor;
    void ChangeTab(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                foreach (GameObject obj in videoSettingsObjects)
                {
                    obj.SetActive(true);
                }
                foreach (GameObject obj in audioSettingsObjects)
                {
                    obj.SetActive(false);
                }
                break;
            case 1:
                foreach (GameObject obj in videoSettingsObjects)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in audioSettingsObjects)
                {
                    obj.SetActive(true);
                }
                break;
            case 2:
                foreach (GameObject obj in videoSettingsObjects)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in audioSettingsObjects)
                {
                    obj.SetActive(true);
                }
                break;
            default:
                break;
        }
        for (int i = 0; i < tabButtons.Length; i++)
        {
            Shadow outline = tabButtons[i].gameObject.GetComponent<Shadow>();

            if (i == tabIndex)
            {
                outline.effectColor = enabledColor;
            }
            else
            {
                outline.effectColor = disabledColor;
            }
        }
    }

    public void setVolume()
    {
        Debug.Log(MasterSlider.value);
        MasterVolume.SetFloat("volume", MasterSlider.value);
        ImpactVolume.SetFloat("volume", MasterSlider.value - 0.2f);
    }
}
