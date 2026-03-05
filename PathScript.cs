using UnityEngine;

public class PathScript : MonoBehaviour {
    public Transform[] path;
    [SerializeField] private Color rayColor;

    private void OnDrawGizmos() {
        Gizmos.color = rayColor;
        Transform[] path_objs = transform.GetComponentsInChildren<Transform>();
        path = new Transform[path_objs.Length-1];
        for(int i = 1; i < path_objs.Length; i++) {
            if(path_objs[i] != this.gameObject.transform){
                path[i-1] = path_objs[i];
            }
        }

        for(int i = 0; i < path.Length; i++) {
            Vector3 pos = path[i].position;
            if(i>0){
                Vector3 prevPos = path[i-1].position;
                Gizmos.DrawLine(prevPos,pos);
            }
            Gizmos.DrawWireSphere(path[i].position,0.3f);
        }
    }
}