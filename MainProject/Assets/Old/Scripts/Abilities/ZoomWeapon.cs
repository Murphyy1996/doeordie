//Author: James Murphy
//Purpose: To zoom the weapon in when a button is pressed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomWeapon : MonoBehaviour
{
    [SerializeField]
    private KeyCode zoomButton;
    [SerializeField]
    private bool zoomToggle = false;
    private bool isZooming = false;
    public float defaultFOV;
    [Range(0, 30)]
    [SerializeField]
    private float zoomAmount = 10;
    [SerializeField]
    [Range(0, 2f)]
    private float zoomSpeed = 1f;
    [SerializeField]
    [Range(0, 5)]
    private float amountToSlowDownDuringZoom = 3f;
    private Camera mainCamera, gunCamera;
    static float t = 0.0f;
    private float targetFOV;
    private Grapple grappleScript;
    private Teleporting teleportScript;
    private FirstPersonCamera fpsLook;
    private Crouch crouchScript;
    private float reducedX, reducedY;

    private void Start()
    {
        Invoke("DelayedStart", 0.05f);
    }

    private void DelayedStart()
    {
        grappleScript = GetComponent<Grapple>();
        mainCamera = Camera.main;
        fpsLook = mainCamera.GetComponent<FirstPersonCamera>();
        teleportScript = GetComponent<Teleporting>();
        crouchScript = GetComponent<Crouch>();
        reducedX = fpsLook.GetCurrentXSensitivity() / amountToSlowDownDuringZoom;
        reducedY = fpsLook.GetCurrentYSensitivity() / amountToSlowDownDuringZoom;
        gunCamera = mainCamera.gameObject.transform.Find("Gun Camera").GetComponent<Camera>();
        defaultFOV = mainCamera.fieldOfView;
        zoomAmount = mainCamera.fieldOfView - zoomAmount;
        mainCamera.fieldOfView = defaultFOV;
        gunCamera.fieldOfView = defaultFOV;

    }

    private void Update()
    {
        if (mainCamera != null)
        {
            //Control the button clicks depending if toggle or not and whether the player is in the teleport view
            if (teleportScript.ReturnIfTeleportButtonHeld() == false && QuestManager.inst.inConvo == false)
            {
                if (zoomToggle == true)
                {
                    if (Input.GetKeyDown(zoomButton))
                    {
                        isZooming = !isZooming;
                    }
                }
                else
                {
                    if (Input.GetKey(zoomButton))
                    {
                        isZooming = true;
                    }
                    else
                    {
                        isZooming = false;
                    }
                }
            }
            else //If zoom is not allowed for it as false
            {
                isZooming = false;
            }

            //Zoom the camera when needed
            if (isZooming == true && grappleScript.isHoldingEnemy() == false)
            {
                fpsLook.ChangeXSensitivity(reducedX);
                fpsLook.ChangeYSensitivity(reducedY);
                targetFOV = zoomAmount;
            }
            else
            {
                if (Time.timeScale != 0 && grappleScript.isMomentumSliding() == false && crouchScript.IsPlayerSliding() == false)
                {
                    fpsLook.ResetCameraSensitivity();
                }
                targetFOV = defaultFOV;
            }

            if (Time.timeScale != 0)
            {
                if (targetFOV > mainCamera.fieldOfView)
                {
                    //idk what do
                }

                float lerpedZoom = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, t);
                mainCamera.fieldOfView = lerpedZoom;
                gunCamera.fieldOfView = lerpedZoom;
                t += zoomSpeed * Time.deltaTime;

                if (t > zoomSpeed)
                {
                    t = 0.0f;
                }
            }


        }
    }

    public void SetZoomButton(KeyCode button)
    {
        zoomButton = button;
    }

    public void SetZoomToggle(bool value)
    {
        zoomToggle = value;
    }
}
