using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour{
    float speed;
    [SerializeField]
    GameObject gcObject;
    Transform gcObjectTF;
    GameController gameController;
    Vector3 Direction;
    void Start(){
        gcObject = GameObject.FindGameObjectWithTag("GameController");
        gcObjectTF = gcObject.transform;
        gameController = gcObject.GetComponent<GameController>();
        speed = gameController.speed;
    }

    // Update is called once per frame
    void Update(){
        int dirX = gameController.isLandscape?1:0;
        int dirY = gameController.isLandscape?0:1;
        Direction = new Vector3(dirX, dirY, 0);
        transform.position -= Direction * speed * Time.deltaTime;
        
        if (gcObjectTF.position.x - transform.position.x  > 11 ||
           gcObjectTF.position.y - transform.position.y > 11)  
            Destroy(gameObject);
        speed = gameController.speed;
    }
}
