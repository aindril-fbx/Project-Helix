using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    //public GameObject loadingScreen;
    //public Slider slider;
    public GameObject MainMenuParent;
    public GameObject CarSelectionParent;
    [SerializeField] private TextMeshProUGUI moneyDisplay;

    public void SelectCar(){
        //StartCoroutine(LoadAsynchronously(lvlIndex)); 
        MainMenuParent.SetActive(false);
        CarSelectionParent.SetActive(true);
    }
    public void Back(){
        MainMenuParent.SetActive(true);
        CarSelectionParent.SetActive(false);
    }
    public void QuitAp(){
        Application.Quit();
    }

    private void Start() {
        moneyDisplay.text = playerStats.instance.playerMoney_().ToString("n0");
        //Handheld.Vibrate();
    }
    /*
    IEnumerator LoadAsynchronously(int lvlIndex){
        AsyncOperation operation = SceneManager.LoadSceneAsync(lvlIndex);
        loadingScreen.SetActive(true);

        while(!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress/.9f);
            slider.value = progress;
            yield return null;
        }
    }
    */

    
}
