using UnityEngine;

public class CarFollower : MonoBehaviour
{
    [SerializeField] private GameObject CarHolder;
    [SerializeField] private GameObject Car;
   /// <summary>
   /// Awake is called when the script instance is being loaded.
   /// </summary>
   private void Awake()
   {
          Invoke("SetUp",0.1f);
   }
   
   private void SetUp () {
       CarHolder = GameObject.FindWithTag("CarHolder");
       Car = CarHolder.transform.GetChild(0).gameObject;
       
       transform.parent = Car.transform;
       transform.position = Car.transform.position;
   }

}
