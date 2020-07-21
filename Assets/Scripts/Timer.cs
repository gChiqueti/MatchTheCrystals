using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float timerControl;
    // Start is called before the first frame update
    void Start()
    {
        timerControl = 120;
        timerText.text = ((int) timerControl).ToString();  
    }

    // Update is called once per frame
    void Update()
    {
        timerControl -= Time.deltaTime;
        timerText.text = ((int)timerControl).ToString();
    }
}
