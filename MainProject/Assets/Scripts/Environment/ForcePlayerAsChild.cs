//Author: James Murphy
//Purpose: To force the player as child of the selected object
//Requirements: A collider

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePlayerAsChild : MonoBehaviour
{
    [SerializeField]
    private Transform transformToChildPlayerTo;
    private GameObject createdEmpty;
    private Transform player;
    private Grapple grappleScript;
    [SerializeField]
    private bool collisionsAllowed = true;
    private bool collided = false;
    private LayerMask rayLayerMask;

    private void Start()
    {
        if (GetComponent<Collider>() == null || transformToChildPlayerTo == null)
        {
            Destroy(this);
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
            //Check for a parent empty obj, else create it
            GameObject playerHolderEmpty;
            if (GameObject.Find("MovingPlatformEmptys") == null)
            {
                playerHolderEmpty = new GameObject();
                playerHolderEmpty.name = "MovingPlatformEmptys";
            }
            else
            {
                playerHolderEmpty = GameObject.Find("MovingPlatformEmptys");
            }
            //Create the empty for this empty
            createdEmpty = new GameObject();
            createdEmpty.name = transform.root.name + " target zone empty";
            //Child the empty to the player holder empty to keep the scene tidy
            createdEmpty.transform.SetParent(playerHolderEmpty.transform);
        }
        rayLayerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("No Teleport"));
        player = GameObject.FindGameObjectWithTag("Player").transform;
        grappleScript = player.GetComponent<Grapple>();
        this.gameObject.layer = 23;
    }

    private void Update() //Make the created empted follow the target
    {
        if (createdEmpty != null && transformToChildPlayerTo != null)
        {
            createdEmpty.transform.SetPositionAndRotation(transformToChildPlayerTo.position, transformToChildPlayerTo.rotation);
        }
    }

    private void FixedUpdate() //Do a lil raycast for the heavens
    {
        if (collisionsAllowed == true && player.transform.parent == createdEmpty.transform)
        {
            bool rayHit = false;
            float rayLength = 2f;
            RaycastHit rayOut;
            //Raycast in 4 standard directions
            if (Physics.Raycast(transform.position, transform.forward, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            if (Physics.Raycast(transform.position, -transform.forward, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            if (Physics.Raycast(transform.position, transform.right, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            if (Physics.Raycast(transform.position, -transform.right, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            //Raycast diagonally too
            if (Physics.Raycast(transform.position, transform.forward + transform.right, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            if (Physics.Raycast(transform.position, -transform.forward + transform.right, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            if (Physics.Raycast(transform.position, transform.forward + -transform.right, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            if (Physics.Raycast(transform.position, -transform.forward + -transform.right, out rayOut, rayLength, rayLayerMask))
            {
                rayHit = true;
            }
            //
            if (rayHit == true)
            {
                collided = true;
                player.transform.SetParent(null);
            }
            else
            {
                collided = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (grappleScript.IsCurrentlyGrappling() == false && other.tag == "Player" && collided == false)
        {
            player.transform.SetParent(createdEmpty.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (grappleScript.IsCurrentlyGrappling() == false && other.tag == "Player")
        {
            player.transform.SetParent(null);
            collided = false;
        }
    }


}
