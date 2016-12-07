﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    //private GameObject health, score;
    private Text health, score, prevScore, shield;
    //private int pts = 0;
    private PlayerCharacter player;
    private LevelManager lvlman;
    public GameObject pauseScreen;// = GameObject.FindGameObjectWithTag("PauseScreen");
    public bool isPaused = false;
    public bool resumePressed = false;
	// Use this for initialization
	void Start () {
        lvlman = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        health = GameObject.FindGameObjectWithTag("Health").GetComponent<Text>() as Text;
        shield = GameObject.FindGameObjectWithTag("Shield").GetComponent<Text>() as Text;
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>() as Text;
        prevScore = GameObject.FindGameObjectWithTag("PrevScore").GetComponent<Text>() as Text;
        pauseScreen = GameObject.FindGameObjectWithTag("PauseScreen");
        pauseScreen.SetActive(false);
        //prevScore.text = "Last Run's Score: " +PlayerPrefs.GetFloat("previousScore");
        prevScore.text = "Highest Score: " + PlayerPrefsX.GetFloatArray("Scores")[0];
        health.text = "Health: " +player.getHealth().ToString();
        score.text = "Score: " +0.ToString();
        health.color = new Color(0f, 255f / 255f, 65f / 255f);
        //fixes weird frozen ship.
        isPaused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
        onResume();
    }
	
	// Update is called once per frame
	void Update () {
        
        if(!isPaused)
        {
            player.modifyScore(1);
        }
        score.text = "Score: " +player.getScore().ToString();
        health.text = "Health: " + player.getHealth().ToString();
        shield.text = "Shield: " + player.getShield().ToString();

        //changes color of health value so that the player notices they are close to dying
        if (player.getHealth() <= 20)
        {
            health.color = Color.red;
        }
        if(player.getHealth() <= 0)
        {
            //player iz ded
            player.heal(100f);
            playerDied(PlayerPrefs.GetString("CurrentName", "No Name"), player.getScore());
        }

   
        //allows user to pause the game and return to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {  
            isPaused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
      
    }
    
    
    public void onResume()
    {
        pauseScreen = GameObject.FindGameObjectWithTag("PauseScreen");
        isPaused = false;
        resumePressed = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }
    public void onQuit()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }

    public void playerWin()
    {
        Time.timeScale = 0.1f;
        Text gameOver = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Text>();
        gameOver.text = "LEVEL COMPLETE\n Final Score: " + player.getScore();
        GameObject temp1 = GameObject.FindGameObjectWithTag("Health");
        temp1.SetActive(false);
        GameObject temp2 = GameObject.FindGameObjectWithTag("Score");
        temp2.SetActive(false);
        Time.timeScale = 1;
    }

    public void playerDied(string name, float finalScore)
    {
        Time.timeScale = 0.1f;
        Text gameOver = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Text>();
        gameOver.text = "GAME OVER\n Final Score: " + finalScore;
        GameObject temp1 = GameObject.FindGameObjectWithTag("Health");
        temp1.SetActive(false);
        GameObject temp2 = GameObject.FindGameObjectWithTag("Score");
        temp2.SetActive(false);
        StartCoroutine(endGame());
        PlayerPrefs.SetFloat("previousScore", finalScore);

        float[] scores = PlayerPrefsX.GetFloatArray("Scores");
        string[] names = PlayerPrefsX.GetStringArray("Names");

        bool higher = false;
        if (finalScore > scores[scores.Length - 1])
        {
            //replace last element with new high score
            scores[scores.Length - 1] = finalScore;
            names[names.Length - 1] = name;
            higher = true;
        }

        if (higher) { 
            for (int j = scores.Length-1; j >0; j--)
            {
                //find correct position for new score
                //assumes score array is already sorted
                if (scores[j -1] < scores[j])
                {
                    float temp = scores[j];
                    string tempName = names[j];
                    scores[j] = scores[j - 1];
                    names[j] = names[j - 1];
                    scores[j - 1] = temp;
                    names[j - 1] = tempName;
                }
            }
        }

        PlayerPrefsX.SetFloatArray("Scores", scores);
        PlayerPrefsX.SetStringArray("Names", names);
    }

    //waits for 5 seconds then returns the user to the main menu
    IEnumerator endGame()
    {
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1;
        lvlman.lose_Scene();
    }
}
