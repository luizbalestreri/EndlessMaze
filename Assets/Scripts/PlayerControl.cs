using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{   
    GameController gameController;
    ParticleSystem pS;
    WindZone wZ;
    float waitTime = 1;
    public float wzMain;
    void Start(){
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        pS = GameObject.Find("Particle System").GetComponent<ParticleSystem>(); 
        wZ= GameObject.Find("Wind").GetComponent<WindZone>();
        wZ.windMain = 0;
    }

    void Update(){
        var main = pS.main;
        main.simulationSpeed = gameController.GetSpeed()/3;
        wZ.windMain = wzMain;
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

    public IEnumerator WindDirection(int direction){
        wzMain = direction;
        yield return new WaitForSeconds (waitTime/gameController.GetSpeed());
        wzMain = 0;
        yield return null;
    }
}
