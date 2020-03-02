using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Spawner;
    public GameObject PathV;
    public GameObject Player;
    public Queue<GameObject[]> pathQueue;
    float speed;
    float waitTime = 0.7f;
    float time = 2f;
    int turnPathSize = 11;
    Vector3 vStartPos = new Vector3(0.5f,6,2);
    Vector3 hStartPos = new Vector3(9, -2.5f, 2);
    bool dirChanging = false;
    bool isLandscape = false;
    public bool canTurn = false;
    public bool gameOver = false;
    void Awake() {
        speed = 3f;
        Spawner = transform.Find("Spawner").gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
        pathQueue = new Queue<GameObject[]>();
    }
    void Start(){
        StartCoroutine("CreatPath");
    }

    void Update(){
        if (!gameOver){
            time -= Time.deltaTime;
            if (time <= 0) {dirChanging = true; CreateTurn(); time = 3;}
            if (Input.GetMouseButtonDown(0)){
                Turn();
                dirChanging = false;
                StartCoroutine("CreatV");
            }
        }
    }

    IEnumerator CreatPath(){
        Instantiate(PathV, Spawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(waitTime/speed);
        if(!dirChanging) {StartCoroutine("CreatPath");}
    }

    void Turn(){
        if (canTurn){
            isLandscape = !isLandscape;
            GameObject[] tempPath = pathQueue.Dequeue();
            if (isLandscape) {
                transform.Rotate(0, 0, -90); 
                //Player.transform.Rotate(0, 0, -90);
                Vector3 temp = Spawner.transform.position;
                foreach (GameObject path in tempPath){
                    path.transform.position = new Vector3(path.transform.position.x, temp.y, path.transform.position.z);
                }

            } else {

                transform.Rotate(0, 0, 90); 
                //Player.transform.Rotate(0, 0, 90);
                Vector3 temp = Spawner.transform.position;
                foreach (GameObject path in tempPath){
                    path.transform.position = new Vector3(temp.x, path.transform.position.y, path.transform.position.z);
                }
            }
            canTurn = false;
        }
    }

    void CreateTurn(){
        dirChanging = true;
        GameObject[] tempPath = new GameObject[turnPathSize];
        if (!isLandscape){
            for(int i = 0; i < turnPathSize - 1; i++){
                GameObject clone = Instantiate(PathV,
                            new Vector3(Spawner.transform.position.x + i, Spawner.transform.position.y, Spawner.transform.position.z),
                            Quaternion.identity);
                tempPath[i] = clone;  
            }
        }else{
            for(int i = 0; i < turnPathSize - 1; i++){
                GameObject clone = Instantiate(PathV,
                            new Vector3(Spawner.transform.position.x, Spawner.transform.position.y + i, Spawner.transform.position.z),
                            Quaternion.identity);
                            tempPath[i] = clone;
            }
        }
        GameObject clone2 = Instantiate(PathV, Spawner.transform.position, Spawner.transform.rotation);
        clone2.GetComponent<Collider>().enabled = true;
        tempPath[turnPathSize - 1] = clone2;
        pathQueue.Enqueue(tempPath);
    }

    public void GameOver(){
        speed = 0;
        canTurn = false;
        gameOver = true;
    }

    public float GetSpeed(){
        return speed;
    }

    public bool GetDir(){
        return isLandscape;
    }
}
