using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject ControlsMenu;
    public GameObject PauseMenuUI;
    [SerializeField] private GameObject settingsObject;
    [SerializeField] private Button settingsButton;
    private void Start()
    {
        PauseMenuUI.SetActive(false);
        settingsObject.SetActive(false);
        paused = false;
        settingsButton.onClick.AddListener(() =>
        {
            settingsObject.SetActive(!settingsObject.activeSelf);
        });
    }
    public void BackToMenu(int lvlindex)
    {
        SceneManager.LoadScene(lvlindex);
    }
    public void PauseResume()
    {
        paused = !paused;
    }

    public void QuitAp()
    {
        Debug.Log("Quit App");
        Application.Quit();
    }
    // Update is called once per frame
    public void Resume()
    {
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        Time.timeScale = 0f;
    }
}
