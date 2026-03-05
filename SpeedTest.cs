using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedTest : MonoBehaviour
{
    private CarController CC;
    private SpeedTestValue SV;
    public TextMeshProUGUI msgText;
    private MessageTextController MTC;
    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<CarController>();
        msgText.gameObject.transform.parent.gameObject.SetActive(false);
        MTC = msgText.gameObject.transform.parent.gameObject.GetComponent<MessageTextController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        SV = other.gameObject.GetComponent<SpeedTestValue>();
        
        if(SV != null){
            float speed = SV.speedRequired;
            if(CC.currentSpud > speed){
                msgText.gameObject.transform.parent.gameObject.SetActive(true);
                MTC.setText("Speed Test Cleared!");
            }else{
                msgText.gameObject.transform.parent.gameObject.SetActive(true);
                MTC.setText("Not Fast Enough!");
            }
        }
        
    }
}
