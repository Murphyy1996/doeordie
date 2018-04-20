//Author: James Murphy
//Purpose: when the player presses the selected button and they are in the trigger zone the activate all animations
//Requirements: Collider

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimationOnButton : MonoBehaviour
{
    [SerializeField]
    private KeyCode activationButton = KeyCode.E;
    [SerializeField]
    private Animator[] animatorComponents;
    [SerializeField]
    private GameObject[] objects;
    private bool playerInZone = false;
    private bool objActivated = false;
    private void Start() //Destroy the script if it wont work
    {
        //Only collide with the player
        this.gameObject.layer = 23;
        //Make sure this collider is a trigger
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().isTrigger = true;
        }
        else
        {
            print("script destroy as no collider");
            Destroy(this);
        }
        if (animatorComponents.Length == 0)
        {
            print("script destroy as no animations");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (playerInZone == true)
        {
            UIElements.singleton.travelIndication.enabled = true;
            if (Input.GetKey(activationButton))
            {
                foreach (Animator animator in animatorComponents)
                {
                    animator.enabled = true;
                    objActivated = true;
                }
                foreach (GameObject obj in objects)
                {
                    obj.SetActive(true);
                }
                Destroy(this);
                UIElements.singleton.travelIndication.enabled = false;
            }
        }
        else
        {
            UIElements.singleton.travelIndication.enabled = false;
        }
    }
    private void OnTriggerStay(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            playerInZone = false;
        }
    }
    public bool ReturnBool()
    {
        return objActivated;
    }

}
