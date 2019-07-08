using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Author: James Murphy
//Purpose: Activate obj when the player enters a trigger

public class TriggerActivateObj : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objsToActivate;
    [SerializeField]
    private GameObject[] objsToTurnOff;
    private Collider thisCollider;
    private ReusableHealth playerHealth;

    private void Start()
    {
        //Put this on layer 23 so it doesn't interfere with the game
        this.gameObject.layer = 23;
        //If there are no objects just destroy this script as it is of no use
        if (objsToActivate.Length == 0 && objsToTurnOff.Length == 0)
        {
            Destroy(this);
        }
        //Check this object and make sure it has a collider
        if (GetComponent<Collider>() != null)
        {
            thisCollider = GetComponent<Collider>();
        }
        else //Make a collider
        {
            thisCollider = this.gameObject.AddComponent<BoxCollider>();
        }
        //Set this collider as trigger
        thisCollider.isTrigger = true;
        //Make this mesh renderer turn off
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        playerHealth = GameObject.Find("Player").GetComponent<ReusableHealth>();
    }

    //Test to see if the player has entered the collider
    private void OnTriggerEnter(Collider other)
    {
        if (playerHealth == null)
        {
            playerHealth = GameObject.Find("Player").GetComponent<ReusableHealth>();
        }
        if (other.tag == "Player" && playerHealth.healthValue > 0)
        {
            if (objsToActivate.Length != 0)
            {
                foreach (GameObject obj in objsToActivate)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
            }
            if (objsToTurnOff.Length != 0)
            {
                foreach (GameObject obj in objsToTurnOff)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }
            //Destroy this script as its no longer needed
            Destroy(this);
        }
    }
}
