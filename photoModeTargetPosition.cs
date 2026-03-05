using UnityEngine;
using UnityEngine.InputSystem;

public class photoModeTargetPosition : MonoBehaviour {
    
    public PlayerControls controls;
    [SerializeField] private Vector2 inputDirection;
    [SerializeField] private Transform cameraT;
    [SerializeField] private float moveSpeed = 1000f;


    private void Awake() {
        controls = new PlayerControls();
        controls.Gameplay.Accelarate.performed += ctx => inputDirection.y = ctx.ReadValue<float>();
        controls.Gameplay.Accelarate.canceled += ctx => inputDirection.y = 0f;
        controls.Gameplay.Steer.performed += ctx => inputDirection.x = ctx.ReadValue<float>();
        controls.Gameplay.Steer.canceled += ctx => inputDirection.x = 0f;
    }

    private void Update() {
        //transform.rotation = cameraT.rotation;
        //transform.LookAt(cameraT);
        transform.Translate(cameraT.InverseTransformVector(Vector3.forward) * inputDirection.y * moveSpeed * Time.unscaledDeltaTime);
        transform.Translate(cameraT.InverseTransformVector(Vector3.right) * inputDirection.x * moveSpeed * Time.unscaledDeltaTime);
    }

    private void OnEnable() {
        controls.Gameplay.Enable();
    }

    private void OnDisable() {
        controls.Gameplay.Disable();
    }
}