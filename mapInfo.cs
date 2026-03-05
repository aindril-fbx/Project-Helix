using UnityEngine;

[CreateAssetMenu(fileName = "mapInfo", menuName = "mapInfo", order = 0)]
public class mapInfo : ScriptableObject {
    public string M_Name;
    public Material preview;
    public int sceneIndex;
}