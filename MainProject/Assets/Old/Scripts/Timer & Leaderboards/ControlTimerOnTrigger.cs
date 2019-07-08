using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: James Murphy
//Purpose: Affect the timer when player hits the trigger

public class ControlTimerOnTrigger : MonoBehaviour
{
    private enum timerSettings { start, stop, reset };
    [SerializeField]
    private timerSettings selectedTimerSetting = timerSettings.start;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && TimerController.singleton != null)
        {
            //Control the timer based on the selected setting
            switch (selectedTimerSetting)
            {
                case timerSettings.start:
                    TimerController.singleton.StartTimer();
                    break;
                case timerSettings.stop:
                    TimerController.singleton.StopTimer();
                    break;
                case timerSettings.reset:
                    TimerController.singleton.ResetTimer();
                    break;
            }
            //Destroy this script as its no longer need
            Destroy(this);
        }
    }
}
