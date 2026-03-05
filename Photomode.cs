using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Photomode : MonoBehaviour
{
    public PlayerControls controls;
    [SerializeField] private GameObject MainCam;
    private Camera cameraSettings;
    [SerializeField] private GameObject PhotoModeCam;
    [SerializeField] private GameObject postVolumeOBJ;
    private Volume PPVolume;
    [SerializeField] private GameObject pCanvas;
    [SerializeField] private GameObject MainCanvas_;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject framesParent;
    public bool photoMode_Enabled;
    [SerializeField] private GameObject[] frames;
    [SerializeField] private GameObject grid;

    [Header("Sliders")]
    [SerializeField] private Slider frameSlider;
    [SerializeField] private Slider satSlider;
    [SerializeField] private Slider contSlider;
    [SerializeField] private Slider DOFSlider;
    [SerializeField] private Slider TempSlider;
    [SerializeField] private Slider VigSlider;
    [SerializeField] private Slider FOVSlider;
    /*
    Saturation: 0
    Contrast: 1
    Depth of Field: 2
    Temperature: 3
    Vignette: 4
    */
    [SerializeField] private float[] defaultValues = new float[5];
    private ColorAdjustments colorAdjustments;
    private WhiteBalance whiteBalance;
    private DepthOfField depthOfField;
    private Vignette vignette;

    [SerializeField] private GameObject photoModeControlsUI;



    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControls(); 

        PhotoModeCam.SetActive(false);
        postVolumeOBJ.SetActive(false);
        framesParent.SetActive(false);

        controls.Gameplay.PhotoMode.performed += ctx => changeModeState();

        cameraSettings = PhotoModeCam.GetComponent<Camera>();
        PPVolume = postVolumeOBJ.GetComponent<Volume>();
        PPVolume.profile.TryGet(out colorAdjustments);
        PPVolume.profile.TryGet(out whiteBalance);
        PPVolume.profile.TryGet(out depthOfField);
        PPVolume.profile.TryGet(out vignette);
        defaultValues[0] = colorAdjustments.saturation.value;
        defaultValues[1] = colorAdjustments.contrast.value;
        defaultValues[2] = depthOfField.focusDistance.value;
        defaultValues[3] = whiteBalance.temperature.value;
        defaultValues[4] = vignette.intensity.value;

        colorAdjustments.saturation.value = defaultValues[0] + satSlider.value;
        colorAdjustments.contrast.value = defaultValues[1] + contSlider.value;
        depthOfField.focusDistance.value = 10f;
        whiteBalance.temperature.value = defaultValues[3] + TempSlider.value;
        vignette.intensity.value = defaultValues[4] + VigSlider.value;
    }

    private void OnEnable() {

        colorAdjustments.saturation.value = defaultValues[0] + satSlider.value;
        colorAdjustments.contrast.value = defaultValues[1] + contSlider.value;
        depthOfField.focusDistance.value = 10f;
        whiteBalance.temperature.value = defaultValues[3] + TempSlider.value;
        vignette.intensity.value = defaultValues[4] + VigSlider.value;

        cameraSettings = PhotoModeCam.GetComponent<Camera>();
        PPVolume = postVolumeOBJ.GetComponent<Volume>();
        PPVolume.profile.TryGet(out colorAdjustments);
        PPVolume.profile.TryGet(out whiteBalance);
        PPVolume.profile.TryGet(out depthOfField);
        PPVolume.profile.TryGet(out vignette);
        defaultValues[0] = colorAdjustments.saturation.value;
        defaultValues[1] = colorAdjustments.contrast.value;
        defaultValues[2] = depthOfField.focusDistance.value;
        defaultValues[3] = whiteBalance.temperature.value;
        defaultValues[4] = vignette.intensity.value;

        colorAdjustments.saturation.value = defaultValues[0] + satSlider.value;
        colorAdjustments.contrast.value = defaultValues[1] + contSlider.value;
        depthOfField.focusDistance.value = 10f;
        whiteBalance.temperature.value = defaultValues[3] + TempSlider.value;
        vignette.intensity.value = defaultValues[4] + VigSlider.value;

        controls.Gameplay.Enable();
    }

    public void ManualUpdate(){
        for(int i = 0; i < frames.Length; i++) {
            if(i == (int)frameSlider.value){
                frames[i].SetActive(true);
            }else{
                frames[i].SetActive(false);
            }
        }

        colorAdjustments.saturation.value = defaultValues[0] + satSlider.value;
        colorAdjustments.contrast.value = defaultValues[1] + contSlider.value;
        depthOfField.focusDistance.value = DOFSlider.value;
        whiteBalance.temperature.value = defaultValues[3] + TempSlider.value;
        vignette.intensity.value = defaultValues[4] + VigSlider.value;

        cameraSettings.fieldOfView = FOVSlider.value;
    }


    public void changeModeState(){
        photoMode_Enabled = !photoMode_Enabled;
        if(photoMode_Enabled){
            Time.timeScale = 0f;
            MainCam.SetActive(false);
            PhotoModeCam.SetActive(true);
            postVolumeOBJ.SetActive(true);
            pCanvas.SetActive(true);
            framesParent.SetActive(true);
            MainCanvas_.SetActive(false);
            PhotoModeCam.transform.position = MainCam.transform.position;
            PhotoModeCam.transform.LookAt(transform);
        }else{
            //Time.timeScale = 1f;
            MainCam.SetActive(true);
            PhotoModeCam.SetActive(false);
            postVolumeOBJ.SetActive(false); 
            pCanvas.SetActive(false);
            framesParent.SetActive(false);
            MainCanvas_.SetActive(true);
            colorAdjustments.saturation.value = defaultValues[0];
            colorAdjustments.contrast.value = defaultValues[1];
            depthOfField.focusDistance.value = defaultValues[2];
            whiteBalance.temperature.value = defaultValues[3];
            vignette.intensity.value = defaultValues[4];
        }
    }

    public void takePhoto(){

        photoModeControlsUI.SetActive(false);
        int imageWidth = Screen.width * 4;
        int imageHeight = Screen.height * 4;
        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 24,RenderTextureFormat.DefaultHDR);
        RenderTexture sdRt = new RenderTexture(imageWidth, imageHeight, 24,RenderTextureFormat.ARGB32);
        Camera ssCam = PhotoModeCam.GetComponent<Camera>(); 
        ssCam.targetTexture = rt;
        ssCam.Render();
        Graphics.Blit(rt,sdRt);

        Texture2D texture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGBAHalf, false);
        RenderTexture.active = sdRt;
        texture.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        rt.Release();
        sdRt.Release();
        Destroy(rt);
        Destroy(sdRt);
        Destroy(texture);

        string currentDate = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        SaveTextureAsPNG(texture,currentDate + "_Image.png");
        photoModeControlsUI.SetActive(true);
        
    }

    void SaveTextureAsPNG(Texture2D texture, string fileName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Screenshots");
        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        byte[] bytes = texture.EncodeToPNG();
        string path;
        if(Application.platform == RuntimePlatform.Android){
            NativeGallery.SaveImageToGallery(texture, "Helix", fileName);
            return;
            //path = Path.Combine("storage/emulated/0/DCIM" ,"Helix-Photo-Mode", fileName);
        }else{
            path = Path.Combine(folderPath, fileName);
        }
        File.WriteAllBytes(path, bytes);
        Debug.LogWarning($"Image saved at: {path}");
    }

    public void enableGrid(){
        grid.SetActive(!grid.activeSelf);
    }


    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
