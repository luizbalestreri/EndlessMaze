using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    public GameObject gameOver, scoreCounter, newGame, pauseBtn, newRecord;
    public AudioSource audioSource;
    public Text txtPause;
    public GameController gameController;
    public bool pause = false;
    public Text txtScoreCounter;
    void Update(){
        txtScoreCounter.text = gameController.score.ToString();
    }

    public void GameOverUI(){
        gameOver.SetActive(true);
        scoreCounter.SetActive(false);
        Text txtTopScore = GameObject.Find("Top Score").GetComponent<Text>();
        int topScore = PlayerPrefs.GetInt("score", gameController.score);
        if (topScore > gameController.score){
            txtTopScore.text = topScore.ToString();
        } else {
            PlayerPrefs.SetInt("score", gameController.score);
            PlayerPrefs.Save();
            newRecord.SetActive(true);
            txtTopScore.text = gameController.score.ToString();
            Debug.Log(PlayerPrefs.GetInt("score", gameController.score));
        }
    }

    public void Restart(){
        gameOver.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } 

    public void NewGameUI(){
        newGame.SetActive(true);
    }

    public void NewGame(){
        gameController.NewGame();
        newGame.SetActive(false);
        scoreCounter.SetActive(true);
    }

    public void PauseGame(){
        if (pause){
            Time.timeScale = 1;
            txtPause.text = "=";
        } else {
            Time.timeScale = 0;
            txtPause.text = "▲";
        }
        pause = !pause;
    }

    public static bool IsPointerOverGameObject(){
             //check mouse
        if(EventSystem.current.IsPointerOverGameObject())
            return true;    
             //check touch
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began ){
            if(EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return true;
        }     
        return false;
    }
}
