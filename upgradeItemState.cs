using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class upgradeItemState : MonoBehaviour {
    public enum ItemState{
        NotOwned,
        Owned,
        Installed
    };
    [SerializeField] private GameObject ownedIndicator;
    [SerializeField] private GameObject installedIndicator;
    [SerializeField] private TextMeshProUGUI priceDisplay;
    [SerializeField] private float price = 0f;
    public ItemState itemState = ItemState.NotOwned;
    public bool selected = false;

    [SerializeField] private Transform enableEffect;
    [SerializeField] private Vector2 offPosition = new Vector2(-280f, 0f);
    [SerializeField] private Vector2 onPosition = new Vector2(280f,0f);


    private void Start() {
        if(priceDisplay != null) {
            priceDisplay.text = price + "";
        }
    }
    public int getPrice(){
        return (int)price;
    }
    public void changeItemState(){
        if(itemState == ItemState.NotOwned){
            ownedIndicator.SetActive(true);
            installedIndicator.SetActive(false);
            itemState = ItemState.Owned;

            enableEffect.LeanMoveLocal(onPosition,0.7f).setEaseInQuart().setOnComplete(OnComplete);
            
        }else if(itemState == ItemState.Owned){
            ownedIndicator.SetActive(false);
            installedIndicator.SetActive(true);
            itemState = ItemState.Installed;
        }
    }

    void OnComplete(){
        enableEffect.transform.position = offPosition;
    }

    public void SetStateToInstalled(){
        itemState = ItemState.Installed;
        ownedIndicator.SetActive(false);
        installedIndicator.SetActive(true);
    }

    public void SetStateToOwned(){
        itemState = ItemState.Owned;
        ownedIndicator.SetActive(true);
        installedIndicator.SetActive(false);
    }
}