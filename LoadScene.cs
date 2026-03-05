using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;

    public void LoadLevel(int lvlIndex){
        StartCoroutine(LoadAsynchronously(lvlIndex));
    }

    IEnumerator LoadAsynchronously(int lvlIndex){
        AsyncOperation operation = SceneManager.LoadSceneAsync(lvlIndex);
        loadingScreen.SetActive(true);

        while(!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress/.9f);
            slider.value = progress;
            yield return null;
        }
    }
}
