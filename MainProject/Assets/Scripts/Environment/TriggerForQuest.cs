using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForQuest : MonoBehaviour {

    //Author : Kate Georgiou

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (QuestReward.inst.retriveTimer() != 0)
            {
                QuestReward.inst.TriggerTrue();
            }
        }
       
    }
}
