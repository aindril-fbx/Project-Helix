using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerControls controls;
    public CanvasControlsEvent CCE;
    public float Throttle;
    public float ThrottleValue;
    private float SteerValue;
    float buttonSteer;
    public float Steer;

    public bool HandBrake;
    public bool Nos_;
    public bool showDisplay = false;
    public bool showAngle = false;
    public bool showSpeed = true;
    public bool useDriftSteering = false;
    public bool light_ = false;
    [SerializeField] private Quaternion Gyro;

    [SerializeField] private float idleTime = 15f;
    float afkTime;
    bool showspeed_ = true;

    [SerializeField] private MessageTextController MTC;

    [SerializeField] private bool ghostMode = false;
    [SerializeField] private Ghost ghost;

    private float timeValue;
    private int index1;
    private int index2;

    private Rigidbody rb;

    private float throttleIN;
    private float brakeIN;

    public bool burnOutBool;

    [Header("Type of Input")]
    [SerializeField] private inputType inputType; // 0 = button, 1 = tilt, 2 = sliders;
    [SerializeField] private float sens = 10f;
    // Start is called before the first frame update
    void Awake()
    {
        if (CCE != null)
        {
            CCE.showControls = true;
            CCE.showSpeedo = true;
        }
        inputType.inputType_ = PlayerPrefs.GetInt("InputType", 0);
        controls = new PlayerControls();
        if (!ghostMode)
        {
            controls.Gameplay.Accelarate.performed += ctx => ThrottleValue = ctx.ReadValue<float>();
            controls.Gameplay.Accelarate.canceled += ctx => ThrottleValue = 0f;
            controls.Gameplay.Steer.performed += ctx => buttonSteer = ctx.ReadValue<float>();
            controls.Gameplay.Steer.canceled += ctx => buttonSteer = 0f;
            controls.Gameplay.Brake.performed += ctx => HandBrake = true;
            controls.Gameplay.Brake.canceled += ctx => HandBrake = false;
            controls.Gameplay.Nos.performed += ctx => Nos_ = true;
            controls.Gameplay.Nos.canceled += ctx => Nos_ = false;
            controls.Gameplay.TyreTempDisplay.performed += ctx => showDisplay = !showDisplay;
            controls.Gameplay.DriftAngle.performed += ctx => showAngle = !showAngle;
            controls.Gameplay.SpeedDisplay.performed += ctx => changeSpeedoState();
            controls.Gameplay.DriftSteering.performed += ctx => changeDS();
            controls.Gameplay.Lights.performed += ctx => light_ = !light_;
            controls.Gameplay.ThrottleInput.performed += ctx => throttleIN = ctx.ReadValue<float>();
            controls.Gameplay.ThrottleInput.canceled += ctx => throttleIN = 0f;
            controls.Gameplay.BrakeInput.performed += ctx => brakeIN = ctx.ReadValue<float>();
            controls.Gameplay.BrakeInput.canceled += ctx => brakeIN = 0f;
        }
        else
        {
            timeValue = 0f;
        }
    }

    private void setSteering(int type)
    {
        if (ghostMode)
        {
            return;
        }
        if (type == 0)
        {
            SteerValue = buttonSteer;
        }
        else if (type == 1)
        {
            SteerValue = Mathf.Clamp((Input.acceleration.x * sens) / 90f, -1f, 1f);
        }
    }
    void changeSpeedoState()
    {
        showSpeed = !showSpeed;
        // CCE.showSpeedo = !CCE.showSpeedo;
    }
    private void Update()
    {
        if (!ghostMode)
        {
            Throttle = Mathf.Lerp(Throttle, ThrottleValue, Time.deltaTime * 10f);
            Steer = Mathf.Lerp(Steer, SteerValue, Time.deltaTime * 6f);
            Gyro = Input.gyro.attitude;
            if (
                    Input.anyKey ||
                    Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f ||
                    Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f ||
                    Input.GetAxis("Joystick X") != 0f || Input.GetAxis("Joystick Y") != 0f ||
                    Input.GetButton("Fire1") || Input.GetButton("Fire2") || Input.GetButton("Jump"))
            {
                afkTime = 0f;
                if (showspeed_)
                {
                    showSpeed = true;
                    showspeed_ = !showspeed_;
                    //Debug.Log(CCE.showSpeedo);
                }
            }
            else
            {
                afkTime += Time.deltaTime;
                if (afkTime > idleTime)
                {
                    showSpeed = false;
                    showDisplay = false;
                    showspeed_ = true;
                    CCE.firstpersonUI = false;
                }
            }
        }
        setSteering(inputType.inputType_);
        if (Time.timeScale < 0.2f)
        {
            CCE.firstpersonUI = false;
        }
        burnOut();
    }

    private void burnOut()
    {
        burnOutBool = throttleIN + brakeIN > 1.4f ? true : false;
        //Debug.Log(throttleIN+" , "+ brakeIN);
    }

    private void FixedUpdate()
    {
        if (ghostMode)
        {
            timeValue += Time.deltaTime;
            if (ghost.isReplay)
            {
                GetIndex();
                SetValues();
            }
        }
    }

    void GetIndex()
    {
        for (int i = 0; i < ghost.timeStamp.Count - 2; i++)
        {
            if (ghost.timeStamp[i] == timeValue)
            {
                index1 = i;
                index2 = i;
                return;
            }
            else if (ghost.timeStamp[i] < timeValue & timeValue < ghost.timeStamp[i + 1])
            {
                index1 = i;
                index2 = i + 1;
                return;
            }
        }


    }

    void SetValues()
    {
        if (index1 == index2)
        {
            Throttle = ghost.throttle[index1];
            Steer = ghost.steering[index1];
            HandBrake = ghost.handBrake[index1];
            rb.AddForce((ghost.position[index1] - this.gameObject.transform.position) * 10000f * Time.deltaTime);
        }
        else
        {
            float interpolationFactor = (timeValue - ghost.timeStamp[index1]) / (ghost.timeStamp[index2] - ghost.timeStamp[index1]);

            Throttle = Mathf.Lerp(ghost.throttle[index1], ghost.throttle[index2], interpolationFactor);
            Steer = Mathf.Lerp(ghost.steering[index1], ghost.steering[index2], interpolationFactor);
            HandBrake = ghost.handBrake[index1];
            rb.AddForce((position(interpolationFactor) - this.gameObject.transform.position) * 10000f * Time.deltaTime);
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, position(interpolationFactor), 0.08f);
            this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation, rotation(interpolationFactor), 0.08f);

        }
    }

    Vector3 position(float interpolationFactor)
    {
        return Vector3.Lerp(ghost.position[index1], ghost.position[index2], interpolationFactor);
    }

    Quaternion rotation(float interpolationFactor)
    {
        return Quaternion.Slerp(ghost.rotation[index1], ghost.rotation[index2], interpolationFactor);
    }

    void changeDS()
    {
        useDriftSteering = !useDriftSteering;
        if (useDriftSteering)
        {
            MTC.gameObject.SetActive(true);
            MTC.setText("Drift Assist Active");
        }
        else
        {
            MTC.gameObject.SetActive(true);
            MTC.setText("Drift Assist Disabled");
        }
    }

    IEnumerator NoBoost()
    {
        yield return new WaitForSeconds(5);
        Nos_ = false;
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
        rb = GetComponent<Rigidbody>();
        timeValue = 0f;
        index1 = 0;
        index2 = 0;
        this.gameObject.transform.position = ghost.position[0];
        this.gameObject.transform.rotation = ghost.rotation[0];
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
