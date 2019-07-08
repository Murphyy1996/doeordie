using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: James Murphy
//Purpose: Set up the game for recoding

public class RecordingSetUp : MonoBehaviour
{
    private Camera oldCamera;
    private GameObject player;
    private ReusableHealth playerHealth;

    private void Start()
    {
        oldCamera = Camera.main;
        GameObject.Find("Gun Camera").GetComponent<Camera>().enabled = false;
        player = GameObject.Find("Player");
        playerHealth = player.GetComponent<ReusableHealth>();
        GameObject.Find("InGameCanvas(Clone)").GetComponent<Canvas>().enabled = false;
        try
        {
            GameObject.Find("BossUI").GetComponent<Canvas>().enabled = false;
        }
        catch
        {
            print("Error");
        }
    }

    private void Update()
    {
        playerHealth.maxHealth = 9999;
        playerHealth.currentHealth = 9999;
    }
}
