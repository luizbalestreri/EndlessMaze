using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class GameController : MonoBehaviour
{
    public GameObject Spawner, Path, Player, Explosion, PathRight;
    public CameraController cameraController;
    public UIController UIController;
    public Queue<GameObject> pathQueue, trapQueue;
    public float speed{get; set;} = 0;
    public float speedAdd, nextTurnInterval = 10, rotationSpeed = 0.2f;
    protected float counter, waitTime = 0.7f;
    public int score{get; private set;}
    public int trap, scoreAdd = 1, RandomMax = 4;
    public bool isLandscape {get; protected set;} = false;
    public bool canTurn = false, gameOver = true;
    public bool creatingPath = true;
    
    void Awake() {
        cameraController = Camera.main.GetComponent<CameraController>();
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
                if (counter<=(0.1f/speed)){
                    creatingPath = false;
                }
            }

            if (counter <= 0) {
                int trapRange = Random.Range(0, RandomMax);
                Debug.Log(trapRange);
                bool createTrap = trapRange == 3;
                if (!createTrap){
                   trap = 0;
                    CreateTurn();
                }else{
                    CreateTurnWithTrap();
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

    public IEnumerator CreatePath(){
        Instantiate(Path, Spawner.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(waitTime/speed);
        if(creatingPath) {StartCoroutine(CreatePath());}
    }

    void Turn(){
        if (canTurn){
            GameObject dequeuePath = pathQueue.Dequeue();
            GameObject dequeueTrap = null;

            if(trapQueue.Count > 0){
                dequeueTrap = trapQueue.Dequeue();
            }

            if (isLandscape) {
                Quaternion destRotation = Quaternion.Euler(Vector3.zero);
                transform.rotation = destRotation; 
                StartCoroutine(cameraController.CameraRotation(destRotation, rotationSpeed));
                Vector3 spwnPos = Spawner.transform.position;
                if (dequeueTrap != null) {
                    float diff = spwnPos.x - dequeuePath.transform.position.x;
                    Vector3 dqTrapPos = dequeueTrap.transform.position;
                    dequeueTrap.transform.position = new Vector3(dqTrapPos.x + diff, dqTrapPos.y, dqTrapPos.z);
                }
                dequeuePath.transform.position = new Vector3(spwnPos.x, dequeuePath.transform.position.y, dequeuePath.transform.position.z);
                StartCoroutine(Player.GetComponent<PlayerControl>().WindDirection(-1));

            } else {
                Quaternion destRotation = Quaternion.Euler(0, 0, -90);
                transform.rotation = destRotation;
                StartCoroutine(cameraController.CameraRotation(destRotation, rotationSpeed));
                Vector3 spwnPos = Spawner.transform.position;
                if (dequeueTrap != null) {
                    float diff = spwnPos.y - dequeuePath.transform.position.y;
                    Vector3 dqTrapPos = dequeueTrap.transform.position; 
                    dequeueTrap.transform.position = new Vector3(dqTrapPos.x, dqTrapPos.y + diff, dqTrapPos.z);
                }
                dequeuePath.transform.position = new Vector3(dequeuePath.transform.position.x, spwnPos.y, dequeuePath.transform.position.z);
                StartCoroutine(Player.GetComponent<PlayerControl>().WindDirection(1));
            }
            isLandscape = !isLandscape;
            canTurn = false;
        
        } else {
            GameOver();
            if (isLandscape) {
                transform.Rotate(0, 0, 90);
                StartCoroutine(cameraController.CameraRotation(Quaternion.Euler(Vector3.zero), rotationSpeed/2));
            } else{
                transform.Rotate(0, 0, -90); 
                StartCoroutine(cameraController.CameraRotation(Quaternion.Euler(0, 0 , -90), rotationSpeed/2));
            }
            isLandscape = !isLandscape;
        }
    }

    public void CreateTurn(){
        GameObject tempPath = null;
        if (!isLandscape){
            tempPath = Instantiate(PathRight, Spawner.transform.position, Spawner.transform.rotation);
        }else{
            tempPath = Instantiate(PathRight, Spawner.transform.position, 
                Quaternion.Euler(new Vector3(0, 0, 90)));
        }
        pathQueue.Enqueue(tempPath);
    }

    protected void CreateTrap(int posicao){
        GameObject tempPath = null;
        Vector3 spawnerPos = Spawner.transform.position;
        if (isLandscape){ 
            tempPath = Instantiate(PathRight, 
            new Vector3(spawnerPos.x + posicao, spawnerPos.y, spawnerPos.z), Spawner.transform.rotation);
        }else{
            tempPath = Instantiate(PathRight, 
            new Vector3(spawnerPos.x, spawnerPos.y + posicao, spawnerPos.z), Quaternion.Euler(0, 0, 180));
        }
        if (trap == 0){
            tempPath.transform.Find("Collider").GetComponent<Collider2D>().enabled = false;
        } else {
            tempPath.transform.Find("Collider").tag = "trap";
        }
        trapQueue.Enqueue(tempPath);
    }

    void CreateTurnWithTrap(){
        bool trapFirst = Random.Range(0, 2) == 1;        
        if (trapFirst){
            trap = 0;
            CreateTurn();
            CreateTrap(-1);
        } else {
            trap = 1;
            CreateTurn();
            CreateTrap(1);
        }
    }

    public void GameOver(){
        speed = 0;
        canTurn = false;
        gameOver = true;
        Vector3 playerPos = Player.transform.position;
        Instantiate(Explosion, new Vector3(playerPos.x, playerPos.y, playerPos.z - 8), Quaternion.identity);
        Destroy(Player);
        UIController.GameOverUI();
        CameraShaker.Instance.ShakeOnce(2f, 2f, .1f, 0.5f);
    }
}