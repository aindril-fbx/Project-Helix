using UnityEngine;
using UnityEngine.InputSystem;

public class PhotomodeCameraMovement : MonoBehaviour {

    public PlayerControls controls;
    [SerializeField] private Vector2 inputDirection;
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float UPDOWNinput;

    float yaw;
    float pitch;
    [SerializeField] private FixedTouchField FTF;

    private void Awake() {
        controls = new PlayerControls();
        controls.Gameplay.Accelarate.performed += ctx => inputDirection.y = ctx.ReadValue<float>();
        controls.Gameplay.Accelarate.canceled += ctx => inputDirection.y = 0f;
        controls.Gameplay.Steer.performed += ctx => inputDirection.x = ctx.ReadValue<float>();
        controls.Gameplay.Steer.canceled += ctx => inputDirection.x = 0f;
        controls.Gameplay.UPDOWN.performed += ctx => UPDOWNinput = ctx.ReadValue<float>();
        controls.Gameplay.UPDOWN.canceled += ctx => UPDOWNinput = 0f;
    }

    private void Update() {
        transform.Translate(Vector3.forward * inputDirection.y * moveSpeed * Time.unscaledDeltaTime);
        transform.Translate(Vector3.right * inputDirection.x * moveSpeed * Time.unscaledDeltaTime);
        transform.Translate(Vector3.up * UPDOWNinput * -moveSpeed * Time.unscaledDeltaTime);

        lookDirection = FTF.TouchDist * 0.1f;

        yaw += moveSpeed * Time.unscaledDeltaTime * 10f * lookDirection.x;
        pitch -= moveSpeed * Time.unscaledDeltaTime * 10f * lookDirection.y;

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    private void OnEnable() {
        controls.Gameplay.Enable();
    }

    private void OnDisable() {
        controls.Gameplay.Disable();
    }
}