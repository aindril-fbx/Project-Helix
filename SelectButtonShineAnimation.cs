using UnityEngine;

public class SelectButtonShineAnimation : MonoBehaviour {
    
    [SerializeField] private Vector2 offPosition = new Vector2(-280f, 0f);
    [SerializeField] private Vector2 onPosition = new Vector2(280f,0f);
    [SerializeField] private float duration = 2f;
    private void Start() {
        offPosition = transform.position;
        anim();
    }

    void anim(){
        this.transform.LeanMoveLocal(onPosition,duration).setEaseInQuart().setOnComplete(complete).delay = 2f;
    }

    void complete(){
        transform.position = offPosition;
        anim();
    }
}