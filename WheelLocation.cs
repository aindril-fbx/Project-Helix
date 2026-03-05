using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelLocation : MonoBehaviour
{
    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
    [SerializeField] private Rigidbody rb;

    public bool showcase = false;
    
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        float currentSpud = Mathf.Round((Mathf.Abs(rb.linearVelocity.magnitude)));
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;
            m_WheelColliders[i].GetWorldPose(out position, out quat);
            m_WheelMeshes[i].transform.position = position;
            m_WheelMeshes[i].transform.rotation = quat;
            if(showcase){
                m_WheelMeshes[i].transform.Rotate(0f,10f,0f);
            }
           
        }
    }
    float x = 1f;
    private void LateUpdate() {
        x = (x+0.5f)%360f;
        if(showcase){
            for(int i = 0; i < m_WheelMeshes.Length; i++) {
                m_WheelMeshes[i].transform.Rotate(x,0f,0f);
            }
        }else{
            return;
        }
    }

}
