using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: James Murphy
//Purpose: Control the timer

public class TimerController : MonoBehaviour
{
    private float time = 0;
    private bool timerStarted = false;
    public static TimerController singleton;

    private void Awake()
    {
        singleton = this;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (UIElements.singleton.timerText != null)
        {
            //Increase the timer
            if (timerStarted == true)
            {
                time = time + Time.fixedDeltaTime;
            }
            //Set the timer if it is zero
            if (time == 0)
            {
                UIElements.singleton.timerText.text = "00:00";
                UIElements.singleton.timerText.color = Color.red;
            }
            else //Update the timer and format it
            {
                if (timerStarted == true)
                {
                    int intTime = (int)time;
                    int minutes = intTime / 60;
                    int seconds = intTime % 60;
                    float fraction = time * 1000;
                    fraction = (fraction % 1000);
                    string timeText = String.Format("{0:00}:{1:00}", minutes, seconds);
                    UIElements.singleton.timerText.text = timeText;
                    //Colour the formatted text if it is activley counting
                    UIElements.singleton.timerText.color = Color.yellow;
                }
                else //if the timer has stopped set the colour to green
                {
                    int intTime = (int)time;
                    int minutes = intTime / 60;
                    int seconds = intTime % 60;
                    float fraction = time * 1000;
                    fraction = (fraction % 1000);
                    string timeText = String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
                    UIElements.singleton.timerText.text = timeText;
                    UIElements.singleton.timerText.color = Color.green;
                }
            }
        }
    }

    //Public methods for controlling the timer

    public void StartTimer()
    {
        timerStarted = true;
    }

    public void StopTimer()
    {
        LeaderboardManager.singleton.AddTimeToUnsortedLeaderboard(time.ToString());
        timerStarted = false;
    }

    public void ResetTimer()
    {
        time = 0;
    }
}
