using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GhostRecorder : MonoBehaviour {
    public Ghost ghost;
    private float timer;
    private float timeValue;

    private InputManager IM;
    private void Awake() {
        StartRecording();
    }

    public void StartRecording() {
        if(ghost.isRecord){
            ghost.ResetData();
            timeValue = 0f;
            timer = 0f;
        }
    }

    private void Start() {
        IM = GetComponent<InputManager>();
    }

    private void Update() {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        if(ghost.isRecord && timer >= 1/ghost.recordFrequency){
            ghost.timeStamp.Add(timeValue);
            ghost.throttle.Add(IM.Throttle);
            ghost.steering.Add(IM.Steer);
            ghost.handBrake.Add(IM.HandBrake);
            ghost.position.Add(this.gameObject.transform.position);
            ghost.rotation.Add(this.gameObject.transform.rotation);
            timer = 0f;
        }
    }
}