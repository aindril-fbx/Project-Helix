using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DriftAngle : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Slider AngleBar;
    [Range(-90f,90f)]
    public float Angle = 0f;
    public TextMeshProUGUI angleDisplay;
    [SerializeField] private GameObject displayParent;

    
    private void Start() {

        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 velocity = new Vector2(Mathf.Round(transform.InverseTransformVector(rb.linearVelocity).x) ,Mathf.Round(transform.InverseTransformVector(rb.linearVelocity).z));
        Vector2 transformAngle = new Vector2(rb.transform.localEulerAngles.y,0);
        Angle = -Vector2.Angle(velocity,transformAngle) + 90f;
        if(rb.linearVelocity.magnitude >= 1f){
            AngleBar.value = Angle;
            angleDisplay.text = " "+ Mathf.Round(Angle) + "°";
        }else{
            AngleBar.value = 0f;
            angleDisplay.text = " 0°";
        }
    }
}
