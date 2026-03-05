using UnityEngine;

public class GhostPlayer : MonoBehaviour {
    public Ghost ghost;
    private float timeValue;
    private int index1;
    private int index2;

    [SerializeField] private GhostCarController GCC;

    private void Awake() {
        timeValue = 0f;
    }

    private void Update() {
        timeValue += Time.unscaledDeltaTime;
        if(ghost.isReplay){
            GetIndex();
            SetValues();
        }
    }

    void GetIndex(){
        for(int i = 0; i < ghost.timeStamp.Count - 2; i++) {
            if(ghost.timeStamp[i] == timeValue){
                index1 = i;
                index2 = i;
                return;
            }else if(ghost.timeStamp[i] < timeValue & timeValue < ghost.timeStamp[i + 1])
            {
                index1 = i;
                index2 = i + 1;
                return;
            }
        }

        index1 = ghost.timeStamp.Count - 1;
        index2 = ghost.timeStamp.Count - 1;
    }

    void SetValues(){
        if(index1 == index2){
            GCC.Throttle = ghost.throttle[index1];
            GCC.Steer = ghost.steering[index1];
            GCC.HandBrake = ghost.handBrake[index1];
        }else{
            float interpolationFactor = (timeValue-ghost.timeStamp[index1])/(ghost.timeStamp[index2] - ghost.timeStamp[index1]);

            GCC.Throttle = Mathf.Lerp(ghost.throttle[index1],ghost.throttle[index2],interpolationFactor);
            GCC.Steer = Mathf.Lerp(ghost.steering[index1],ghost.steering[index2],interpolationFactor);
            GCC.HandBrake = ghost.handBrake[index1];
        }
    }
}