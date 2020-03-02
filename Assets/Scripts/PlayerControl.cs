using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{   
    GameController gameController;
    void Start(){
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Update(){
        
    }

    private void OnTriggerEnter(Collider other) {
        gameController.canTurn = true;
        Debug.Log("enter");
    }

    private void OnTriggerExit(Collider other){
        other.GetComponent<Collider>().enabled = false;
        Debug.Log("exit");
        if(gameController.canTurn){
            gameController.GameOver();
        }
    }
}
