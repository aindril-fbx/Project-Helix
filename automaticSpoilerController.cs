using UnityEngine;

public class automaticSpoilerController : MonoBehaviour
{
    [SerializeField] private Animator SpoilerAnim;
    [SerializeField] private float threadhold = 60f;

    float deployCoeff = 0f;
    float brakeCoeff = 0f;
    public float speed = 1f;

    [SerializeField] private Rigidbody rb;

    private void FixedUpdate() {
        float currentSpud = Mathf.Round(rb.linearVelocity.magnitude * 1.8f);
        if(currentSpud > threadhold){
            deployCoeff = Mathf.Lerp(deployCoeff,0f,speed);
        }else{
            deployCoeff = Mathf.Lerp(deployCoeff,1f,speed);
        }

        brakeCoeff = Mathf.Lerp(brakeCoeff,0f,speed);

        SpoilerAnim.SetFloat("Deploy",deployCoeff);
        SpoilerAnim.SetFloat("Brake",brakeCoeff);
    }
}