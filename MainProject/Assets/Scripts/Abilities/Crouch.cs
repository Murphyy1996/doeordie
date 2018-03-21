//Author: James Murphy
//Date: 03/08/2017
//Purpose: To enable the player to crouch
//Location: On the player character

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    [SerializeField]
    private bool crouchAllowed = true, toggleCrouch = false, slideAllowed = true;
    [SerializeField]
    private float currentPlayerSpeed;
    [SerializeField]
    private float minSlideSpeed = 25f;
    [SerializeField]
    [Range(0.001f, 0.013f)]
    private float slideFriction = 0.008f;
    private Vector3 thisVelocity;
    private bool isSliding = false;
    [SerializeField]
    private KeyCode crouchKeycode = KeyCode.C;
    private GameObject playerCameraObj;
    private bool isCrouching = false;
    private CharacterController thisCC;
    private CharacterControllerMovement movementScript;
    private Grapple grappleScript;
    private GameObject defaultPositionObj, crouchPositionObj;
    private float colliderDefaultHeight, colliderDefaultYPosition;
    private Vector3 colliderOffsets, currentHeading;
    private float slideSpeed = 0, timeInSlide = 0;
    //This variable will keep hold of the ever decreasing slide speed time
    private bool slideExitPending = false;
    //The camera script variable
    private FirstPersonCamera fpsCamScript;
    private Transform fpsCamera;
    [SerializeField]
    private LayerMask exitCheckLayerMask;
    private bool isSprinting = false;
    private int exitPresses = 0;

    private void Awake()
    {
        Invoke("SetUpAbility", 0.4f);
    }

    private void SetUpAbility() //This method will get the required components and set up objects required for the ability
    {
        movementScript = GetComponent<CharacterControllerMovement>();
        playerCameraObj = GetComponentInChildren<Camera>().gameObject;
        defaultPositionObj = new GameObject();
        defaultPositionObj.name = "camDefaultPosEmpty";
        defaultPositionObj.transform.SetParent(transform.root);
        defaultPositionObj.transform.position = playerCameraObj.transform.position;
        crouchPositionObj = new GameObject();
        crouchPositionObj.name = "crouchPosEmpty";
        crouchPositionObj.transform.SetParent(transform.root);
        crouchPositionObj.transform.position = new Vector3(playerCameraObj.transform.position.x, playerCameraObj.transform.position.y, playerCameraObj.transform.position.z);
        thisCC = GetComponent<CharacterController>();
        colliderDefaultHeight = thisCC.height;
        colliderOffsets = thisCC.center;
        colliderDefaultYPosition = thisCC.center.y;
        fpsCamScript = GetComponentInChildren<FirstPersonCamera>();
        fpsCamera = fpsCamScript.gameObject.transform;
        grappleScript = GetComponent<Grapple>();
    }

    private void Update()
    {
        //If the player activates grapple during slide, exit the slide
        if (Input.GetMouseButton(2) && isSliding == true)
        {
            slideExitPending = true;
        }

        //Manual exit conditions for the slide will be here
        if (timeInSlide >= 0.50f && isSliding == true)
        {
            ExitSlideCode();
        }
    }

    private void FixedUpdate() //Run required methods and code on update
    {
        if (thisCC != null)
        {
            //Keep track of the current player speed
            currentPlayerSpeed = thisCC.velocity.magnitude;
            thisVelocity = thisCC.velocity;
            //Return if sprinting
            isSprinting = movementScript.ReturnIfSprinting();
        }


        //The below code will perform the actual handling of the slide
        if (slideAllowed == true && isSliding == true)
        {
            ContinueSlide();
        }
        else //Normal crouch and slide code will be run here
        {
            if (grappleScript != null && isSliding == false && grappleScript.isMomentumSliding() == false)
            {
                fpsCamScript.ResetCameraSensitivity();
            }
            //Only run the below code if the player is allowed to change the crouch state
            if (crouchAllowed == true)
            {
                //If the player can slide, bypass the normal crouch code
                if (slideAllowed == true && currentPlayerSpeed >= minSlideSpeed && isSprinting == true && Input.GetKey(crouchKeycode))
                {
                    //Make sure the measured speed is not the vertical speed
                    if (movementScript.ReturnObjectStandingOnJump() != null)
                    {
                        //If the player is not currently sliding enable the slide code and not enable multiple slides
                        if (isSliding == false)
                        {
                            StartSlide();
                        }
                    }
                }
                else //If the player cannot slide, run the normal crouch code
                {
                    ToggleCrouch();
                    AutomaticallyExitCrouchState();
                }
            }
        }

        //If the player is crouching and not sliding, force the speed
        if (isCrouching == true && isSliding == false)
        {
            movementScript.ChangeMovementSpeed(movementScript.GetMovementSpeed() / 3);
        }
    }

    private void StartSlide() //When run this code will start the slide
    {
        //Force player closer to the ground by adding lots of gravity but only when needed (Stops the glitch where you cannot slide)
        RaycastHit hitPoint;
        if (Physics.Raycast(transform.position, -transform.up, out hitPoint))
        {
            if (hitPoint.distance > 0.5800051)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);

            }
        }
        AudioManage.inst.slide.Play();
        exitPresses = 0;
        //Exit current slides
        StopSlide();
        //Mark the slide exit pending variable as false
        slideExitPending = false;
        //Reset the tracker for the time in the slide
        timeInSlide = 0;
        //Get the current heading/direction of the character controller
        currentHeading = movementScript.GetCurrentDirection();
        //Set the default slide speed
        slideSpeed = movementScript.GetMovementSpeed() / 14.5f;
        //Reduce the camera movement speed
        fpsCamScript.ChangeXSensitivity((fpsCamScript.GetCurrentXSensitivity() / 6));
        fpsCamScript.ChangeYSensitivity((fpsCamScript.GetCurrentYSensitivity() / 5));
        //Make the player crouch
        ManuallyChangeCrouchState(true);
        //Mark the player as sliding
        isSliding = true;
    }

    private void ContinueSlide() //When this code is run in the update it will performance that actual slide and detect when to end it
    {
        //Force player closer to the ground by adding lots of gravity but only when needed (Stops the glitch where you cannot slide)
        RaycastHit hitPoint;
        if (Physics.Raycast(transform.position, -transform.up, out hitPoint))
        {
            if (hitPoint.distance > 0.5800051)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
            }
        }

        //The float is the friction value
        slideSpeed = slideSpeed - slideFriction;

        //If the player can still slide let them
        if (movementScript.CheckIfControllerGroundedForSlide() == true && slideSpeed > 0 && slideExitPending == false)
        {
            //Update the time in the slide
            timeInSlide = timeInSlide + Time.deltaTime;
            //This line actually moves the player
            thisCC.Move(currentHeading * slideSpeed);
            //Disable player movement while sliding
            movementScript.IsPlayerInputEnabled(false);

            //If the player has been in the slide for more than a certain amount of time
            if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() == null && timeInSlide > 0.3f)
            {
                slideExitPending = true;
            }
        }
        else //If the player fails the slide conditions, end the slide
        {
            exitPresses = 0;
            slideExitPending = false;
            fpsCamScript.ResetCameraSensitivity();
            isSliding = false;
            movementScript.IsPlayerInputEnabled(true);
            if (AllowedToExitCrouch() == true)
            {
                ManuallyChangeCrouchState(false);
                ManuallyChangeCrouchState(true);
            }
        }
    }

    private void ExitSlideCode() //When this code is ran it will check whether the player has exited the slide
    {
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.Space)) //If any of the movement 
        {
            exitPresses++;
            if (exitPresses >= 1)
            {
                slideExitPending = true;
            }
        }
        if (movementScript.CheckIfHitWall(movementScript.GetCurrentDirection(), true, null) == true)
        {
            slideExitPending = true;
        }
    }

    private void ToggleCrouch() //This method contains the code for toggling crouch
    {
        if (thisCC != null && isSliding == false && grappleScript.ReturnIfGrappleSliding() == false)
        {
            if (toggleCrouch == true)
            {
                if (Input.GetKeyDown(crouchKeycode))
                {
                    if (isCrouching == false) //Crouch the player
                    {
                        ManuallyChangeCrouchState(true);
                    }
                    else //Go back to default height
                    {
                        if (AllowedToExitCrouch() == true)
                        {
                            ManuallyChangeCrouchState(false);
                        }
                    }
                }
            }
            else //Add in a hold crouch button
            {
                if (Input.GetKey(crouchKeycode) && isCrouching == false)
                {
                    ManuallyChangeCrouchState(true);
                }
                else if (!Input.GetKey(crouchKeycode) && isCrouching == true && AllowedToExitCrouch() == true)
                {
                    ManuallyChangeCrouchState(false);
                }
            }

        }
    }

    private void AutomaticallyExitCrouchState() //This code will be run if the player presses the jump button and is already crouched, therefore standing the player up
    {
        if (isCrouching == true)
        {
            if (Input.GetKeyUp(KeyCode.Space) && AllowedToExitCrouch() == true || Input.GetKeyDown(KeyCode.LeftShift) && AllowedToExitCrouch() == true)
            {
                ManuallyChangeCrouchState(false);
                //If sprinting is allowed on the character controller, enable it when standing back up
                if (movementScript.GetDefaultSprintValue() == true)
                {
                    movementScript.SprintEnabled(true);
                }
            }
        }
    }

    public bool AllowedToExitCrouch() //Will return a true or false saying if the player is able to exit the crouched state
    {
        RaycastHit hit;
        //If there is something above blocking standing do not stand up
        Ray origin = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.up);
        if (Physics.Raycast(origin, out hit, (colliderDefaultHeight / 1.2f), exitCheckLayerMask))
        {
            //This is a special case bug fix for rotating surfaces
            if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() == hit.collider.gameObject && movementScript.ReturnObjectPlayerIsStandingOnAccurate().GetComponent<ObjectRotation>() != null)
            {
                return true;
            }
            //This is a returns false if it hits a surface
            return false;
        }
        origin = new Ray(new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z), transform.up);
        if (Physics.Raycast(origin, out hit, (colliderDefaultHeight / 1.2f), exitCheckLayerMask))
        {
            //This is a special case bug fix for rotating surfaces
            if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() == hit.collider.gameObject && movementScript.ReturnObjectPlayerIsStandingOnAccurate().GetComponent<ObjectRotation>() != null)
            {
                return true;
            }
            //This is a returns false if it hits a surface
            return false;
        }
        origin = new Ray(new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z), transform.up);
        if (Physics.Raycast(origin, out hit, (colliderDefaultHeight / 1.2f), exitCheckLayerMask))
        {
            //This is a special case bug fix for rotating surfaces
            if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() == hit.collider.gameObject && movementScript.ReturnObjectPlayerIsStandingOnAccurate().GetComponent<ObjectRotation>() != null)
            {
                return true;
            }
            //This is a returns false if it hits a surface
            return false;
        }
        origin = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.3f), transform.up);
        if (Physics.Raycast(origin, out hit, (colliderDefaultHeight / 1.2f), exitCheckLayerMask))
        {
            //This is a special case bug fix for rotating surfaces
            if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() == hit.collider.gameObject && movementScript.ReturnObjectPlayerIsStandingOnAccurate().GetComponent<ObjectRotation>() != null)
            {
                return true;
            }
            //This is a returns false if it hits a surface
            return false;
        }
        origin = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.3f), transform.up);
        if (Physics.Raycast(origin, out hit, (colliderDefaultHeight / 1.2f), exitCheckLayerMask))
        {
            //This is a special case bug fix for rotating surfaces
            if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() == hit.collider.gameObject && movementScript.ReturnObjectPlayerIsStandingOnAccurate().GetComponent<ObjectRotation>() != null)
            {
                return true;
            }
            //This is a returns false if it hits a surface
            return false;
        }
        else
        {
            //Allow standing up if nothing is detected by the raycast
            return true;
        }
    }

    public void ManuallyChangeCrouchState(bool crouchState) //When this method is called it will allow other scripts to change the crouch state of the player, useful for cutscenes
    {
        if (crouchState == true)
        {
            //Force player closer to the ground by adding lots of gravity but only when needed (Stops the glitch where you cannot slide)
            RaycastHit hitPoint;
            if (Physics.Raycast(transform.position, -transform.up, out hitPoint))
            {
                if (hitPoint.distance > 0.5800051)
                {
                    if (movementScript.CheckIfWalking() == false || movementScript.GetCurrentSpeedandDirection().magnitude > 11)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
                    }
                    else
                    {
                        movementScript.IsPlayerInputEnabled(false);
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z);
                        movementScript.IsPlayerInputEnabled(true);
                    }
                }
            }
            movementScript.JumpEnabled(false);
            movementScript.SprintEnabled(false);
            //If the player is sliding do not change the movement speed
            if (isSliding == false)
            {
                movementScript.ChangeMovementSpeed((movementScript.GetMovementSpeed() / 3));
            }
            isCrouching = true;
            playerCameraObj.transform.position = crouchPositionObj.transform.position;
            //Change the collider size for the player
            thisCC.height = colliderDefaultHeight / 2f;
            colliderOffsets.y = colliderOffsets.y * 3f;
            thisCC.center = colliderOffsets;
        }
        else
        {
            isCrouching = false;
            if (movementScript.GetDefaultSprintValue() == true)
            {
                movementScript.SprintEnabled(true);
            }
            playerCameraObj.transform.position = defaultPositionObj.transform.position;
            movementScript.ResetMovementSpeed();
            movementScript.JumpEnabled(true);
            //Change the collider size for the player
            thisCC.height = colliderDefaultHeight;
            colliderOffsets.y = colliderDefaultYPosition;
            thisCC.center = colliderOffsets;
        }
    }

    public void ForceStandUp() //Force the player to stand up
    {
        isCrouching = false;
        if (movementScript.GetDefaultSprintValue() == true)
        {
            movementScript.SprintEnabled(true);
        }
        playerCameraObj.transform.position = defaultPositionObj.transform.position;
        movementScript.ResetMovementSpeed();
        movementScript.JumpEnabled(true);
        //Change the collider size for the player
        thisCC.height = colliderDefaultHeight;
        colliderOffsets.y = colliderDefaultYPosition;
        thisCC.center = colliderOffsets;
    }

    public bool IsPlayerCrouching() // This function will be used for checking if the player is crouching
    {
        return isCrouching;
    }

    public void StopSlide() //If called this script will stop the slide
    {
        slideExitPending = true;
    }

    public bool CheckIfSlideAllowed() //Return the slide allowed value to other scripts if needed
    {
        return slideAllowed;
    }

    public void SetSlideAllowValue(bool valueToSet) //Lets other scripts set the slide allowed value
    {
        slideAllowed = valueToSet;
    }

    public void SetCrouchAllowedValue(bool valueToSet)
    {
        crouchAllowed = valueToSet;
    }

    public bool GetCrouchAllowedValue()
    {
        return crouchAllowed;
    }

    public bool IsPlayerSliding()
    {
        return isSliding;
    }

    public void SetCrouchToggleMode(bool value)
    {
        toggleCrouch = value;
    }

    public void SetCrouchKeycode(KeyCode keycodeToSet)
    {
        crouchKeycode = keycodeToSet;
    }

}
