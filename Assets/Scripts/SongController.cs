using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongController : MonoBehaviour
{
    Text txtMute;
    void Awake(){
        GameObject btnGO = GameObject.FindGameObjectWithTag("txtMute");
        Button btn = btnGO.GetComponent<Button>();
        txtMute = btnGO.transform.Find("Text").GetComponent<Text>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("music");
        if (objs.Length > 1){
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
