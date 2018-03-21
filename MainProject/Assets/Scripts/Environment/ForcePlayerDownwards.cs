using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: James Murphy
//Purpose: Force the player downwwards

public class ForcePlayerDownwards : MonoBehaviour
{
    private CharacterControllerMovement movementScript;
    private void Start()
    {
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().isTrigger = true;
        }
        else //if there is no collider, destroy this game object
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (movementScript == null)
            {
                movementScript = other.GetComponent<CharacterControllerMovement>();
            }
            movementScript.TriggerForcedDescent(500f);
        }
    }
}
