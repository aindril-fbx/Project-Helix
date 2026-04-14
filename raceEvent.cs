using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class raceEvent : MonoBehaviour
{
    public bool debug = false;
    [SerializeField] private Vector2 debugPos = new Vector2(10, 10);
    [Header("CheckPoints")]
    [SerializeField] private GameObject[] CheckPoints;
    [SerializeField] private int currentCP = 0;
    [SerializeField] private int totalCP = 0;

    [SerializeField] private bool lapped = false;
    [SerializeField] private int lap = 1;
    [Range(2, 100)]
    [SerializeField] private int totalLaps = 10;
    [SerializeField] private float distanceFromFinishLine = 0f;

    public bool raceStarted = false;

    [Header("Timer")]
    [SerializeField] private float lapTime = 0f;
    float t = 0f;
    [SerializeField] private float bestTime = 0f;

    [Header("Text Objects")]
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI lapTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI positionText;

    [Header("AI Racers")]
    [SerializeField] private GameObject[] aiRacers;
    [SerializeField] private int[] aiPositions;
    [SerializeField] private int[] aiLaps;
    [SerializeField] private GameObject playerCar;

    [Header("UI")]
    [SerializeField] private GameObject firstCanvasObj;
    [SerializeField] private GameObject mainCanvasObj;




    private void Start()
    {
        showUI(false);
        CheckPoints[0].GetComponent<checkpointBehavior>().changeState(true);
        totalCP = CheckPoints.Length;
        for (int i = 0; i < totalCP; i++)
        {
            CheckPoints[i].name = i.ToString();
        }
        lapTimeText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (raceStarted) t += Time.deltaTime;
        if (lap > totalLaps)
        {
            raceStarted = false;
        }
        if (currentCP == totalCP && lap <= totalLaps)
        {
            currentCP = 0;
            lapTime = t;
            StartCoroutine(showLapTime(5f));
            if (lapTime < bestTime || bestTime == 0f)
            {
                bestTime = lapTime;
            }
            t = 0f;
            if (lapped)
            {
                lap++;
            }
        }
        UpdateUI();
    }

    public void changeActiveCheckpoint(int index, waypointSetter WPS)
    {
        if (!raceStarted)
        {
            return;
        }
        if (index != currentCP)
        {
            Debug.Log("Wrong Checkpoint");
            return;
        }
        currentCP = index + 1;
        for (int i = 0; i < totalCP; i++)
        {
            if (index == totalCP - 1)
            {
                index = -1;
            }
            if (i == index + 1)
            {
                CheckPoints[i].GetComponent<checkpointBehavior>().changeState(true);
            }
            else
            {
                CheckPoints[i].GetComponent<checkpointBehavior>().changeState(false);
            }

        }
        int waypoint = (index + 2) % totalCP;
        WPS.targetTransform = CheckPoints[waypoint].transform;

    }

    private void UpdateUI()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(t);
        TimeSpan timeSpan2 = TimeSpan.FromSeconds(bestTime);
        lapText.text = lap + "/" + totalLaps;
        if (bestTimeText != null) bestTimeText.text = timeSpan2.ToString(@"mm\:ss\:ff");
        if (Time.timeScale >= 0.1 && raceStarted)
        {
            mainCanvasObj.SetActive(true);
        }
        else
        {
            mainCanvasObj.SetActive(false);
        }
    }

    IEnumerator showLapTime(float time)
    {
        lapTimeText.text = TimeSpan.FromSeconds(lapTime).ToString(@"mm\:ss\:ff");
        lapTimeText.gameObject.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        lapTimeText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(lapTime);
        TimeSpan timeSpan2 = TimeSpan.FromSeconds(bestTime);
        timeSpan.ToString(@"mm\:ss\:ff");
        if (debug)
        {
            GUI.Label(new Rect(debugPos.x, debugPos.y, 100, 20), "Lap: " + lap + "/" + totalLaps);
            GUI.Label(new Rect(debugPos.x, debugPos.y + 25, 120, 20), "Checkpoint: " + currentCP + "/" + totalCP);
            GUI.Label(new Rect(debugPos.x, debugPos.y + 50, 150, 20), "Best Lap Time: " + timeSpan2.ToString(@"mm\:ss\:ff"));
            GUI.Label(new Rect(debugPos.x, debugPos.y + 75, 120, 20), "Lap Time: " + timeSpan.ToString(@"mm\:ss\:ff"));

        }
    }
    [SerializeField] private float thresholdSpeed = 20f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            float speed = other.gameObject.GetComponent<CarController>().currentSpud;
            playerCar = other.gameObject;
            if (speed < thresholdSpeed && !raceStarted)
            {
                Debug.Log("Car near: " + other.gameObject.name);
                showUI(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            showUI(false);
        }
    }

    private void showUI(bool active)
    {
        firstCanvasObj.SetActive(active);
        return;
    }
}
