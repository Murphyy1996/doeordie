using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: James Murphy
//Purpose: Unlock certain abilities at certain times

public class UnlockAbilities : MonoBehaviour
{
    private Collider thisCollider;
    private GameObject player;
    private Grapple grappleScript;
    private Teleporting teleportScript;
    private Crouch crouchScript;
    private enum ability { grapple, teleport, slide };
    [SerializeField]
    private ability abilityToEnable;
    [SerializeField]
    private bool turnOffThisMesh = false;

    // Use this for initialization
    private void Awake()
    {
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
        if (GetComponent<MeshRenderer>() != null && turnOffThisMesh == true)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        //Run delayed awake
        Invoke("DelayedAwake", 0.2f);
    }

    private void DelayedAwake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        grappleScript = player.GetComponent<Grapple>();
        teleportScript = player.GetComponent<Teleporting>();
        crouchScript = player.GetComponent<Crouch>();
    }

    private void OnTriggerEnter(Collider other) //If the player enters
    {
        if (other.tag == "Player")
        {
            //Activate the specified ability
            switch (abilityToEnable)
            {
                case ability.grapple:
                    grappleScript.SetGrappleAllowedValue(true);
                    UIElements.singleton.cooldownGrapple.enabled = true;
                    thisCollider.enabled = false;
                    break;
                case ability.teleport:
                    teleportScript.SetTeleportEnabledValue(true);
                    teleportScript.SetDefaultGrappleValue(true);
                    UIElements.singleton.cooldownTele.enabled = true;
                    thisCollider.enabled = false;
                    break;
                case ability.slide:
                    crouchScript.SetCrouchAllowedValue(true);
                    thisCollider.enabled = false;
                    break;
            }
        }
        //Start the failsafe
        StartCoroutine(StartFailSafe(0.5f, abilityToEnable));
    }

    private IEnumerator StartFailSafe(float waitTime, ability AbilityToEnable)
    {
        yield return new WaitForSeconds(waitTime);
        FailSafeEnable(AbilityToEnable);
    }

    private void FailSafeEnable(ability whatToenable) //Will ensure stuff is activated
    {
        switch (whatToenable)
        {
            case ability.grapple:
                grappleScript.SetGrappleAllowedValue(true);
                UIElements.singleton.cooldownGrapple.enabled = true;
                break;
            case ability.teleport:
                teleportScript.SetTeleportEnabledValue(true);
                teleportScript.SetDefaultGrappleValue(true);
                UIElements.singleton.cooldownTele.enabled = true;
                break;
            case ability.slide:
                crouchScript.SetCrouchAllowedValue(true);
                break;
        }
        //Destroy this script as its no longer needed
        Destroy(this.gameObject);
    }
}
