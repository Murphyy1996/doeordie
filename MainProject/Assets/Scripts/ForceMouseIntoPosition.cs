//Author: James Murphy
//Purpose: Will force the mouse into the correct position
//Requirements: Goes on the player prefab

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceMouseIntoPosition : MonoBehaviour
{
    private void Awake() //Control the mouse on the tick, independent of timescale
    {
        InvokeRepeating("Tick", 0, 0.1f);
    }

    private void Tick()
    {
        if (Time.timeScale != 0)
        {
            print("hello");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
