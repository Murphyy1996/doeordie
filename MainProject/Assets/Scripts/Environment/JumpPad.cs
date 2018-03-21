//Author: James Murphy
//Purpose: Make the player jump
//Requirements: A collider

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class JumpPad : MonoBehaviour
{
    private CharacterControllerMovement movementScript;
    private CharacterController playerCC;
    private Crouch crouchScript;
    private Grapple grappleScript;
    private BoxCollider collider;
    [SerializeField]
    [Range(0, 100)]
    private int jumpForce = 10;
    [SerializeField]
    private bool deleteAfterUse = false;

    // Use this for initialization
    private void Start() //Get the collider
    {
        collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) //When the player is on the pad make the player jump
    {
        if (other.tag == "Player")
        {
            //Get the movement script if needed
            if (movementScript == null)
            {
                playerCC = other.gameObject.GetComponent<CharacterController>();
                movementScript = other.gameObject.GetComponent<CharacterControllerMovement>();
                crouchScript = other.gameObject.GetComponent<Crouch>();
                grappleScript = other.gameObject.GetComponent<Grapple>();
            }
            if (grappleScript.isMomentumSliding() == true)
            {
                grappleScript.ForceCancelGrappleMomentum();
            }
            //Stop any slides
            crouchScript.StopSlide();
            if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() != null)
            {
                //Run the trigger forced jump code with desired force
                movementScript.TriggerForcedJump(jumpForce);
            }
            //Delete after use
            if (deleteAfterUse == true)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
;