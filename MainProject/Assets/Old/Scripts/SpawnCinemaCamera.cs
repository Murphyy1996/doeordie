using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: James Murphy
//Purpose: Allow for easy spawning and deleting of the camera

public class SpawnCinemaCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject spectatorCamPrefab, spawnedSpectatorCam;

    private void Start()
    {
        if (spectatorCamPrefab == null)
        {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        if (spawnedSpectatorCam == null)
        {
            if (Input.GetKey(KeyCode.Insert))
            {

                spawnedSpectatorCam = Instantiate(spectatorCamPrefab, transform.position, transform.rotation) as GameObject;
                spawnedSpectatorCam.name = "Spectator Camera";
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                Destroy(spawnedSpectatorCam);
                GameObject.Find("Gun Camera").GetComponent<Camera>().enabled = true;
                GameObject player = GameObject.Find("Player");
                player.GetComponentInChildren<Camera>().enabled = true;
                player.GetComponentInChildren<Camera>().tag = "MainCamera";
                GameObject.Find("InGameCanvas(Clone)").GetComponent<Canvas>().enabled = true;
                try
                {
                    GameObject.Find("BossUI").GetComponent<Canvas>().enabled = true;
                }
                catch
                {
                    print("Error getting boss ui to turn on");
                }
            }
        }
    }
}
