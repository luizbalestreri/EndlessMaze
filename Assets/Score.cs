using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public GameController gameController;
    void Update(){
       gameObject.GetComponent<Text>().text = gameController.score.ToString(); 
    }
}
