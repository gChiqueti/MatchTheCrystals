using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public Score score;

    private float timerControl;

    public const int MAX_TIME_IN_SECONDS = 120;
    // Start is called before the first frame update
    void Start()
    {
        timerControl = MAX_TIME_IN_SECONDS;
        timerText.text = ((int) timerControl).ToString();  
    }

    // Update is called once per frame
    void Update()
    {
        timerControl -= Time.deltaTime;
        timerText.text = ((int)timerControl).ToString();

        bool desiredScoreActieved = IsScoreDesiredActieved();
        if (desiredScoreActieved)
        {
            LevelController.isScoreBeaten = true;
            LevelController.elapsedTime = MAX_TIME_IN_SECONDS - (int)timerControl;
            SceneManager.LoadScene("LevelPassedScene");
            
            
        } else if (timerControl <= 0)
        {
            LevelController.elapsedTime = MAX_TIME_IN_SECONDS;
            LevelController.isScoreBeaten = false;
            SceneManager.LoadScene("GameOverScene");
        }
    }

    private bool IsScoreDesiredActieved() {
        if (score.score >= LevelController.desiredScore)
        {
            return true;
        }
        return false;
    }
}
