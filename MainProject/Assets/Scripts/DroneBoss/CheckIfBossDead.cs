//Author: James Murphy
//Purpose: Check if the drone boss is dead
//Requirements: A drone to check

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckIfBossDead : MonoBehaviour
{
    [SerializeField]
    private GameObject droneBoss;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (droneBoss == null)
        {
            SceneManager.LoadScene("Boss");
        }
    }
}
