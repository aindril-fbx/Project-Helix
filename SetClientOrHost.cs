using UnityEngine;

public class SetClientOrHost : MonoBehaviour {
    
    public void SetClient () {
        PlayerPrefs.SetInt("HostorClient",1);
    }

    public void SetHost () {
        PlayerPrefs.SetInt("HostorClient",0);
    }
}