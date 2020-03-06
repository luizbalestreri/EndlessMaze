using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class GameController : MonoBehaviour
{
    public GameObject Spawner, Path, Player, Explosion;
    public UIController UIController;
    public Queue<GameObject[]> pathQueue;
    public float speed{get; private set;} = 0;
    public float speedAdd, nextTurnInterval = 10;
    float counter, waitTime = 0.7f;
    public int score{get; private set;}
    public int scoreAdd = 1;
    int turnPathSize = 10;
    public bool isLandscape {get; private set;} = false;
    public bool canTurn = false, gameOver = true;
    bool creatingPath = true;
    
    void Awake() {
        score = 0;
        speedAdd = 0.03f; 
        Spawner = transform.Find("Spawner").gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
        pathQueue = new Queue<GameObject[]>();
    }

    public void NewGame(){
        speed = 3f;
        StartCoroutine("CreatPath");
        gameOver = false;
        counter = nextTurnInterval/speed - (9 * waitTime/speed);
    }

    void Update(){
        if (!gameOver){
            counter -= Time.deltaTime;
            speed+=speedAdd*Time.deltaTime;
            score+=scoreAdd;

            if(creatingPath){
                if (counter<=(0.2f/speed)){
                    creatingPath = false;
                }
            }
            if (counter <= 0) {
                CreateTurn(); 
                counter = nextTurnInterval/speed;}
            if (Input.GetMouseButtonDown(0)){
                Turn();
                creatingPath = true;
                StartCoroutine(CreatPath());
            }
        }
    }

    IEnumerator CreatPath(){
        Instantiate(Path, Spawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(waitTime/speed);
        if(creatingPath) {StartCoroutine(CreatPath());}
    }

    void Turn(){
        if (canTurn){
            isLandscape = !isLandscape;
            GameObject[] tempPath = pathQueue.Dequeue();
            if (isLandscape) {
                transform.Rotate(0, 0, -90); 
                Vector3 temp = Spawner.transform.position;
                foreach (GameObject path in tempPath){
                    path.transform.position = new Vector3(path.transform.position.x, temp.y, path.transform.position.z);
                }
                StartCoroutine(Player.GetComponent<PlayerControl>().WindDirection(1));
            } else {
                transform.Rotate(0, 0, 90); 
                Vector3 temp = Spawner.transform.position;
                foreach (GameObject path in tempPath){
                    path.transform.position = new Vector3(temp.x, path.transform.position.y, path.transform.position.z);
                }
                StartCoroutine(Player.GetComponent<PlayerControl>().WindDirection(-1));
            }
            canTurn = false;
        } else {
            GameOver();
            if (isLandscape) {
                transform.Rotate(0, 0, 90);
            } else{
                transform.Rotate(0, 0, -90); 
            }
        }
    }

    void CreateTurn(){
        GameObject[] tempPath = new GameObject[turnPathSize];
        if (!isLandscape){
            for(int i = 0; i < turnPathSize - 1; i++){
                GameObject clone = Instantiate(Path,
                            new Vector3(Spawner.transform.position.x + 1 + i, Spawner.transform.position.y, Spawner.transform.position.z),
                            Quaternion.identity);
                tempPath[i] = clone;  
            }
        }else{
            for(int i = 0; i < turnPathSize - 1; i++){
                GameObject clone = Instantiate(Path,
                            new Vector3(Spawner.transform.position.x, Spawner.transform.position.y + 1 + i, Spawner.transform.position.z),
                            Quaternion.identity);
                            tempPath[i] = clone;
            }
        }
        GameObject clone2 = Instantiate(Path, Spawner.transform.position, Spawner.transform.rotation);
        clone2.GetComponent<Collider>().enabled = true;
        tempPath[turnPathSize - 1] = clone2;
        pathQueue.Enqueue(tempPath);
    }

    public void GameOver(){
        speed = 0;
        canTurn = false;
        gameOver = true;
        Instantiate(Explosion, this.transform.position, Quaternion.identity);
        Destroy(Player);
        UIController.GameOverUI();
        CameraShaker.Instance.ShakeOnce(2f, 2f, .1f, 0.5f);

    }
}