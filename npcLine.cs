using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class npcLine : MonoBehaviour
{
    [SerializeField] private GameObject chatParent;
    [SerializeField] private string[] chatLines;

    [SerializeField] private float duration = 2f;

    void Start()
    {
        chatParent.SetActive(false);
    }

    

}