using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using TMPro;

public class ComboDisplay : MonoBehaviour {
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject displayObject;
    [SerializeField] private MessageTextController MTC;
    [SerializeField] private Image comboMeter;
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private TextMeshProUGUI mulDisplay;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Animation delayAnimation;
    public bool ShowDisplay = false;
    public bool AddScore = false;

    public float Score = 0f;
    public float Score_;
    public float rate = 1f;
    public float Multiplier = 1f;

    Vector3 acceleration;
    Vector3 lastVelocity;

    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private float GroundCheckRadius = 0.5f;

    [SerializeField] private AudioSource tickSound;
    
    [Header("Float Timer")]
    float time_;
    [SerializeField] private float delay = 3f;
    [SerializeField] private bool photoMode = false;
    public PlayerControls controls;

    [Header("Event Stuff")]
    [SerializeField] private Animation[] initAnimation;
    [SerializeField] private TextMeshProUGUI eventLabel;
    [SerializeField] private Color driftColor;
    [SerializeField] private Color raceColor;
    [SerializeField] private Color dragColor;

    [SerializeField] private GameObject scoreCard;
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private TextMeshProUGUI eventScoreLabel;
    public float[] starValues = new float[3];
    [SerializeField] private GameObject[] stars = new GameObject[3];

    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private bool runTimer = false;
    float t = 0f;
    public Ghost ghost;
    public bool record = false;
    [SerializeField] private Vector2 defaultPositon;
    void Awake() {
        controls = new PlayerControls();
        controls.Gameplay.Enable();
        controls.Gameplay.HideUI.performed += ctx => photoModeState();
        defaultPositon = displayObject.GetComponent<RectTransform>().anchoredPosition;
    }
    [SerializeField] private float eventScore = 0f;
    float dot;
    private void FixedUpdate() {
        bool isGrounded;
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckRadius , GroundLayer);
        Vector2 velocity = new Vector2(Mathf.Round(rb.transform.InverseTransformVector(rb.linearVelocity).x) ,Mathf.Round(rb.transform.InverseTransformVector(rb.linearVelocity).z));
        Vector2 transformAngle = new Vector2(rb.transform.localEulerAngles.z,0);
        float dotProduct = Vector2.Dot(velocity.normalized,transformAngle.normalized);
        float shake = 100f * dotProduct;
        displayObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(displayObject.GetComponent<RectTransform>().anchoredPosition,defaultPositon + new Vector2(UnityEngine.Random.Range(-shake,shake),UnityEngine.Random.Range(-shake,shake)),Time.deltaTime);
        if(velocity.magnitude > 10f && Mathf.Abs(dotProduct)>0.3f && isGrounded){
            time_ = 0f;
            Score += rate;
            if(!tickSound.isPlaying){
                tickSound.Play();
            }
            if(Score > 20f){
                //delayAnimation.Stop();
                labelText.text = "Drift";
                displayObject.SetActive(true);
                ShowDisplay = true;
                if(comboMeter.fillAmount >= 1f){
                    comboMeter.fillAmount = 0f;
                    Multiplier += 1f;
                }else{
                    comboMeter.fillAmount += (rate/Multiplier)/50f * Mathf.Abs(dotProduct);
                }
                Score_ = Score * Multiplier;
            }
        }else if(velocity.magnitude > 80f && isGrounded){
            time_ = 0f;
            Score += rate*0.2f;
            if(!tickSound.isPlaying){
                tickSound.Play();
            }
            if(Score > 20f){
                //delayAnimation.Stop();
                labelText.text = "Speed";
                displayObject.SetActive(true);
                ShowDisplay = true;
                if(comboMeter.fillAmount >= 1f){
                    comboMeter.fillAmount = 0f;
                    Multiplier += 1f;
                }else{
                    comboMeter.fillAmount += (rate/Multiplier)/130f;
                }
                Score_ = Score * Multiplier;
            }
        }else if(!isGrounded){
            time_ = 0f;
            Score += rate;
            if(!tickSound.isPlaying){
                tickSound.Play();
            }
            if(Score > 20f){
                //delayAnimation.Stop();
                labelText.text = "Air Time";
                displayObject.SetActive(true);
                ShowDisplay = true;
                if(comboMeter.fillAmount >= 1f){
                    comboMeter.fillAmount = 0f;
                    Multiplier += 1f;
                }else{
                    comboMeter.fillAmount += (rate/Multiplier)/130f;
                }
                Score_ = Score * Multiplier;
            }
        
        }else{
            if(ShowDisplay){
                //delayAnimation.Play();
                time_ += Time.deltaTime;
                if(time_ >= delay){
                    ShowDisplay = false;
                    AddScore = true;
                }
            }else{
                displayObject.SetActive(false);
                if(AddScore){
                    MTC.gameObject.SetActive(true);
                    MTC.setText(Mathf.Round(Score_) + " Points");
                    eventScore += Score_;
                    AddScore = false;
                }
                comboMeter.fillAmount = 0f;
                Score = 0f;
                Multiplier = 1f;
            }
        }
        dot = dotProduct * 180f;
        scoreDisplay.text = Mathf.Round(Score) + "";
        mulDisplay.text = "x " + Multiplier;

        eventScoreLabel.text = eventScore + "";
    }

    public void playAnimation(int x){
        Score = 0f;
        Multiplier = 1f;
        comboMeter.fillAmount = 0f;
        Score_ = 0f;
        StartCoroutine(countDown());
        if(record){
            GhostRecorder GR = rb.transform.gameObject.AddComponent(typeof(GhostRecorder)) as GhostRecorder;
            GR.enabled = true;
            GR.ghost = ghost;
            ghost.isRecord = true;
            GR.StartRecording();
        }
        if(x == 0){
            eventScore = 0f;
            eventLabel.text = "DRIFT EVENT";
            eventLabel.color = driftColor;
            
            Debug.Log(x);
        }else if(x == 1){
            eventLabel.text = "RACE EVENT";
            eventLabel.color = raceColor;
            Debug.Log(x);
        }else if(x == 2){
            eventLabel.text = "DRAG EVENT";
            eventLabel.color = dragColor;
            Debug.Log(x);
        }
    }

    IEnumerator countDown(){
        rb.isKinematic = true;
        for(int i = 0; i < initAnimation.Length; i++) {
            if(i != 0){
                yield return new WaitForSeconds(2f);
            }
            initAnimation[i].Play();
        }
        timerLabel.gameObject.transform.parent.gameObject.SetActive(true);
        runTimer = true;
        t = 0f;
        rb.isKinematic = false;
    }

    public void endEvent(){
        runTimer = false;
        t = 0f;
        eventScore += Score_;
        Debug.Log("Event Ended: " + eventScore);
        StartCoroutine(displayStars(eventScore));
        StartCoroutine(endSequence());
    }

    IEnumerator endSequence(){
        if(ghost != null){
            ghost.isRecord = false;
        }
        scoreCard.SetActive(true);
        scoreLabel.text = eventScore + "";
        eventScore = 0f;
        timerLabel.gameObject.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(6f);
        scoreCard.SetActive(false);
    }

    IEnumerator displayStars(float _score){
        foreach(GameObject obj in stars) {
            obj.SetActive(false);
        }
        Debug.Log("displayStars: " + _score);
        for(int i = 0; i < starValues.Length; i++) {
            if(_score >= starValues[i]){
                stars[i].SetActive(true);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void LateUpdate() {
        acceleration = (rb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = rb.linearVelocity;
        if(acceleration.magnitude > 500f){
            ShowDisplay = false;
            comboMeter.fillAmount = 0f;
            Multiplier = 1f;
            if(Score_ > 20f){
                MTC.gameObject.SetActive(true);
                MTC.setText("Combo Failed! Points: " + (Score * Multiplier).ToString());
                Score_ = 0f;
            }
            Score = 0f;
        }

        if(Time.timeScale < 0.1f){
            displayObject.SetActive(false);
        }

        if(runTimer){
            t += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(t);
            timerLabel.text = timeSpan.ToString(@"mm\:ss\:ff");
        }
    }

    public void photoModeState() {
        Debug.Log(photoMode);
        photoMode = !photoMode;
    }

    void OnGUI()
    {
        //GUI.HorizontalSlider(new Rect(10, 300, 100, 10), dot , -200 , 200);
    }
}