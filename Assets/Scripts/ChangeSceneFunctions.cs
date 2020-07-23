using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneFunctions : MonoBehaviour
{
    public void GoToBoardScene()
    {
        SceneManager.LoadScene("BoardScene");
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void SetNewLevelValues()
    {
        LevelController.setNextLevelVariables();
    }

    public void ResetAllLevelValues()
    {
        LevelController.ResetVariables();
    }

}
