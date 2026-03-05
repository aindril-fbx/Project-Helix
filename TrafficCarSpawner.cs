using UnityEngine;

public class TrafficCarSpawner : MonoBehaviour {
    [SerializeField] private GameObject[] trafficCars;

    float value;
    [SerializeField] private float interval = 10f;
    bool carNear = false;
    GameObject car;

    [SerializeField] private bool loop = false;

    private void Update() {
        if(value < interval ){
            value += Time.deltaTime;
        }else{
            value = 0f;
            if(!carNear && car == null){
                spawnCar();
            }
        }
    }

    async void spawnCar(){
        int i = (int)Random.Range(0,trafficCars.Length);
        car = Instantiate(trafficCars[i],transform.position,transform.rotation);
        car.GetComponent<CarPathFollower>().PS = this.gameObject.GetComponent<PathScript>();
        car.GetComponent<CarPathFollower>().loop = loop;
        interval = Random.Range(30,80);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Car"){
            carNear = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Car"){
            carNear = false;
        }
    }
}