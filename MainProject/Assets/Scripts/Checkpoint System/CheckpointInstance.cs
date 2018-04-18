using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Place this script on a invisible checkpoint trigger (It will be a prefab any way) and when the player walks into it, it will set this object as the checkpoint

public class CheckpointInstance : MonoBehaviour
{
    private ReusableHealth playerHealth;

    //Make sure the checkpoint is invisible
    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        playerHealth = GameObject.Find("Player").GetComponent<ReusableHealth>();
    }

    private void OnTriggerEnter(Collider otherObject)
    {
        //If the other object is the player
        if (otherObject.tag == "Player")
        {
            if (CheckpointManager.singleton != null && playerHealth.healthValue > 0)
            {
                //Do not set the checkpoint multiple times
                if (CheckpointManager.singleton.GetCurrentCheckpoint() != this.gameObject)
                {
                    //Set the current object as this game object
                    CheckpointManager.singleton.SetCurrentCheckpoint(this.gameObject);
                }
            }
            else
            {
                print("Checkpoint manager has not be loaded");
            }
        }
    }
}
