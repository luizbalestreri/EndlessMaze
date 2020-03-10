using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongController : MonoBehaviour
{
    Text txtMute;
    void Awake(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("music");
        GameObject btnGO = GameObject.FindGameObjectWithTag("txtMute");
        Button btn = btnGO.GetComponent<Button>();
        txtMute = btnGO.transform.Find("Text").GetComponent<Text>();
        if (objs.Length > 1){
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    
    bool mute = true;
    public void Mute(){
        if (mute){
            gameObject.GetComponent<AudioSource>().volume = 1;
            txtMute.text = "♪ ON";
        }
        if (!mute){
            gameObject.GetComponent<AudioSource>().volume = 0;
            txtMute.text = "♪ OFF";
        }
        mute = !mute;

    }
}
