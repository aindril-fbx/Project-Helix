using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCameraController : MonoBehaviour
{
    public PlayerControls controls;
    public Vector2 StickValue;
    public Vector3 _LocalRotation;
    float targetAngle;
    public GameObject Car;
    [SerializeField] private Vector3 FixedCamPos;
    public GameObject CarHolder;
    public GameObject FollowCam;
    public GameObject OrbitCam;
    [SerializeField] private GameObject FixedCamCamera;
    [SerializeField] private GameObject FixedCamPositionParent;
    Vector3 direction;
    public GameObject Camera;
    bool fixedCam = false;

    [SerializeField] private float idleTime;
    float afkTime;
    [SerializeField] private GameObject IdleAnimParent;
    [SerializeField] private bool isIdle;
    [SerializeField] private CanvasControlsEvent CCE;
    //public float OrbitDampning = 1f;

    void Awake()
    {
        controls = new PlayerControls(); 
        Invoke("SetUp",0.1f);
        
        controls.Gameplay.CameraRX.performed += ctx => StickValue.x = ctx.ReadValue<float>();
        controls.Gameplay.CameraRY.performed += ctx => StickValue.y = ctx.ReadValue<float>();
        controls.Gameplay.CameraRX.canceled += ctx => StickValue.x = 0f;
        controls.Gameplay.CameraRY.canceled += ctx => StickValue.y = 0f;
        controls.Gameplay.FixedCam.performed += ctx => fixedCam = !fixedCam; 

        Camera.transform.parent = null;

    }

    private void Update()
    {
        float vertical = StickValue.y;
        float horizontal = StickValue.x;
        Vector3 inputlol = new Vector3(0f , 0f , Mathf.Abs(horizontal) + Mathf.Abs(vertical)).normalized;
        direction = new Vector3(horizontal , 0f , vertical).normalized;

        if(direction.magnitude >= 0.3f){
            // This line is black magic
            targetAngle = Mathf.Atan2(direction.x , direction.z) * Mathf.Rad2Deg + Car.transform.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation , Quaternion.Euler(0f , targetAngle , 0f) , Time.deltaTime * 5f);
            FollowCam.SetActive(true);
            OrbitCam.SetActive(false);
            FixedCamCamera.SetActive(false);
            CCE.firstpersonUI = false;
        }else{
            if(fixedCam){
                transform.localEulerAngles = new Vector3(0f , 0f , 0f);
                FixedCamCamera.SetActive(true);
                OrbitCam.SetActive(false);
                FollowCam.SetActive(false);
                CCE.firstpersonUI = true;
            }else{
                targetAngle = 0f;
                transform.localEulerAngles = new Vector3(0f , 0f , 0f);
                FixedCamCamera.SetActive(false);
                OrbitCam.SetActive(true);
                FollowCam.SetActive(false);
                CCE.firstpersonUI = false;
            } 
        }
        if (Input.anyKey || Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f){
            afkTime = 0f;
            isIdle = false;
        }else{
            afkTime += Time.deltaTime;
            if(afkTime > idleTime){
                isIdle = true;
            }
        }

        if(isIdle){
            IdleAnimParent.SetActive(true);
            FixedCamCamera.SetActive(false);
            OrbitCam.SetActive(false);
            FollowCam.SetActive(false);
        }else{
            IdleAnimParent.SetActive(false);          
        }
    }

    private void SetUp () {
       CarHolder = GameObject.FindWithTag("CarHolder");
       Car = CarHolder.transform.GetChild(0).gameObject;
       FixedCamPos = Car.GetComponent<CarStats>().fixedCamPos;
       FixedCamPositionParent.transform.localPosition = FixedCamPos;
    }
   
    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
