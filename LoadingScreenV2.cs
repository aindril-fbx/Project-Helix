using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenV2 : MonoBehaviour
{
    [SerializeField] private Transform LeftHalf;
    [SerializeField] private Transform RightHalf;
    [SerializeField] private Vector2 LCenterPosition;
    [SerializeField] private Vector2 RCenterPosition;
    [SerializeField] private Vector2 LOffSetPosition;
    [SerializeField] private Vector2 ROffSetPosition;
    [SerializeField] private float Duration = 1f;

    [Header("Load Scene too")]
    [SerializeField] private bool LoadScene;
    [SerializeField] private Slider slider;
    int SceneIndex;

    [SerializeField] private mapInfo[] MapList;
    [SerializeField] private GameObject WelcomeLabel;
    [SerializeField] private TextMeshProUGUI LocationLabel;
    [SerializeField] private Animation welcomeAnim;

    public void SetSceneIndex(int s){
        SceneIndex = s;
        slider.value = 0f;
        LoadScene = true;
    }

    private void Start() {
        //LeftHalf.position = LCenterPosition;
        //RightHalf.position = RCenterPosition;
        WelcomeLabel.transform.parent = null;
        LeftHalf.LeanMoveLocal(LOffSetPosition,Duration).setEaseInQuart().setOnComplete(OnComplete).delay = 0.5f;
        RightHalf.LeanMoveLocal(ROffSetPosition,Duration).setEaseInQuart().setOnComplete(OnComplete).delay = 0.5f;

        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.dspBufferSize = 4096;
    }
    private void OnEnable() {
        LeftHalf.LeanMoveLocal(LCenterPosition,Duration).setEaseInQuart();
        RightHalf.LeanMoveLocal(RCenterPosition,Duration).setEaseInQuart().setOnComplete(LoadLevel);
    }

    public void OnComplete() {
        if(MapList[SceneManager.GetActiveScene().buildIndex] != null){
            WelcomeLabel.SetActive(true);
            welcomeAnim.Play();
            LocationLabel.text = MapList[SceneManager.GetActiveScene().buildIndex].M_Name;
        }
        gameObject.SetActive(false);
    }
    public void LoadLevel(){
        if(LoadScene){
            StartCoroutine(LoadAsynchronously(SceneIndex));
        }
    }

    IEnumerator LoadAsynchronously(int lvlIndex){
        AsyncOperation operation = SceneManager.LoadSceneAsync(lvlIndex);

        while(!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress/.9f);
            slider.value = progress;
            yield return null;
        }
    }
}
