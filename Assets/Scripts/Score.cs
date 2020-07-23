using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Score : MonoBehaviour
{
    
    public int score = 0;
    public Text scoreText;
    public Text levelText;

    public void Start()
    {
        LevelController.desiredScore = LevelController.actualLevel*10 + 10;
        
        UpdateScoreText();
        UpdateLevelText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }


    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }


    private void UpdateScoreText()
    {
        scoreText.text = score.ToString() + "/" + LevelController.desiredScore;
    }

    private void UpdateLevelText()
    {
        levelText.text = LevelController.actualLevel.ToString();
    }

}
