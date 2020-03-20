using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteBtn : MonoBehaviour
{
    bool mute = true;
    Text BtnTxt;
    GameObject SongController;
    void Start(){
        SongController = GameObject.FindGameObjectWithTag("music");
        mute = SongController.GetComponent<AudioSource>().volume == 0;
        BtnTxt = gameObject.transform.Find("Text").GetComponent<Text>();
        BtnTxt.text = mute ?  "♪ OFF" : "♪ ON"; 
    }

    public void Mute(){
        if (mute){
            SongController.GetComponent<AudioSource>().volume = 1;
            BtnTxt.text = "♪ ON";
        }
        if (!mute){
            SongController.GetComponent<AudioSource>().volume = 0;
            BtnTxt.text = "♪ OFF";
        }
        mute = !mute;
    }
}
