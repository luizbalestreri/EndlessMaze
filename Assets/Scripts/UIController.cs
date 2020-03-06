using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject gameOver, scoreCounter, newGame;
    public GameController gameController;
    
    public Text ScoreCounter;
    void Start(){
        
    }

    void Update(){
        ScoreCounter.text = gameController.score.ToString();
    }

    public void GameOverUI(){
        gameOver.SetActive(true);
        scoreCounter.SetActive(false);
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
}
