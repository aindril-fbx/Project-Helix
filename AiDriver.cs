using UnityEngine;

public class AiDriver : MonoBehaviour {
    public Transform targetPositionTransform;
    [SerializeField] private AiCarController AIcar;
    [SerializeField] private float steerDot; 
    [SerializeField] private float accelDot;
    [SerializeField] private float reverseDistance;
    [SerializeField] private float brakeDistance;
    float distanceToTarget;

    [SerializeField] private bool policeCar;

    [SerializeField] private float damping = 0.4f;
    public CarController CC;
    [SerializeField] private Transform[] rays;
    [SerializeField] private LayerMask carLayer;

    public bool travel;

    private void Start() {
        AIcar = GetComponent<AiCarController>();
    }

    private void FixedUpdate() {
        if(targetPositionTransform != null) {
            Vector3 directionVector = (targetPositionTransform.position - transform.position).normalized;
            steerDot = Vector3.Dot(directionVector, transform.right);
            accelDot = Vector3.Dot(directionVector, transform.forward);
            distanceToTarget = Vector3.Distance(targetPositionTransform.position,transform.position);
        }

        if(travel){
            if(distanceToTarget < AIcar.currentSpud && AIcar.currentSpud > 5f && targetPositionTransform.gameObject.GetComponent<CarController>().currentSpud < 50f){
                AIcar.Throttle = -1f * Mathf.Sign(accelDot);
            }else{
                if(distanceToTarget < 10f){
                    AIcar.Throttle = 0f;
                }else{
                    if(accelDot > 0f && distanceToTarget > reverseDistance){
                        AIcar.Throttle = Mathf.Lerp(AIcar.Throttle,1f,damping);
                    }else{
                        AIcar.Throttle = Mathf.Lerp(AIcar.Throttle,-1f,damping);
                    }     
                }
            }
            
            if(distanceToTarget > 30f && accelDot > 0f){
                AIcar.Steer = steerDot;
            }else{
                AIcar.Steer = Mathf.Clamp(steerDot * distanceToTarget / 10f,-1f,1f);
            }

            if(distanceToTarget < brakeDistance){
                AIcar.HandBrake = true;
            }else{
                AIcar.HandBrake = false;
            }
        }else{
            AIcar.Throttle = 0f;
            AIcar.Steer = 0f;
        }

        if(CC != null && policeCar){
            if(CC.Revs > 0.7f){
                targetPositionTransform = CC.gameObject.transform;
                travel = true;
            }
        }

        RaycastHit hit;
        for(int i = 0; i < rays.Length; i++) {
            if(Physics.Raycast(rays[i].position,rays[i].forward, out hit,15f)){
                if(hit.transform.gameObject.layer != 6){
                    RayHitBehavior(i);
                    Debug.DrawRay(rays[i].position,rays[i].forward * hit.distance,Color.green);
                }
            }
        }
    }

    void RayHitBehavior(int x){
        switch(x){
            case 0:
                Debug.Log("Right");
                AIcar.Steer = 0.5f;
                break;
            case 1:
                Debug.Log("Hard Right");
                AIcar.Steer = 1f;
                break;
            case 2:
                Debug.Log("Hard Left");
                AIcar.Steer = -1f;
                break;
            case 3:
                Debug.Log("Left");
                AIcar.Steer = -0.5f;
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Car"){
            CC = other.gameObject.GetComponent<CarController>();
        }
    }
}