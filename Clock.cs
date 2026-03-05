using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Clock : MonoBehaviour
{
    public TMP_Text clockText;
    DateTime time;

    // Start is called before the first frame update
    void Start()
    {
        //clockText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        time = DateTime.Now;
        string hour = "";
        string minute = LeadingZero(time.Minute);
        string second = LeadingZero(time.Second);

        if(time.Hour >12){ 
            hour = LeadingZero(time.Hour-12);
        }else{
            hour = LeadingZero(time.Hour);
        }
        clockText.text = hour + ":" + minute;
    }

    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
}
