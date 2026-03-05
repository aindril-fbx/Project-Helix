using UnityEngine;

public class SparkEmit : MonoBehaviour {
    
    [SerializeField] private ParticleSystem spark;
    private void OnEnable(){
        spark.Play();
        Invoke("Destroy",2f);
    }
    void Destroy(){
        Destroy(gameObject);
    }
}