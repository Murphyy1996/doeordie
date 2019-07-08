//Author: James Murphy
//Purpose: To create a way for the player to interact with npcs and start dialogue
//Placement: Put me on an npc and ill take care of everything
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private BoxCollider triggerZone;
    [SerializeField]
    private KeyCode interactionButton = KeyCode.E;
    [SerializeField]
    [Range(0, 15)]
    private float turningSpeed = 5f;
    private GameObject dialogueObj;
    private GameObject lookAtPlayerEmpty;
    private Transform player;
    [SerializeField]
    private bool playerInTrigger = false, canSeePlayer = false;
    [SerializeField]
    private LayerMask layersToDetect;

    //Set this object up
    private void Awake()
    {
        //Get the player object
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //Create the look at empty
        lookAtPlayerEmpty = new GameObject();
        lookAtPlayerEmpty.name = "PlayerLookAtEmpty";
        lookAtPlayerEmpty.transform.SetPositionAndRotation(transform.position, transform.rotation);
        lookAtPlayerEmpty.transform.SetParent(transform);
        //Set up the collider
        triggerZone = this.gameObject.AddComponent<BoxCollider>();
        triggerZone.isTrigger = true;
        triggerZone.size = new Vector3(triggerZone.size.x * 5f, triggerZone.size.y, triggerZone.size.z * 25);
        //Set this and all children to the npc layer
        this.gameObject.layer = 22;
        if (dialogueObj == null)
        {
            //Get the dialogue object
            if (this.tag == "FlowchartObj")
            {
                dialogueObj = this.gameObject;
            }
            else //Find the flow chart object in the children
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.layer = 22;
                    if (child.tag == "FlowchartObj")
                    {
                        dialogueObj = child.gameObject;
                    }
                }
            }
        }
        //If the dialogue object cannot be found then destroy this script as it is not needed
        if (dialogueObj == null)
        {
            Destroy(this);
        }
        else //Set the dialogue object off by default
        {
            dialogueObj.SetActive(false);
        }
    }

    private void FixedUpdate() //For the line of sight check
    {
        //Only run this when the player is nearby
        if (playerInTrigger == true)
        {
            RaycastHit rayhit;
            if (Physics.Raycast(transform.position, lookAtPlayerEmpty.transform.forward, out rayhit, 20, layersToDetect))
            {
                if (rayhit.collider.tag == "Player")
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
        }
    }

    private void Update() //For the interaction key press
    {
        //Make the NPC look at the player when the player is nearby
        if (player != null)
        {
            lookAtPlayerEmpty.transform.LookAt(player);
            if (playerInTrigger == true)
            {
                Vector3 directionToLook = player.position - transform.position;
                directionToLook.y = 0;
                //Determine the target rotation
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                // Slerp to the desired rotation over the course of this coroutine
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (turningSpeed * Time.deltaTime));
            }
        }

        //Activate dialogue if the player presses e
        if (dialogueObj != null && dialogueObj.activeSelf == false && playerInTrigger == true && canSeePlayer == true)
        {
            if (Input.GetKeyUp(interactionButton))
            {
                dialogueObj.SetActive(true);
            }
        }


        if (playerInTrigger == true)
        {
            if (dialogueObj.activeSelf == false)
            {
                UIElements.singleton.interactionText.enabled = true;
            }
            else
            {
                UIElements.singleton.interactionText.enabled = false;
            }
        }
        else
        {
            UIElements.singleton.interactionText.enabled = false;
        }
    }


    //Mark whether or not the player is in the trigger zone

    private void OnTriggerStay(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            playerInTrigger = false;
        }
    }
}
