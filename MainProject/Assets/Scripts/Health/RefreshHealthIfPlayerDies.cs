using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Author: James Murphy
//Purpose: Refresh health of this obj if the player dies

public class RefreshHealthIfPlayerDies : MonoBehaviour
{
    private ReusableHealth thisHealth, playerHealth;
    [SerializeField]
    private bool refreshPositionUponPlayerDeath = false, destroyAllSpawnedDrones = false, restartSceneUponPlayerDeath = false, respawnPickups = true;
    private Vector3 defaultPosition;
    private GameObject[] foundPickups;

    // Use this for initialization
    private void Start()
    {
        //Get the required references
        thisHealth = GetComponent<ReusableHealth>();
        playerHealth = GameObject.Find("Player").GetComponent<ReusableHealth>();
        defaultPosition = transform.position;
        foundPickups = GameObject.FindGameObjectsWithTag("Pickup");
        
        //Delete this script is the required references are not got
        if (thisHealth == null || playerHealth == null)
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //If the players health is zero or below
        if (thisHealth != null && playerHealth != null)
        {
            if (playerHealth.healthValue <= 0)
            {
                //Restart the scene upon player death - Useful for the boss
                if (restartSceneUponPlayerDeath == true)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                //Refresh pick ups
                if (respawnPickups == true)
                {
                    foreach (GameObject pickups in foundPickups)
                    {
                        pickups.SetActive(true);
                    }
                }
                //Refresh this object health
                thisHealth.healthValue = thisHealth.maxHealth;
                if (refreshPositionUponPlayerDeath == true)
                {
                    //Refresh the position
                    transform.position = defaultPosition;
                }
                //Kill all existing drones
                GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("enemy");

                foreach (GameObject enemy in foundEnemies)
                {
                    if (enemy.name == "AlwaysAttackDrone(Clone)")
                    {
                        Destroy(enemy.gameObject);
                    }
                }
            }
        }
    }
}
