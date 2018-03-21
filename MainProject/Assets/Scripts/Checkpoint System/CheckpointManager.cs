using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager singleton;

    [SerializeField]
    private GameObject currentCheckpoint;

    //Initialise the singleton
    private void Awake()
    {
        singleton = this;
    }

    public void SetCurrentCheckpoint(GameObject checkpointObj) //This will set the variable containing the current checkpoint with the specified object
    {
        currentCheckpoint = checkpointObj;
        if (SaveSystemManager.inst != null)
        {
            SaveSystemManager.inst.SaveGame();
        }
        else
        {
            print("Save system manager hasn't loaded in. You need to run the game from the main menu.");   
        }

    }

    public GameObject GetCurrentCheckpoint() //Return the current checkpoint if a script needs it
    {
        return currentCheckpoint;
    }
}
