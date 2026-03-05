using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Resetcar : MonoBehaviour
{
    public Rigidbody rb;
    bool WantsToResetCar;
    PlayerControls controls;
    [SerializeField] private Vector3 checkPointPos;
    [SerializeField] private Vector3 checkPointRot;
    bool holding = false;
    float value = 0f;
    [SerializeField] private Image slider;
    [SerializeField] GameObject sliderParent;
    [SerializeField] private float delay = 1f;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();
        controls.Gameplay.ResetCar.performed += ctx => holding = true;
        controls.Gameplay.ResetCar.canceled += ctx => stop();
    }

    IEnumerator ResetKrow(){
        if(checkPointPos != Vector3.zero){
            transform.position = checkPointPos;
            transform.eulerAngles = checkPointRot;
            rb.isKinematic = true; 
        }else{
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            rb.isKinematic = true;
        }
        yield return new WaitForSeconds(2f);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 12){
            if(other.gameObject.GetComponent<SphereCollider>() != null){
                checkPointPos = other.gameObject.transform.position + other.gameObject.GetComponent<SphereCollider>().center;
            }else{
                checkPointPos = other.gameObject.transform.position + new Vector3(0f, 2f, 0f);
                checkPointRot = this.gameObject.transform.localEulerAngles;
            }
            
            checkPointRot = other.gameObject.transform.localEulerAngles;
        }
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }


    void stop(){
        holding = false;
        rb.isKinematic = false;
    }

    
    private void FixedUpdate() {
        sliderParent.SetActive(holding);
        if(holding){
            value += Time.deltaTime;
        }else{
            value = 0f;
        }
        if(value >= delay){
            value = 0f;
            StartCoroutine(ResetKrow());
        }
        slider.fillAmount = value;
    }
}
