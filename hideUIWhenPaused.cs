using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class hideUIWhenPaused : MonoBehaviour
{
    private RectTransform rt;
    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (Time.timeScale <= 0.1)
        {
            rt.localScale = new Vector3(0f, 0f, 0f);
        }
        else
        {
            rt.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
