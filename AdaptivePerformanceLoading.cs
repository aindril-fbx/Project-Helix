using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AdaptivePerformanceLoading : MonoBehaviour {
    
    [SerializeField] private float delay = 10f;
    IEnumerator Start () {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(1);
    }
}