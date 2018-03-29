using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Author: Kate Georgiou Purpose: To attach to any object with a trigger on that might be in the environment to instantly kill the player.
[RequireComponent(typeof(BoxCollider))]
public class InstantKill : MonoBehaviour
{
    private Teleporting teleportScript;
	// Use this for initialization
	void Start () {
        GetComponent<BoxCollider>().isTrigger = true;
        teleportScript = GameObject.Find("Player").GetComponent<Teleporting>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.GetComponent<ReusableHealth>() != null && teleportScript.ReturnIfTeleporting() == false || other.tag == "enemy" && other.gameObject.GetComponent<ReusableHealth>() != null)
        {
            other.gameObject.GetComponent<ReusableHealth>().healthValue = 0;
            other.gameObject.GetComponent<ReusableHealth>().CheckToSeeIfDead();
        }
    }
}
