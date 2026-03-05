using UnityEngine;

public class destroyWIthTime : MonoBehaviour {

    [SerializeField] private float destroyTime = 1f;
    private void OnEnable() {
        Invoke("DestroyKrow",destroyTime);
    }

    void DestroyKrow(){
        Destroy(gameObject);
    }
}