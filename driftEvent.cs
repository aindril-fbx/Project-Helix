using UnityEngine;
using UnityEngine.UI;

public class driftEvent : MonoBehaviour
{
    [SerializeField] private GameObject[] objsToSetActive;
    public bool eventStarted;
    [SerializeField] private float speed;
    [SerializeField] private float thresholdSpeed;
    [SerializeField] private ComboDisplay CDisplay;
    [SerializeField] private Transform startPosition;
    [SerializeField] private float[] starValues = new float[3];

    [SerializeField] private GameObject canvasObj;
    [SerializeField] private GameObject mainCanvasObj;
    [SerializeField] private GameObject barrierEmpty;
    [SerializeField] private GameObject Vcam;
    [SerializeField] private Button startButton;
    [SerializeField] private Button secondStartButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Ghost ghost;
    [SerializeField] private bool record = false;
    [SerializeField] private GameObject ghostCar;
    [SerializeField] private GameObject finishLine;

    [SerializeField] private CanvasControlsEvent CCE;

    private GameObject car;

    private void Start()
    {
        Vcam.transform.parent = null;
        mainCanvasObj.SetActive(false);
        canvasObj.SetActive(false);
        barrierEmpty.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (eventStarted)
        {
            foreach (GameObject obj in objsToSetActive)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject obj in objsToSetActive)
            {
                obj.SetActive(false);
            }
        }
        float distance = Vector3.Distance(ghostCar.transform.position, finishLine.transform.position);
        if (distance < 10f)
        {
            ghostCar.transform.position = ghost.position[10];
            ghostCar.transform.rotation = ghost.rotation[10];
            ghostCar.SetActive(false);
            Debug.Log("Ghost disabled");
        }
    }

    public void endEvent()
    {
        ghostCar.SetActive(false);
        eventStarted = false;
        CDisplay.endEvent();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            speed = other.gameObject.GetComponent<CarController>().currentSpud;
            CDisplay = other.gameObject.GetComponent<CarController>().CDisplay;
            car = other.gameObject;
            if (speed < thresholdSpeed && !eventStarted)
            {
                showUI(true);
            }
        }
    }

    void showUI(bool active)
    {
        canvasObj.SetActive(active);
        if (active)
        {
            startButton.onClick.AddListener(() =>
            {
                /*
                if(!record){
                    ghostCar.transform.position = ghost.position[10];
                    ghostCar.transform.rotation = ghost.rotation[10];
                    ghostCar.SetActive(true);
                    car.transform.rotation = startPosition.rotation;
                    car.transform.position = startPosition.position;
                }
                CDisplay.record = record;
                CDisplay.ghost = ghost;
                CDisplay.playAnimation(0);
                CDisplay.starValues = starValues;
                eventStarted = true;
                */
                ghostCar.transform.position = ghost.position[10];
                ghostCar.transform.rotation = ghost.rotation[10];
                car.transform.position = startPosition.position;
                car.transform.rotation = startPosition.rotation;
                car.GetComponent<Rigidbody>().isKinematic = true;
                car.GetComponent<waypointSetter>().targetTransform = finishLine.transform;
                CCE.showControls = false;
                CCE.showSpeedo = false;
                //startButton.gameObject.SetActive(false);
                ShowMainUI(true);
                Vcam.SetActive(true);
            });
        }
    }

    void ShowMainUI(bool active)
    {
        mainCanvasObj.SetActive(active);
        barrierEmpty.SetActive(active);
        secondStartButton.onClick.AddListener(() => {
            CCE.showControls = true;
            CCE.showSpeedo = true;
            startEvent();
            mainCanvasObj.SetActive(false);
            barrierEmpty.SetActive(false);
            Vcam.SetActive(false);
            
        });
        cancelButton.onClick.AddListener(() =>
        {   
            eventStarted = false;
            CCE.showControls = true;
            CCE.showSpeedo = true;
            showUI(false);
            mainCanvasObj.SetActive(false);
            barrierEmpty.SetActive(false);
            Vcam.SetActive(false);
            car.GetComponent<Rigidbody>().isKinematic = false;
            car = null;
        });
    }

    void startEvent()
    {
        if (!record)
        {
            ghostCar.transform.position = ghost.position[10];
            ghostCar.transform.rotation = ghost.rotation[10];
            ghostCar.SetActive(true);
            car.transform.rotation = startPosition.rotation;
            car.transform.position = startPosition.position;
        }
        CDisplay.record = record;
        CDisplay.ghost = ghost;
        CDisplay.playAnimation(0);
        CDisplay.starValues = starValues;
        eventStarted = true;
    }

    void OnTriggerExit(Collider other)
    {
        //car = null;
        showUI(false);
        speed = 0f;
    }
}