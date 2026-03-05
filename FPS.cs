using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public int MaxFPS = 60;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = MaxFPS;
    }


}
