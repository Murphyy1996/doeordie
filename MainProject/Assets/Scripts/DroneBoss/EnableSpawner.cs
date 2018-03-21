using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Purpose: Enable the spawner for the enemy drones when the drone is in an attack behaviour

public class EnableSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject objToEnable;
    private Drone droneScript;

    private void Start()
    {
        droneScript = GetComponent<Drone>();
    }

    private void FixedUpdate()
    {
        if (droneScript.CheckIfAttacking() == true && objToEnable != null)
        {
            objToEnable.SetActive(true);
            Destroy(this);
        }
    }

}
