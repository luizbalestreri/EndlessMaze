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
        if(!gameController.gameOver){main.simulationSpeed = gameController.speed/3;}
        wZ.windMain = wzMain;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag != "trap")
            gameController.canTurn = true;
    }

    private void OnTriggerExit2D(Collider2D other){
        other.GetComponent<Collider2D>().enabled = false;
        if(gameController.canTurn && gameController.trap == 0){
            gameController.GameOver();
        } else if (other.tag == "trap"){
            gameController.GameOver();
        }
        gameController.canTurn = false;
    }

    public IEnumerator WindDirection(int direction){
        wzMain = direction;
        yield return new WaitForSeconds (waitTime/gameController.speed);
        wzMain = 0;
        yield return null;
    }
}
