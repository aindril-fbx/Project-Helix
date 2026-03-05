using UnityEngine;

public class RPMController : MonoBehaviour
{
    public float maxRPM = 7000f;
    public float minRPM = 1000f;
    public float maxSpeed = 200f;
    public float minSpeed = 20f;
    public float[] gearRatios = { 4.0f, 2.8f, 2.0f, 1.5f, 1.0f }; // Gear ratios for each gear
    public float engineTorque = 500f;
    public float maxThrottle = 1.0f;
    public AnimationCurve torqueCurve;
    public float inertia = 0.2f; // Engine inertia

    private float currentRPM;
    private int currentGear = 1;
    private float throttleInput;
    private float currentSpeed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        currentSpeed = rb.linearVelocity.magnitude * 2.237f; // Convert m/s to mph

        // Calculate current RPM based on gear ratio and current speed
        float gearRatio = gearRatios[currentGear - 1];
        float engineSpeed = currentSpeed / gearRatio;
        float targetRPM = Mathf.Lerp(minRPM, maxRPM, torqueCurve.Evaluate(engineSpeed / maxSpeed));
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, inertia);

        // Automatic gear shifting based on RPM
        if (currentRPM >= maxRPM && currentGear < gearRatios.Length)
        {
            currentGear++;
        }
        else if (currentRPM <= minRPM && currentGear > 1)
        {
            currentGear--;
        }

        // Apply engine torque to wheels
        float torque = CalculateTorque(currentRPM);
        float power = torque * (2.0f * Mathf.PI / 60.0f);
        float wheelTorque = power / (0.5f * rb.mass * 9.81f);

    }

    float CalculateTorque(float rpm)
    {
        // Calculate torque using torque curve
        float normalizedRPM = Mathf.InverseLerp(minRPM, maxRPM, rpm);
        float normalizedTorque = torqueCurve.Evaluate(normalizedRPM);
        return normalizedTorque * engineTorque;
    }

    public void SetThrottle(float throttle)
    {
        // Set throttle input
        throttleInput = Mathf.Clamp(throttle, 0f, maxThrottle);
    }

    public int GetCurrentGear()
    {
        return currentGear;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    private void OnGUI () {
        float pos = 100;
        GUI.VerticalSlider(new Rect(10, pos, 100, 10), currentRPM , -minRPM , maxRPM );
    }
}
