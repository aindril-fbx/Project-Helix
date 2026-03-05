using UnityEngine;

public class lightBlink : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 1f;
    [SerializeField] private Light light;
    [SerializeField] private float intensity = 3000f;

    private void Start() {
        light = GetComponent<Light>();
    }
    private void Update()
    {
        light.intensity = Mathf.PingPong(Time.time * intensity * blinkSpeed, intensity);
    }
}