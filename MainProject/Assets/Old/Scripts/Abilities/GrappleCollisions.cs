//Author: James Murphy
//Purpose: Will detect collisions for grapple (It will ignore the player though), will be spawned in by the grapple script when needed
//Requirements: Rigidbody and a nice collider

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollisions : MonoBehaviour
{

    public CharacterController playerCC;
    private Transform playerTransform;
    public GameObject objectGrappledTo;
    public LayerMask rayLayerMask;
    private Grapple grappleScript;

    //This code will run when this object is enabled
    private void Awake()
    {
        Invoke("Delay", 0.6f);
    }

    private void Delay()
    {
        playerTransform = playerCC.transform;
        transform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
        transform.SetParent(playerTransform);
        grappleScript = playerTransform.GetComponent<Grapple>();
    }

    private void FixedUpdate()
    {
        //Raycsat in 8 directions, checking for a collision
        if (objectGrappledTo != null && grappleScript != null)
        {
            float rayLength = 0.5f;
            RaycastHit rayOut;
            //Raycast in 4 standard directions
            Debug.DrawRay(transform.position, transform.forward, Color.cyan);
            if (Physics.Raycast(transform.position, transform.forward, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
            Debug.DrawRay(transform.position, -transform.forward, Color.cyan);
            if (Physics.Raycast(transform.position, -transform.forward, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
            Debug.DrawRay(transform.position, transform.right, Color.cyan);
            if (Physics.Raycast(transform.position, transform.right, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
            Debug.DrawRay(transform.position, -transform.right, Color.cyan);
            if (Physics.Raycast(transform.position, -transform.right, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
            //Raycast diagonally too
            Debug.DrawRay(transform.position, transform.forward + transform.right, Color.cyan);
            if (Physics.Raycast(transform.position, transform.forward + transform.right, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
            Debug.DrawRay(transform.position, -transform.forward + transform.right, Color.cyan);
            if (Physics.Raycast(transform.position, -transform.forward + transform.right, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
            Debug.DrawRay(transform.position, transform.forward + -transform.right, Color.cyan);
            if (Physics.Raycast(transform.position, transform.forward + -transform.right, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
            Debug.DrawRay(transform.position, -transform.forward + -transform.right, Color.cyan);
            if (Physics.Raycast(transform.position, -transform.forward + -transform.right, out rayOut, rayLength, rayLayerMask))
            {
                if (rayOut.collider.gameObject != objectGrappledTo)
                {
                    grappleScript.ExitGrapple();
                    grappleScript.ForceCancelGrappleMomentum();
                }
            }
        }
    }

}
