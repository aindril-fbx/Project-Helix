using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TyreTemp : MonoBehaviour
{
    public bool tyreSmoke;
    public WheelSkid WS;
    public float SmokeCoe = 80f;
    public float wheelTemp = 0f;
    [SerializeField] private float SmokeTemp = 60f;
    float currentTemp;
    public ParticleSystem smokeEFX;
    public Vector2 GUIpos;
    
    // Start is called before the first frame update
    void Start()
    {
        var emission = smokeEFX.emission;
        WS = GetComponent<WheelSkid>();
                smokeEFX.Play();
                emission.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {   
        float maxTemp = WS.tempAdd ? SmokeCoe : 0f;
        if(WS.tempAdd){
            currentTemp = Mathf.Lerp(currentTemp , maxTemp , WS.WheelTempAdd/20f * Time.deltaTime);
        }else{
            currentTemp = Mathf.Lerp(currentTemp , 0f , Time.deltaTime * 2f);

        }
        wheelTemp = currentTemp;
        TyreSmoke(tyreSmoke);
    }

    void TyreSmoke(bool yes){
        var emission = smokeEFX.emission;
        float x = Mathf.Clamp(wheelTemp/SmokeTemp,0f,1f);
        float emmissionRate = 5f;
        emission.enabled = yes && WS.tempAdd && x>0.8f;
        emission.rateOverTime = emmissionRate * x;
        emission.rateOverDistance = x;
    }

    void OnGUI(){
        GUI.VerticalSlider(new Rect(GUIpos.x, GUIpos.y, 10, 30), wheelTemp , 75f , 0f);
        
    }
}
