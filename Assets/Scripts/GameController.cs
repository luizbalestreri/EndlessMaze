using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject Spawner;
    public GameObject Path;
    public GameObject Player;
    public GameObject Explosion;
    public Text ScoreCounter;
    public GameObject gameOverUI;
    public Queue<GameObject[]> pathQueue;
    public float speed;
    public float speedAdd;
    public float nextTurnInterval = 10;
    float waitTime = 0.7f;
    float counter;
    public int scoreAdd = 1;
    int score;
    int turnPathSize = 10;
    bool isLandscape = false;
    bool creatingPath = true;
    public bool canTurn = false;
    public bool gameOver = false;
    
    void Awake() {
        speed = 3f;
        score = 0;
        speedAdd = 0.03f; 
        Spawner = transform.Find("Spawner").gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
        pathQueue = new Queue<GameObject[]>();
    }

    void Start(){
        StartCoroutine("CreatPath");
        counter = nextTurnInterval/speed;
    }

    void Update(){
        ScoreCounter.text = score.ToString();
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
        gameOverUI.SetActive(true);

    }

    public float GetSpeed(){
        return speed;
    }

    public bool GetDir(){
        return isLandscape;
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameOverUI.SetActive(false);
    }
}