using UnityEngine;

public class checkpointBehavior : MonoBehaviour
{

    [SerializeField] private GameObject targetObj;
    public Vector3 worldPos;

    private void OnEnable()
    {
        worldPos = this.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            gameObject.transform.root.gameObject.GetComponent<raceEvent>().changeActiveCheckpoint(int.Parse(gameObject.name), other.gameObject.GetComponent<waypointSetter>());
            changeState();
        }
    }

    public void changeState(bool activeState = false)
    {
        targetObj.SetActive(activeState);
    }
}
