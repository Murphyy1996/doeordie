using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceAbilities : MonoBehaviour
{
    private Grapple grappleScript;
    private Teleporting teleportScript;

    private void Awake()
    {
        grappleScript = GetComponent<Grapple>();
        teleportScript = GetComponent<Teleporting>();
        Invoke("DelayedDisableAbilities", 1f);
    }

    private void DelayedDisableAbilities()
    {
        if (SceneManager.GetActiveScene().name == "Level1Bl2" || SceneManager.GetActiveScene().name == "Boss")
        {
            grappleScript.SetGrappleAllowedValue(false);
            teleportScript.SetTeleportEnabledValue(false);
        }
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Level1Bl2" || SceneManager.GetActiveScene().name == "Boss")
        {
            int error = 0;
        }
        else
        {
            grappleScript.SetGrappleAllowedValue(true);
            teleportScript.SetTeleportEnabledValue(true);
        }
    }
}
