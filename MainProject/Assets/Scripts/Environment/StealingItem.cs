using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingItem : MonoBehaviour {

    //Author: Kate Georgiou
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            QuestReward.inst.StolenBoolTrue();
            Destroy(this.gameObject);
           
        }
    }
       

}
