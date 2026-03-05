using UnityEngine;

public class MiniMapCam : MonoBehaviour {
    
    [SerializeField] private GameObject parent_Object;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private float cameraTilt = 15f;
    private void Start() {
        transform.parent = null;
    }

    private void LateUpdate() {
        transform.position = parent_Object.transform.position + Offset;
        transform.eulerAngles = new Vector3(parent_Object.transform.eulerAngles.x - cameraTilt,parent_Object.transform.eulerAngles.y,0f);
    }

    private bool originalFogState;
    private Color originalFogColor;
    private FogMode originalFogMode;
    private float originalFogDensity;
    private float originalFogStartDistance;
    private float originalFogEndDistance;

    void OnPreRender()
    {
        // Save original fog settings
        originalFogState = RenderSettings.fog;
        originalFogColor = RenderSettings.fogColor;
        originalFogMode = RenderSettings.fogMode;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogStartDistance = RenderSettings.fogStartDistance;
        originalFogEndDistance = RenderSettings.fogEndDistance;

        // Disable fog for this camera
        RenderSettings.fog = false;
    }
}
