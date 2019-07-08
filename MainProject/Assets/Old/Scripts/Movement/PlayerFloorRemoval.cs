//Author: James Murphy
//Purpose: Get the player out of the floor

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloorRemoval : MonoBehaviour
{
    [SerializeField]
    private LayerMask rayLayerMask;
    [SerializeField]
    private float rayLength = 4f;
    private Crouch crouchScript;
    private Grapple grappleScript;
    private LedgeClimbV2 ledgeClimb;
    private float counter = 0, counterTarget = 0;

    private void Awake()
    {
        crouchScript = this.transform.parent.GetComponent<Crouch>();
        grappleScript = this.transform.parent.GetComponent<Grapple>();
        ledgeClimb = this.transform.parent.GetComponent<LedgeClimbV2>();
    }

    public void TempDisable(float disableTime)
    {
        counterTarget = disableTime;
        counter = 0;
    }

    private void FixedUpdate() //Only do this if standing up
    {
        counter = counter + Time.fixedDeltaTime;
        //Only run this raycast if it hasnt been temporarily disabled
        if (counter > counterTarget)
        {
            float calculatedRayLength = rayLength;
            if (crouchScript.IsPlayerCrouching() == true || crouchScript.IsPlayerSliding() == true)
            {
                calculatedRayLength = rayLength / 2;
            }
            this.gameObject.layer = 23;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, calculatedRayLength, rayLayerMask) && ledgeClimb.ReturnIfLedgeClimbing() == false)
            {
                grappleScript.ExitGrapple();
                //Force way out of the floor
                transform.parent.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            }
        }
    }
}
