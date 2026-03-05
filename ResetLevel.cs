using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour
{
    [SerializeField] private LoadingScreenV2 LS;

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Car"){
            ResetKrow();
        }
    }

    public void ResetKrow(){
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        LS.transform.gameObject.SetActive(true);
        LS.SetSceneIndex(SceneManager.GetActiveScene().buildIndex);
        Debug.Log($"Reset");
    }
}
