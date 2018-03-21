//Author: James Murphy (Based off the Original wall climb script by Ross)
//Purpose: Give the player the ability to wall climb

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimbV2 : MonoBehaviour
{
    private CharacterControllerMovement movement;
    [SerializeField]
    private float climbMomentum = 0;
    [Header("Input")]
    [SerializeField]
    private KeyCode wallRunKey = KeyCode.Space;
    private enum wallRunAxis { horizontal, vertical, none };
    [Header("Runtime Information")]
    [SerializeField]
    private bool isAgainstWall = false;
    private bool hitWallOnRight = false;
    [SerializeField]
    private bool wallRunKeyPressedDown = false, wallRunKeyBeingHeld = false, anotherRunPending = false;
    [SerializeField]
    private wallRunAxis currentWallRunAxis = wallRunAxis.none;
    [Header("General Climb Options")]
    [SerializeField]
    private string wallClimbTag = "Wall";
    [SerializeField]
    private LayerMask rayLayerMask;
    [SerializeField]
    private float climbSpeed = 5f;
    [SerializeField]
    [Range(1, 10)]
    private float climbGravity = 1;
    [SerializeField]
    [Range(0, 20)]
    private float tiltSpeed = 1, tiltAmount = 10;
    [Header("Vertical Climb Options")]
    [SerializeField]
    private float verticalRayRange = 1f;
    [Header("Horizontal Climb Options")]
    [SerializeField]
    private float horizontalRayRange = 1f;
    [SerializeField]
    private float horizontalGravityModifier = 2f;
    private RaycastHit raycastHit;
    private bool momentumResetRun = false;
    private Transform cameraTransform;
    private GameObject climbingHands, gunCamera;
    private float horizontalMovementSpeed;
    private Crouch crouchScript;
    private Grapple grappleScript;

    private void Start() //Get components and set up
    {
        //Get the climbing hands
        climbingHands = GameObject.Find("ClimbingArms");
        climbingHands.SetActive(false);
        //Run the delay start
        Invoke("DelayedStart", 0.2f);
        //Turn off the original wall climb
        if (GetComponent<WallClimb>() != null)
        {
            GetComponent<WallClimb>().enabled = false;
        }
    }

    private void DelayedStart() // Get the character controller script
    {
        //Get the crouch script
        crouchScript = GetComponent<Crouch>();
        //Get the grapple script
        grappleScript = GetComponent<Grapple>();
        //Get the movement script
        movement = GetComponent<CharacterControllerMovement>();
        //Set default vertical height momentum value
        climbMomentum = climbSpeed;
        //Get the camera object reference
        cameraTransform = Camera.main.transform;
        //Set the hands to have the camera as its parent
        climbingHands.transform.SetParent(cameraTransform);
        //Get the gun camera
        gunCamera = GameObject.Find("Gun Camera");
        //Work out the horizontal movement speed
        horizontalMovementSpeed = movement.GetMovementSpeed() + 1.7f;
    }

    private void Update() //Detect button presses
    {
        if (Input.GetKeyDown(wallRunKey))
        {
            wallRunKeyPressedDown = true;
        }

        if (Input.GetKey(wallRunKey) && crouchScript.IsPlayerCrouching() == false && wallRunKeyPressedDown == true)
        {
            wallRunKeyBeingHeld = true;
        }
        else
        {
            wallRunKeyPressedDown = false;
            wallRunKeyBeingHeld = false;
        }
    }

    private void FixedUpdate() //Run everything except the button hold in fixed update as raycasts are physics
    {
        //Only run this code when the reference for the movement script has been got
        if (movement != null)
        {
            //Run the rotation code for the camera
            CameraTiltCode();
            //Check if the player is standing on anything
            if (movement.ReturnObjectStandingOnJump() != null)
            {
                anotherRunPending = false;
                if (climbMomentum <= climbSpeed)
                {
                    if (momentumResetRun == false)
                    {
                        momentumResetRun = true;
                        StartCoroutine(Cooldown());
                    }
                }
            }
            //Run detection raycasts if the player is holding the wall run key
            if (wallRunKeyBeingHeld == true || anotherRunPending == true)
            {
                HitWallDetectionRaycasts();
            }
            else //Mark the player as not near the wall 
            {
                isAgainstWall = false;
                //Keep track of what direction the player is climbing
                currentWallRunAxis = wallRunAxis.none;
            }
            //Stop climb momentum if the player is climbing and just came off the wall
            if (isAgainstWall == false && climbMomentum < climbSpeed)
            {
                //Stop the momentum
                StopClimbMomentum();
            }
            //If the player is ready to wall climb then decide on the block of code to be run
            if (isAgainstWall == true && currentWallRunAxis != wallRunAxis.none && wallRunKeyBeingHeld == true)
            {
                WallRun();
            }
        }
    }

    private void CameraTiltCode() //Rotation code for the camera
    {
        Quaternion toRotationCamera = Quaternion.Euler(cameraTransform.localRotation.eulerAngles.x, cameraTransform.localRotation.eulerAngles.y, 0);
        switch (currentWallRunAxis)
        {
            case wallRunAxis.horizontal:
                if (climbMomentum > 0 && grappleScript.IsCurrentlyGrappling() == false)
                {
                    climbingHands.SetActive(false);
                    gunCamera.SetActive(true);
                    //Change the tilt direction depending on the side of wall the player is against
                    float tempTiltValue = tiltAmount;
                    if (hitWallOnRight == false)
                    {
                        tempTiltValue = -tiltAmount;
                    }
                    toRotationCamera = Quaternion.Euler(cameraTransform.localRotation.eulerAngles.x, cameraTransform.localRotation.eulerAngles.y, tempTiltValue);
                }
                break;
            case wallRunAxis.vertical:
                if (climbMomentum > 0)
                {
                    climbingHands.SetActive(true);
                    gunCamera.SetActive(false);
                }
                break;
            case wallRunAxis.none:
                climbingHands.SetActive(false);
                gunCamera.SetActive(true);
                break;
        }
        //This will control the rotation of the player camera
        cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, toRotationCamera, (Time.fixedDeltaTime * tiltSpeed));
    }

    private void HitWallDetectionRaycasts() //Detect if you are near a wall
    {
        //Raycast forwards
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, verticalRayRange, rayLayerMask))
        {
            //If the player is against a wall
            if (raycastHit.collider.tag == wallClimbTag)
            {
                //Mark the player as against the wall
                isAgainstWall = true;
                //Keep track of what direction the player is climbing
                currentWallRunAxis = wallRunAxis.vertical;
            }
        }
        //Raycast left
        else if (Physics.Raycast(transform.position, -transform.right, out raycastHit, horizontalRayRange, rayLayerMask))
        {
            //If the player is against a wall
            if (raycastHit.collider.tag == wallClimbTag)
            {
                hitWallOnRight = false;
                //Mark the player as against the wall
                isAgainstWall = true;
                //Keep track of what direction the player is climbing
                currentWallRunAxis = wallRunAxis.horizontal;
            }
        }
        //Raycast right
        else if (Physics.Raycast(transform.position, transform.right, out raycastHit, horizontalRayRange, rayLayerMask))
        {
            //If the player is against a wall
            if (raycastHit.collider.tag == wallClimbTag)
            {
                hitWallOnRight = true;
                //Mark the player as against the wall
                isAgainstWall = true;
                //Keep track of what direction the player is climbing
                currentWallRunAxis = wallRunAxis.horizontal;
            }
        }
        else
        {
            //Mark the player as not against the wall
            isAgainstWall = false;
            //Keep track of what direction the player is climbing
            currentWallRunAxis = wallRunAxis.none;
        }
    }

    private IEnumerator Cooldown() //Cooldown the momentum
    {
        ResetMomentum();
        wallRunKeyPressedDown = false;
        wallRunKeyBeingHeld = false;
        yield return new WaitForSeconds(0.2f);
        momentumResetRun = false;
    }

    private void ResetMomentum() //Reset the climb momentum to defaults
    {
        climbMomentum = climbSpeed;
    }

    public void RefreshHorizontalMovementSpeed() //Refresh the horizontal movement speed variable
    {
        horizontalMovementSpeed = movement.GetMovementSpeed() + 1.7f;
    }

    public void AllowAnotherRun() //Will allow jumping between walls
    {
        climbMomentum = climbSpeed;
        momentumResetRun = false;
        anotherRunPending = true;
    }

    public void StopClimbMomentum() //Stop the momentum instantly
    {
        wallRunKeyPressedDown = true;
        wallRunKeyBeingHeld = true;
        climbMomentum = 0;
        movement.ResetMovementSpeed();
    }

    private void WallRun() //This method will be used when the player runs forwards
    {
        //Climb the player at the specified rate
        if (climbMomentum > 0)
        {
            movement.TriggerForcedJump(climbMomentum);
        }
        float calculatedClimbGravity = climbGravity;
        //Increase gravity if running sideways
        if (currentWallRunAxis == wallRunAxis.horizontal && climbMomentum > 0)
        {
            calculatedClimbGravity = calculatedClimbGravity * horizontalGravityModifier;
            //Possibly make the player move faster when running horizontall to give a longer distance
            movement.ChangeMovementSpeed(horizontalMovementSpeed);
        }
        else
        {
            movement.ResetMovementSpeed();
        }
        //Reduce climb momentum and make the player fall eventually
        climbMomentum = climbMomentum - (Time.fixedDeltaTime * calculatedClimbGravity);
    }

    public bool ReturnIfWallClimbing() // This method will return if wall climbing
    {
        bool valueToReturn = true;
        if (currentWallRunAxis == wallRunAxis.none)
        {
            valueToReturn = false;
        }
        return valueToReturn;
    }
}
