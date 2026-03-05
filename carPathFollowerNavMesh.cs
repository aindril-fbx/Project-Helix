using UnityEngine;
using UnityEngine.AI;

public class carPathFollowerNavMesh : MonoBehaviour
{
    [SerializeField] public Vector3[] path;
    public Transform targetPosition;
    public Transform FinalPosition;
    [SerializeField] private float maxThrottleValue = 1f;
    [SerializeField] private float steerDot;
    [SerializeField] private float accelDot;

    public bool travel = true;
    [SerializeField] private float distanceBias = 15f; // distance from target to change target

    [SerializeField] private Transform[] rays;

    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [SerializeField] private float motorForce = 100f;
    [SerializeField] private float steerAngle = 30f;
    [SerializeField] private bool pathEnded = false;
    [SerializeField] private bool carNear = false;
    private Rigidbody rb;
    public float currentSpud;
    public float speedLimit;
    public float tempSpeedLimit;
    GameObject Car;

    public bool loop = false;

    [SerializeField] private bool racer = false;
    [SerializeField] private float boostRatio = 0f;

    [Header("Audio")]
    [SerializeField] private AudioSource honkSound;
    Vector3 startPos, endPos;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(FinalPosition == null) return;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit startHit, 50f, NavMesh.AllAreas) &&
            NavMesh.SamplePosition(FinalPosition.position, out NavMeshHit endHit, 50f, NavMesh.AllAreas))
        {
            startPos = startHit.position;
            endPos = endHit.position;
            NavMeshPath pathArray = new NavMeshPath();
            NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, pathArray);

            path = pathArray.corners;
            if(targetPosition == null) {
                Transform tempT = new GameObject("Target").transform;
                tempT.position = path[1];
                targetPosition = tempT;
            }
        }

    }
    float distanceFromTarget;
    float t = 0f;
    bool rayHit = false;
    bool correctRotation = true;
    Vector3 offsetPosition = new Vector3(0f, 5f, 0f);
    private void FixedUpdate()
    {
        
        currentSpud = Mathf.Round((Mathf.Abs(rb.linearVelocity.magnitude) * 1.55f));
        if (Car != null)
        {
            if (Vector3.Distance(transform.position, Car.transform.position) > 200f)
            {
                carNear = false;
                correctRotation = true;
            }
        }

        float distance = Vector3.Distance(targetPosition.position, transform.position);
        if (distance < Mathf.Clamp(distanceBias * currentSpud / tempSpeedLimit, 0f, distanceBias))
        {
            t = 0f;
            if (loop && indexOfTransform(targetPosition) == path.Length - 2)
            {
                targetPosition.position = path[0];
            }
            else
            {
                if (indexOfTransform(targetPosition) != path.Length - 2)
                {
                    targetPosition.position = path[indexOfTransform(targetPosition) + 1];
                }
                else
                {
                    pathEnded = true;
                    travel = false;
                    handBrake(true);

                }
            }
        }
        else
        {
            t += Time.deltaTime;
        }
        float dotBetween = Vector3.Dot((path[indexOfTransform(targetPosition) + 1] - targetPosition.position).normalized, transform.forward);
        if (dotBetween < 0.5f)
        {
            rb.AddForce(rb.linearVelocity * -10f);
        }
        tempSpeedLimit = Mathf.Clamp(dotBetween * speedLimit, 15f, 100f);
        if (pathEnded && !carNear)
        {
            Destroy(this.gameObject);
        }
        Vector3 directionVector = (targetPosition.position - transform.position).normalized;
        accelDot = Vector3.Dot(directionVector, transform.forward);
        steerDot = Vector3.Dot(directionVector, transform.right);


        if (targetPosition != null && travel)
        {
            applyTorque(accelDot);
            if (!rayHit) steerWheels(steerDot);
        }
        else
        {
            handBrake(true);
        }

        RaycastHit hit;
        rayHit = false;
        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i].position, rays[i].forward, out hit, 20f))
            {
                if (hit.transform.gameObject.layer != 7 && hit.transform.gameObject.layer != 12 && hit.transform.gameObject.layer != 0)
                {
                    RayHitBehavior(i);
                    rayHit = true;
                    Debug.DrawRay(rays[i].position, rays[i].forward * hit.distance, Color.green);
                }
            }
        }
        SteerHelper();
        if (racer)
        {
            if (currentSpud > tempSpeedLimit)
            {
                rb.AddForce((targetPosition.position - transform.position) * 200f);
            }
            downforce(dotBetween);
        }

        bool reverse = Vector3.Dot(transform.forward, directionVector) < 0f;

    }

    void downforce(float x)
    {
        rb.AddForce(-transform.up * Mathf.Clamp(50f * currentSpud * currentSpud * (1f - Mathf.Abs(x)), 0f, 20000f));
        //Debug.Log(-transform.up * 50f * currentSpud * currentSpud * (1f - Mathf.Abs(x)));
    }

    void applyTorque(float Throttle)
    {
        rb.AddForce(transform.forward * 1000f * Mathf.Clamp(tempSpeedLimit - currentSpud, -1f, 1f) * boostRatio);
        if (currentSpud < tempSpeedLimit)
        {
            foreach (WheelCollider w in m_WheelColliders)
            {
                w.brakeTorque = 0f;
                w.motorTorque = motorForce * 5f * Throttle * maxThrottleValue;
            }
        }
        else
        {
            handBrake(true && !racer);
            foreach (WheelCollider w in m_WheelColliders)
            {
                w.brakeTorque = motorForce * 10f * Mathf.Abs(Throttle) * maxThrottleValue;
            }

        }
    }

    void steerWheels(float steer)
    {
        m_WheelColliders[0].steerAngle = Mathf.Lerp(m_WheelColliders[0].steerAngle, steerAngle * steer, 0.1f);
        m_WheelColliders[1].steerAngle = Mathf.Lerp(m_WheelColliders[1].steerAngle, steerAngle * steer, 0.1f);
    }

    void handBrake(bool brake)
    {
        if (brake)
        {
            m_WheelColliders[0].brakeTorque = 100000f;
            m_WheelColliders[1].brakeTorque = 100000f;
        }
        else
        {
            m_WheelColliders[0].brakeTorque = 0f;
            m_WheelColliders[1].brakeTorque = 0f;
        }
    }

    void RayHitBehavior(int x)
    {
        switch (x)
        {
            case 0:
                steerWheels(steerDot + 0.2f);
                break;
            case 1:
                handBrake(true && !racer);
                if (!honkSound.isPlaying && !racer) honkSound.Play();
                steerWheels(steerDot + 0.7f);
                break;
            case 2:
                handBrake(true && !racer);
                if (!honkSound.isPlaying && !racer) honkSound.Play();
                steerWheels(steerDot - 0.7f);
                break;
            case 3:
                steerWheels(steerDot - 0.2f);
                break;
        }
    }

    int indexOfTransform(Transform transform)
    {
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] == transform.position)
            {
                return i;
            }
        }
        return -1;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!honkSound.isPlaying & !racer) honkSound.Play();
        //pathEnded = true;
        //travel = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            carNear = true;
            t = 0f;
            Car = other.gameObject;
            Debug.Log("TriggerEnter");
        }
    }

    float m_OldRotation;
    [SerializeField] private float m_SteerHelper = 0.7f;
    [SerializeField] private float steerHelpCoe = 0.5f;


    private void SteerHelper()
    {

        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper * steerHelpCoe;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rb.linearVelocity = velRotation * rb.linearVelocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(path.Length > 0) {
            for(int i = 0; i < path.Length; i++) {
            Vector3 pos = path[i];
            if(i>0){
                Vector3 prevPos = path[i-1];
                Gizmos.DrawLine(prevPos,pos);
            }
            Gizmos.DrawWireSphere(path[i],0.3f);
        }
        }
    }
}