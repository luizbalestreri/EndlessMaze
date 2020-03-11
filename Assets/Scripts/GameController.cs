using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.Rendering.PostProcessing;

public class GameController : MonoBehaviour
{
    public GameObject Spawner, Path, Player, Explosion, PathRight, PathLeft, Cam;
    public UIController UIController;
    public Queue<GameObject> pathQueue, trapQueue;
    public float speed{get; private set;} = 0;
    public float speedAdd, nextTurnInterval = 10;
    float counter, waitTime = 0.7f;
    public int score{get; private set;}
    public int scoreAdd = 1;
    public bool isLandscape {get; private set;} = false;
    public bool canTurn = false, gameOver = true;
    bool creatingPath = true;
    
    void Awake() {
        score = 0;
        speedAdd = 0.2f; 
        Spawner = transform.Find("Spawner").gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
        pathQueue = new Queue<GameObject>();
        trapQueue = new Queue<GameObject>();
    }
    public void NewGame(){
        speed = 3f;
        StartCoroutine(CreatePath());
        gameOver = false;
        counter = nextTurnInterval/speed - (9 * waitTime/speed);
    }
    void Update(){
        if (!gameOver && !UIController.pause){
            counter -= Time.deltaTime;
            speed+=speedAdd*Time.deltaTime;
            score+=scoreAdd;

            if(creatingPath){
                if (counter<=(0.2f/speed)){
                    creatingPath = false;
                }
            }

            if (counter <= 0) {
                bool createTrap = Random.Range(0, 3) == 1;
                if (!createTrap){
                    CreateTurn();
                }else{
                    StartCoroutine(CreateTurnWithTrap());
                } 
                counter = nextTurnInterval/speed;
            }

            if (Input.GetMouseButtonDown(0) && !UIController.IsPointerOverGameObject()){
                Turn();
                creatingPath = true;
                StartCoroutine(CreatePath());
            }
        }
    }

    IEnumerator CreatePath(){
        Instantiate(Path, Spawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(waitTime/speed);
        if(creatingPath) {StartCoroutine(CreatePath());}
    }

    void Turn(){
        if (canTurn){
            GameObject tempPath = pathQueue.Dequeue();
            GameObject tempTrap = null;
            if(trapQueue.Count > 0){
                tempTrap = trapQueue.Dequeue();
            }
            if (isLandscape) {
                transform.Rotate(0, 0, 90); 
                Vector3 destRotation = Vector3.zero;
                StartCoroutine(CameraRotation(Cam.transform.rotation, destRotation, 0.5f));
                Vector3 temp = Spawner.transform.position;
                float diff = temp.x - tempPath.transform.position.x;
                if (tempTrap != null) {
                    Vector3 tTtp = tempTrap.transform.position;
                    tempTrap.transform.position = new Vector3(tTtp.x + diff, tTtp.y, tTtp.y);
                }
                tempPath.transform.position = new Vector3(temp.x, tempPath.transform.position.y, tempPath.transform.position.z);
                StartCoroutine(Player.GetComponent<PlayerControl>().WindDirection(-1));
            } else {
                transform.Rotate(0, 0, -90);
                Vector3 destRotation = new Vector3(0, 0, -90);
                StartCoroutine(CameraRotation(Cam.transform.rotation, destRotation, 0.5f));
                Vector3 temp = Spawner.transform.position;
                float diff = temp.y - tempPath.transform.position.y;
                if (tempTrap != null) {
                    Vector3 tTtp = tempTrap.transform.position; 
                    tempTrap.transform.position = new Vector3(tTtp.x, tTtp.y + diff, tTtp.y);
                }
                tempPath.transform.position = new Vector3(tempPath.transform.position.x, temp.y, tempPath.transform.position.z);
                StartCoroutine(Player.GetComponent<PlayerControl>().WindDirection(1));
            }
            isLandscape = !isLandscape;
            canTurn = false;
        } else {
            GameOver();
            if (isLandscape) {
                transform.Rotate(0, 0, 90);
                StartCoroutine(CameraRotation(Cam.transform.rotation, Vector3.zero, 0.1f));
            } else{
                transform.Rotate(0, 0, -90); 
                StartCoroutine(CameraRotation(Cam.transform.rotation, new Vector3(0, 0 , -90), 0.1f));
            }
            isLandscape = !isLandscape;
        }
    }

    void CreateTurn(){
        GameObject tempPath = null;
        if (!isLandscape){
            tempPath = Instantiate(PathRight, Spawner.transform.position, Spawner.transform.rotation);
        }else{
            tempPath = Instantiate(PathRight, Spawner.transform.position, 
                Quaternion.Euler(new Vector3(0, 0, 90)));
        }
        pathQueue.Enqueue(tempPath);
    }

    void CreateTrap(int i){
        GameObject tempPath = null;
        if (isLandscape){
            tempPath = Instantiate(PathRight, 
            new Vector3(Spawner.transform.position.x + i, Spawner.transform.position.y, Spawner.transform.position.z),
                        Spawner.transform.rotation);
        }else{
            tempPath = Instantiate(PathRight, 
            new Vector3(Spawner.transform.position.x, Spawner.transform.position.y + i, Spawner.transform.position.z), 
                        Quaternion.Euler(new Vector3(0, 0, 180)));
        }
        tempPath.transform.Find("Collider").GetComponent<Collider2D>().enabled = false;
        trapQueue.Enqueue(tempPath);
    }
    IEnumerator CreateTurnWithTrap(){
        bool trapFirst = Random.Range(0, 2) == 1;        
        if (trapFirst){
            CreateTurn();
            CreateTrap(-1);
        } else {
            CreateTurn();
            CreateTrap(1);
        }
        yield return null;
    }

    IEnumerator CameraRotation(Quaternion rotation, Vector3 target, float time){
        Camera.main.orthographic = false;
        Camera.main.GetComponent<PostProcessLayer>().enabled = true;
        float i = 0;
        float multiplier = 1/time;
        while(i < time){
            Cam.transform.rotation = Quaternion.Slerp(rotation, Quaternion.Euler(target), i * multiplier);
            i += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Cam.transform.rotation = Quaternion.Euler(target);
        Camera.main.GetComponent<PostProcessLayer>().enabled = false;
        Camera.main.orthographic = true;
        yield return null;
    }
    public void GameOver(){
        speed = 0;
        canTurn = false;
        gameOver = true;
        Instantiate(Explosion, 
            new Vector3(Player.transform.position.x,
            Player.transform.position.y,
            Player.transform.position.z - 8),
            Quaternion.identity);
        Destroy(Player);
        UIController.GameOverUI();
        CameraShaker.Instance.ShakeOnce(2f, 2f, .1f, 0.5f);
    }
}