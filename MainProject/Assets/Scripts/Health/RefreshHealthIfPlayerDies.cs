using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: James Murphy
//Purpose: Refresh health of this obj if the player dies

public class RefreshHealthIfPlayerDies : MonoBehaviour
{
    private ReusableHealth thisHealth, playerHealth;
    [SerializeField]
    private bool refreshPositionUponPlayerDeath = false;
    private Vector3 defaultPosition;

    // Use this for initialization
    private void Start()
    {
        //Get the required references
        thisHealth = GetComponent<ReusableHealth>();
        playerHealth = GameObject.Find("Player").GetComponent<ReusableHealth>();
        defaultPosition = transform.position;
        
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
                //Refresh this object health
                thisHealth.healthValue = thisHealth.maxHealth;
                if (refreshPositionUponPlayerDeath == true)
                {
                    //Refresh the position
                    transform.position = defaultPosition;
                }
            }
        }
    }
}
