//Author: James Murphy
//Purpose: To detect collisions with this object
//Requirements: Place me on the ledge trigger zone via script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeCollisionDetection : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;
    private CharacterControllerMovement movement;
    private Grapple grapple;
    private LedgeClimbV2 ledgeClimb;
    private Rigidbody rb;
    private bool scriptSetUp = false;
    private void Awake() //Get the required scripts
    {
        Invoke("GetComponents", 0.1f);
    }
    private void Start()
    {
        UIElements.singleton.ledgeClimb.enabled = false;
    }

    private void GetComponents() //This will get all the required components
    {
        if (player != null)
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            movement = player.GetComponent<CharacterControllerMovement>();
            grapple = player.GetComponent<Grapple>();
            ledgeClimb = player.GetComponent<LedgeClimbV2>();
            scriptSetUp = true;
        }
        else //Start an invoke call checking for the player every half a second until the script has set up
        {
            InvokeRepeating("SetUpScript", 0.5f, 0.5f);
        }
    }

    private void SetUpScript() //This code will set up the script
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            movement = player.GetComponent<CharacterControllerMovement>();
            grapple = player.GetComponent<Grapple>();
            ledgeClimb = player.GetComponent<LedgeClimbV2>();
            scriptSetUp = true;
            CancelInvoke();
        }
    }

    //For detecting whether you are in a trigger zone
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && scriptSetUp == true)
        {

            if (ledgeClimb.WallInfrontOfPlayer() == true)
            {
                UIElements.singleton.ledgeClimb.enabled = true;
            }
            else
            {
                UIElements.singleton.ledgeClimb.enabled = false;
            }

            if (movement.ReturnObjectPlayerIsStandingOnAccurate() == null && grapple.IsCurrentlyGrappling() == false)
            {
                ledgeClimb.SetLedgeCollisionVariable(true);

            }
            else
            {
                ledgeClimb.SetLedgeCollisionVariable(false);
            }

        }
        else
        {
            ledgeClimb.SetLedgeCollisionVariable(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && scriptSetUp == true)
        {
            ledgeClimb.SetLedgeCollisionVariable(false);
            UIElements.singleton.ledgeClimb.enabled = false;
        }
    }
}
