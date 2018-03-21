using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: James Cookj
public class TimeGate : MonoBehaviour {

    [SerializeField]
    private bool start;
    [SerializeField]
    private bool end;

	// Use this for initialization
	void Start () 
    {
        if (start == true && end == true)
        {
            start = true;
            end = false;
        }

        if (start == false && end == false)
        {
            start = true;
            end = false;
        }
	}
	
    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Player")
        {
            if (start == true)
            {
                TimerController.singleton.StartTimer();
            }

            if (end == true)
            {
                TimerController.singleton.StopTimer();
                Debug.Log("End timer");
            }
        }

    }
}
