using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Results : MonoBehaviour
{
    public Text VictoryDataText;

    void Start()
    {
        int level = LevelController.actualLevel;
        int score = LevelController.desiredScore;
        int time = LevelController.elapsedTime;

        VictoryDataText.text = "LEVEL: " + level + "\n" + 
                               "SCORE: " + score + "\n" + 
                               "TIME:  " + time;
    }
    
}
