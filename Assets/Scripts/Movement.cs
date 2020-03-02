using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour{
    float speed;
    [SerializeField]
    GameObject gcObject;
    GameController gameController;
    Vector3 Direction;
    void Start(){
        gcObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = gcObject.GetComponent<GameController>();
        speed = gameController.GetSpeed();
    }

    // Update is called once per frame
    void Update(){
        int dirX = gameController.GetDir()?1:0;
        int dirY = gameController.GetDir()?0:1;
        Direction = new Vector3(dirX, dirY, 0);
        transform.position -= Direction * speed * Time.deltaTime;
        
        if (gcObject.transform.position.x - transform.position.x  > 7 ||
        gcObject.transform.position.y - transform.position.y > 7)  Destroy(gameObject);
        speed = gameController.GetSpeed();
    }
}
