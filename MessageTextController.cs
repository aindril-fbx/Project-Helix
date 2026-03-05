using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MessageTextController : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI label_;
    [SerializeField] private List<string> msgs;
    bool on = false;
    [SerializeField] private AudioSource msgSound;
    [SerializeField] private TextMeshProUGUI counter_;
    
    private void OnEnable() {
        Invoke("OnComplete",5f);
    }

    public void setText(string text_){
        msgs.Add(text_);
        counter_.text = msgs.Count + "";
        counter_.gameObject.transform.parent.gameObject.SetActive(true);
        if(!on){
            StartCoroutine(Display());
            on = true;
        }
        //label_.text = text_;
    }

    IEnumerator Display(){
        while(msgs.Count > 0) {
            counter_.gameObject.transform.parent.gameObject.SetActive(true);
            this.transform.gameObject.SetActive(true);
            if(!msgSound.isPlaying){
                msgSound.Play();
            }
            label_.text = msgs[0];
            msgs.RemoveAt(0);
            counter_.text = msgs.Count + "";
            if(msgs.Count == 0){
                counter_.gameObject.transform.parent.gameObject.SetActive(false);
            }
            if(msgs.Count == 0){
                yield return new WaitForSeconds(3f);
            }else{
                yield return new WaitForSeconds(1f);
            }
        }
        on = false;
        OnComplete();
    }

    public void OnComplete() {
        if(msgs.Count == 0){
            this.transform.gameObject.SetActive(false);
            on = false;
        }
    }

    private void LateUpdate() {
        if(Time.timeScale < 0.1f){
            this.transform.gameObject.SetActive(false);
        }
    }
}