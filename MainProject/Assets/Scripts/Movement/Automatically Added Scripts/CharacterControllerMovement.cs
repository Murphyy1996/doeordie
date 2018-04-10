using UnityEngine;
using System.Collections;

//Script Author: James Murphy
//Date Created: 20/04/2017
//Script Purpose: To move the object via the character controller
//Where the script should be placed: Script will be placed automatically by the set up script

[RequireComponent(typeof(BoxCollider))]
public class CharacterControllerMovement : MonoBehaviour
{
    //Variables for components
    private CharacterController thisCC;
    private BoxCollider roofFixCollider;
    private Rigidbody rigidbody;
    private Transform thisTransform, thisCameraTransform;
    private Camera thisCamera;
    //Variables for values and settings
    private bool gravityAllowed, isSprintAllowed, isGrounded = false, defaultSprintAllowedValue, controllerEnabled, reduceMovementSpeedWhilstAirborne = false;
    [SerializeField]
    private float verticalSpeed, gravityVal, movementSpeed, changedMovementSpeed, jumpHeight, defaultJumpHeight, defaultMovementSpeed, defaultChangedMovementSpeed, sprintMovementSpeed, sprintSpeedPercentageIncrease;
    private KeyCode sprintKey, forwardKey, backwardKey, leftKey, rightKey, jumpKey, jumpButtonController, sprintButtonController;
    private string leftAnalogXAxis, leftAnalogYAxis;
    //Variables related to forcing a jump from another script
    private bool forcedJumpPending = false, forcedDescentPending = false, movementSpeedChangedOnRuntime = false, playerInputEnabled = true;
    private float forcedJumpHeight, forcedDescentPercentage;
    private bool isGrappling = false, playSprintSound = false;
    private LayerMask roofLayerMask;
    private Crouch crouchScript;
    private Grapple grappleScript;
    private Vector3 lastPosition = new Vector3(0, 0, 0);
    //Variables for the sprint toggle
    private bool sprintToggleMode = false, isInSprint = false, regularJumpPending = false, bunnyHopEnabled = false;
    private GameObject jumpCheckForward, jumpCheckBack, JumpCheckLeft, JumpCheckRight;
    private bool pendingGravityReset = false;
    private WallClimbV2 wallClimbScript;

    //Variables for tracking the direction the player has input for the character
    private enum directions
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    ;

    private directions playerInputDirection;
    //The below variable will get the current speed and direction of this character controller incase it needs to be manipulated from another source
    private Vector3 currentSpeedandDirection, currentDirection;

    private void Start() //Get all required components at start up and check to see if axis are valid
    {
        thisCC = GetComponent<CharacterController>();
        thisTransform = GetComponent<Transform>();
        thisCameraTransform = thisCamera.transform;
        roofFixCollider = GetComponent<BoxCollider>();
        roofFixCollider.center = new Vector3(0, 0.5f, 0);
        roofFixCollider.isTrigger = true;
        if (GetComponent<Crouch>() != null)
        {
            crouchScript = GetComponent<Crouch>();
            grappleScript = GetComponent<Grapple>();
        }
        if (GetComponent<WallClimbV2>() != null)
        {
            wallClimbScript = GetComponent<WallClimbV2>();
        }
        //Get the value for the default movement speed
        defaultMovementSpeed = movementSpeed;
        //Get the default jump height value
        defaultJumpHeight = jumpHeight;
        //Get the default sprint allowed value
        defaultSprintAllowedValue = isSprintAllowed;
        //Work out the sprint speed
        float percentageValue = (sprintSpeedPercentageIncrease / 100) * movementSpeed;
        sprintMovementSpeed = defaultMovementSpeed + percentageValue;
        rigidbody = this.gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;


    }

    private void FixedUpdate()
    {
        if (GetComponent<CharacterController>() == null)//If the script detects missing component
        {
            Debug.Log("Make sure you DO NOT have a character controller placed on the same object as the 'Set up' script by default. As a result no custom values could be applied to the character controller.");
            //A temporary character controller has been added
            thisCC = this.gameObject.AddComponent<CharacterController>();
        }
        //Only run the movement code if player input is enabled (Useful for cutscenes)
        if (playerInputEnabled == true)
        {
            //Run the movement code
            MovementCode();
        }
        //Check to see if the player has hit the roof
        if (AccurateIsControllerGrounded() == false)
        {
            StopCeilingGlitch();
        }

    }
    //Run any movement code in a fixed update to make sure it doesn't vary with the frame rate

    private void Update() //Run key presses for sprinting and jumping
    {
        //Detect the key presses for jumping
        if (bunnyHopEnabled == false)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                regularJumpPending = true;
            }
        }
        else
        {
            if (Input.GetKey(jumpKey))
            {
                regularJumpPending = true;
            }
        }
        if (sprintToggleMode == true)
        {
            //Set the sprint value if the player is has toggle to sprint on
            if (Input.GetKeyDown(sprintKey))
            {
                //The is in sprint value will equal the opposite is in sprint value
                isInSprint = !isInSprint;
            }
        }
        CheckIfWalking();
    }

    public LayerMask ReturnRoofLayerMask() //Return the roof layer mask
    {
        return roofLayerMask;
    }

    public bool CheckIfWalking() //Will return whether the player is moving
    {
        if (AccurateIsControllerGrounded() == true)
        {
            if (thisCC.velocity.magnitude != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public GameObject ReturnObjectStandingOnJump() //A Super accurate test of what the player is standing on
    {
        //These variables adjust all of the raycasts
        RaycastHit rayout;
        float height = 0.2f;
        float length = height + 0.2f;
        //This will set up all of the positions of the raycasts
        if (jumpCheckForward == null)
        {
            //Forward jump check block
            jumpCheckForward = new GameObject();
            jumpCheckForward.name = "ForwardEmpty";
            jumpCheckForward.transform.SetParent(transform);
            jumpCheckForward.transform.localPosition = transform.right * -0.5f;
            jumpCheckForward.transform.localPosition = new Vector3(jumpCheckForward.transform.localPosition.x, height, jumpCheckForward.transform.localPosition.z);
            jumpCheckForward.transform.rotation = transform.rotation;
            //Backward jump check block
            jumpCheckBack = new GameObject();
            jumpCheckBack.name = "BackEmpty";
            jumpCheckBack.transform.SetParent(transform);
            jumpCheckBack.transform.localPosition = transform.right * 0.5f;
            jumpCheckBack.transform.localPosition = new Vector3(jumpCheckBack.transform.localPosition.x, height, jumpCheckBack.transform.localPosition.z);
            jumpCheckBack.transform.rotation = transform.rotation;
            //Left jump check block
            JumpCheckLeft = new GameObject();
            JumpCheckLeft.name = "LeftEmpty";
            JumpCheckLeft.transform.SetParent(transform);
            JumpCheckLeft.transform.localPosition = transform.forward * -0.5f;
            JumpCheckLeft.transform.localPosition = new Vector3(JumpCheckLeft.transform.localPosition.x, height, JumpCheckLeft.transform.localPosition.z);
            JumpCheckLeft.transform.rotation = transform.rotation;
            //Right jump check block
            JumpCheckRight = new GameObject();
            JumpCheckRight.name = "RightEmpty";
            JumpCheckRight.transform.SetParent(transform);
            JumpCheckRight.transform.localPosition = transform.forward * 0.5f;
            JumpCheckRight.transform.localPosition = new Vector3(JumpCheckRight.transform.localPosition.x, height, JumpCheckRight.transform.localPosition.z);
            JumpCheckRight.transform.rotation = transform.rotation;

        }
        //Ray in the middle
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        if (Physics.Raycast(rayStart, -transform.up, out rayout, (thisCC.height / 2) + length, roofLayerMask))
        {
            return rayout.collider.gameObject;
        }
        //Forward
        if (Physics.Raycast(jumpCheckForward.transform.position, -transform.up, out rayout, (thisCC.height / 2) + length, roofLayerMask))
        {
            return rayout.collider.gameObject;
        }
        //Back
        if (Physics.Raycast(jumpCheckBack.transform.position, -transform.up, out rayout, (thisCC.height / 2) + length, roofLayerMask))
        {
            return rayout.collider.gameObject;
        }
        //Left
        if (Physics.Raycast(JumpCheckLeft.transform.position, -transform.up, out rayout, (thisCC.height / 2) + length, roofLayerMask))
        {
            return rayout.collider.gameObject;
        }
        //Right
        if (Physics.Raycast(JumpCheckRight.transform.position, -transform.up, out rayout, (thisCC.height / 2) + length, roofLayerMask))
        {
            return rayout.collider.gameObject;
        }
        //Return that the object is standing on nothing
        return null;
    }

    private void MovementCode() //This contains the movement code
    {
        if (isSprintAllowed == true) //Only allow any of the code to run if sprint has been enabled
        {
            //Work out whether the controller is grounded
            isGrounded = AccurateIsControllerGrounded();

            //If it is hold to sprint
            if (sprintToggleMode == false)
            {
                //If the sprint button is pressed...and the character isn't flying
                if (Input.GetKey(sprintKey) || Input.GetKey(sprintButtonController) && controllerEnabled == true)
                {
                    isInSprint = true;
                    //Set the movement speed to sprint speed
                    playSprintSound = true;

                    movementSpeed = sprintMovementSpeed;
                    //If the movement speed has been changed, adjust the sprint accordingly

                    if (movementSpeedChangedOnRuntime == true)
                    {
                        float percentageValue = (sprintSpeedPercentageIncrease / 100) * changedMovementSpeed;
                        changedMovementSpeed = changedMovementSpeed + percentageValue;

                    }

                }
                else //Set the movement speed to the normal movement speed
                {
                    isInSprint = false;
                    movementSpeed = defaultMovementSpeed;
                    playSprintSound = false;
                }
            }
            else //If it is toggle to sprint
            {
                if (isInSprint == true) //Sprint movement speed
                {
                    //If the player isn't grounded slow 
                    if (isGrounded == false)
                    {
                        //Set the movement speed to sprint speed
                        movementSpeed = sprintMovementSpeed;
                        playSprintSound = true;
                    }

                    //If the movement speed has been changed, adjust the sprint accordingly
                    if (movementSpeedChangedOnRuntime == true)
                    {
                        float percentageValue = (sprintSpeedPercentageIncrease / 100) * changedMovementSpeed;
                        changedMovementSpeed = changedMovementSpeed + percentageValue;

                    }
                }

                else //Default movement speed
                {
                    playSprintSound = false;
                    movementSpeed = defaultMovementSpeed;
                }
            }
        }
        else
        {
            movementSpeed = defaultMovementSpeed;
        }

        //Adjust the movement speed value due to the fact that the set up script is a global value for a character controller and rigidbody, causing a disparity in movement speeds
        float speed = movementSpeed / 20;

        //Reduce the movement speed whilst airborne
        if (reduceMovementSpeedWhilstAirborne == true && isGrounded == false)
        {
            //Adjust the speed
            speed = speed / 2;
            //Adjust the changed movement speed
            if (movementSpeedChangedOnRuntime == true)
            {
                changedMovementSpeed = changedMovementSpeed / 2;
            }
        }

        //If the movement speed has been changed on runtime by another script for a power up for example
        if (movementSpeedChangedOnRuntime == true)
        {
            speed = changedMovementSpeed / 20;
            //Remove any modifiers on the changed movement speed to stop infinite speed sprint glitches
            changedMovementSpeed = defaultChangedMovementSpeed;
        }

        //These variables hold the value of the joystick
        float leftAnalogXAxisValue = 0, leftAnalogYAxisValue = 0;
        //Get the value of controller axis if the controller is enabled 
        if (controllerEnabled == true)
        {
            leftAnalogXAxisValue = Input.GetAxis(leftAnalogXAxis);
            leftAnalogYAxisValue = Input.GetAxis(leftAnalogYAxis);
        }

        //Adjust the gravity value due to disparities between values and scripts
        float adjustedGravityVal = gravityVal / 80;

        //This variable is for the direction the rigidbody will move to
        Vector3 direction = Vector3.zero;

        //The below code will work out the direction to go in from the key inputs and controller inputs
        //Four directional movement
        if (Input.GetKey(forwardKey) || leftAnalogYAxisValue <= -0.6f)
        {
            direction = (thisTransform.forward);
            playerInputDirection = directions.North;
        }
        if (Input.GetKey(backwardKey) || leftAnalogYAxisValue >= 0.6f)
        {
            direction = (-thisTransform.forward);
            playerInputDirection = directions.South;
        }
        if (Input.GetKey(leftKey) || leftAnalogXAxisValue <= -0.6f)
        {
            direction = (-thisTransform.right);
            playerInputDirection = directions.West;
        }
        if (Input.GetKey(rightKey) || leftAnalogXAxisValue >= 0.6f)
        {
            direction = (thisTransform.right);
            playerInputDirection = directions.East;
        }
        //Diagonal movement
        if (Input.GetKey(forwardKey) && Input.GetKey(rightKey) || leftAnalogYAxisValue <= -0.4f && leftAnalogXAxisValue >= 0.4f)
        {
            direction = (thisTransform.forward + thisTransform.right);
            playerInputDirection = directions.NorthEast;
        }
        if (Input.GetKey(forwardKey) && Input.GetKey(leftKey) || leftAnalogYAxisValue <= -0.4f && leftAnalogXAxisValue <= -0.4f)
        {
            direction = (thisTransform.forward + -thisTransform.right);
            playerInputDirection = directions.NorthWest;
        }
        if (Input.GetKey(backwardKey) && Input.GetKey(rightKey) || leftAnalogYAxisValue >= 0.4f && leftAnalogXAxisValue >= 0.4f)
        {
            direction = (-thisTransform.forward + thisTransform.right);
            playerInputDirection = directions.SouthEast;
        }
        if (Input.GetKey(backwardKey) && Input.GetKey(leftKey) || leftAnalogYAxisValue >= 0.4f && leftAnalogXAxisValue <= -0.4f)
        {
            direction = (-thisTransform.forward + -thisTransform.right);
            playerInputDirection = directions.SouthWest;
        }

        //Record the current direction if the user needs to know the heading from other scripts
        currentDirection = direction.normalized;

        //Normalize the direction in order to avoid double movement speed when moving diagonally
        direction = (direction.normalized) * speed;
        //If the character controller isn't grounded...apply gravity
        if (ReturnObjectStandingOnJump() == null && gravityAllowed == true && forcedJumpPending == false && isGrappling == false)
        {
            regularJumpPending = false;
            if (pendingGravityReset == false)
            {
                //Apply gravity acceleration to vertical speed
                verticalSpeed -= adjustedGravityVal * Time.deltaTime;
            }
            else
            {
                verticalSpeed = adjustedGravityVal * Time.deltaTime;
                pendingGravityReset = false;
            }
            //If the character needs it vertical momentum reduced due to hitting a ceiling or a non player controller movement for a cutscene
            if (forcedDescentPending == true)
            {
                //Reduce vertical to zero if bigger than zero
                if (verticalSpeed >= 0)
                {
                    //Work out the percentage to reduce the vertical momentum by
                    verticalSpeed = (verticalSpeed / forcedDescentPercentage * 100) * -(Time.deltaTime * 2);
                }
                else //Make the player fall
                {
                    verticalSpeed -= adjustedGravityVal * Time.deltaTime;
                }
                //Turn off forced descent pending else the player will alwyas be stuck with no vertical momentum
                forcedDescentPending = false;
            }
            //Set the vertical speed
            direction.y = verticalSpeed;
        }
        else //Do not apply gravity when grounded
        {
            verticalSpeed = 0;

            //Only allow jumping when gravity is enabled as there is no point when you are floating
            if (gravityAllowed == true)
            {
                //Jump when the space button is pressed if jump is enabled
                if (regularJumpPending == true || Input.GetKey(jumpButtonController) && controllerEnabled == true || forcedJumpPending == true)
                {
                    if (regularJumpPending == true)
                    {
                        regularJumpPending = false;
                    }
                    //If forced jump is pending add the extra height required
                    if (forcedJumpPending == true)
                    {
                        verticalSpeed = verticalSpeed + forcedJumpHeight / 25;
                        //Turn off the pending value, else the player will jump forever
                        forcedJumpPending = false;
                    }
                    else
                    {
                        //Adjust the jump height value to the globalisation of all values
                        verticalSpeed = jumpHeight / 25;
                    }

                    //Adjust the y value of the vector 3, therefore making the player jump
                    direction.y = verticalSpeed;
                }
            }
        }

        //The current velocity value (the direction variable) will be stored if it is needed for other scripts
        currentSpeedandDirection = direction;

        //Apply the movement
        thisCC.Move(currentSpeedandDirection);
    }

    public void GravityVelocityReset()
    {
        pendingGravityReset = true;
    }

    //Below are functions that can be used from other scripts for functionality

    public bool GetDefaultSprintValue()
    {
        return defaultSprintAllowedValue;
    }

    public void SetJumpHeight(float value)
    {
        defaultJumpHeight = value;
        jumpHeight = value;
    }

    public void SetDefaultMovementSpeed(float value)
    {
        defaultMovementSpeed = value;
        float percentageValue = (sprintSpeedPercentageIncrease / 100) * defaultMovementSpeed;
        sprintMovementSpeed = defaultMovementSpeed + percentageValue;
        print(defaultMovementSpeed + " , " + sprintMovementSpeed);
    }

    public void SetIsGrapplingValue(bool value) //Other scripts can set the is grappling value
    {
        isGrappling = value;
    }

    //Get the default sprint enabled or disabled value

    public Vector3 GetCurrentSpeedandDirection() //Return a vector 3 with the current speed and direction value
    {
        return currentSpeedandDirection;
    }

    public GameObject ReturnObjectPlayerIsStandingOnAccurate() //Return the object the player is standing on
    {
        RaycastHit rayout;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        if (Physics.Raycast(rayStart, -transform.up, out rayout, (thisCC.height / 2) + 2.1f, roofLayerMask))
        {
            return rayout.collider.gameObject;
        }
        return null;
    }

    public GameObject ReturnObjectPlayerIsStandingOnGrappleCheck() //Return the object the player is standing on
    {
        RaycastHit rayout;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        if (Physics.Raycast(rayStart, -transform.up, out rayout, (thisCC.height / 2) + 1.1f, roofLayerMask))
        {
            return rayout.collider.gameObject;
        }
        return null;
    }

    public bool CheckIfControllerGroundedForSlide() //Will return whether the controller is grounded enough for a slide
    {
        if (thisCC.velocity.y >= -9 && thisCC.velocity.y <= 9)
        {
            return true;
        }
        return false;
    }

    private void StopCeilingGlitch() //Will raycast up and stop a glitch where the player gets stuck in the ceiling
    {
        if (crouchScript != null && crouchScript.IsPlayerSliding() == false && grappleScript.IsCurrentlyGrappling() == false)
        {
            RaycastHit ceilingGlitchHit;
            if (Physics.Raycast(transform.position, transform.up, out ceilingGlitchHit, 1.09f, roofLayerMask))
            {
                if (wallClimbScript != null)
                {
                    //Stop climb momentum
                    wallClimbScript.StopClimbMomentum();
                }
                TriggerForcedDescent(100f);
            }
        }
    }

    public bool AccurateIsControllerGrounded() //Super accurate is controller grounded
    {
        if (thisCC.velocity.y <= 0.2f && thisCC.velocity.y >= -0.2f)
        {
            return true;
        }
        return false;
    }

    public void GravityEnabled(bool value) //Allow another script to control the gravity enabled value
    {
        gravityAllowed = value;
    }

    public void SetGravityStrength(float strength)
    {
        gravityVal = strength;
    }

    public float GetCurrentGravityStrength()
    {
        return gravityVal;
    }

    public void SetSprintKeycode(KeyCode key) //Set the sprint keycode
    {
        sprintKey = key;
    }
    public string GetPlayerInputDirection() //This will return the heading as a lower case string in a 1 or 2 letter format
    {
        switch (playerInputDirection)
        {
            case directions.North:
                return "n";
            case directions.NorthEast:
                return "ne";
            case directions.East:
                return "e";
            case directions.SouthEast:
                return "se";
            case directions.South:
                return "s";
            case directions.SouthWest:
                return "sw";
            case directions.West:
                return "w";
            case directions.NorthWest:
                return "nw";
        }

        //If the switch is not fulfilled due to standing still then return it as forward
        return "n";
    }

    public Vector3 GetCurrentDirection() //This will return the current direction the character controller is heading
    {
        return currentDirection;
    }

    public bool GetGravityValue() //Returns whether gravity is allowed
    {
        return gravityAllowed;
    }

    public void IsPlayerInputEnabled(bool value) //This will allow the user to enable or disable player input
    {
        playerInputEnabled = value;
    }

    public float GetMovementSpeed() //Returns the value of the movement speed
    {
        return movementSpeed;
    }

    public void ChangeMovementSpeed(float newMovementSpeed) //This method will allow the user to adjust the movement speed from other scripts
    {
        movementSpeedChangedOnRuntime = true;
        changedMovementSpeed = newMovementSpeed;
        defaultChangedMovementSpeed = newMovementSpeed;
    }

    public void ResetMovementSpeed() //Resets movement speed back to default
    {
        movementSpeedChangedOnRuntime = false;
        movementSpeed = defaultMovementSpeed;
    }

    public void StopAllMovement() //Calling this method will stop on character movement
    {
        thisCC.Move(new Vector3(0, 0, 0));
    }

    public void SprintEnabled(bool value) //Allows the user to enable or disable sprint from different scripts during runtime
    {
        if (value == true)
        {
            isSprintAllowed = true;
        }
        else
        {
            isSprintAllowed = false;
        }
    }

    public void JumpEnabled(bool value) //This will allow you to enable or disable jump from other scripts
    {
        if (value == true) //Give jumping the default value
        {
            jumpHeight = defaultJumpHeight;
        }
        else //Give jump a null value, therefore disabling the jump
        {
            jumpHeight = 0;
        }
    }

    public float ReturnJumpHeight() //Return jump height
    {
        return jumpHeight;
    }

    public void TriggerForcedJump(float jumpForce) //This function will force a jump if called (Useful for jump pads etc) - This option is exclusive to character controller as you can just add a force to rigidbody
    {
        //Set the forced jump height
        forcedJumpHeight = jumpForce;
        //Let the game know a forced jump is pending so it will know to do it in the next update
        forcedJumpPending = true;
    }

    public void TriggerForcedDescent(float percentageToReduceVerticalMomentum) //When this function is called it will allow for other scripts to manually force the character to descend by reducing vertical velocity by a certain percentage
    {
        forcedDescentPercentage = percentageToReduceVerticalMomentum;
        forcedDescentPending = true;
    }

    public bool CheckIfPlayerInputEnabled()
    {
        return playerInputEnabled;
    }

    public bool CheckIfHitWall(Vector3 direction, bool isSliding, GameObject grappleObject) //This will check if the player hits a wall while being flung off something with momentum
    {
        Vector3 rayStartPosition = transform.position;
        RaycastHit wallHitCheck;
        float rayLength = 2f;
        Vector3 rayDirection = direction;
        //
        for (int i = 0; i < 5; i++)
        {
            //Decide where the ray starts
            switch (i)
            {
                //Do the initial ray from the middle of the player
                case 0:
                    rayStartPosition = transform.position;
                    rayDirection = direction;
                    rayLength = 1.2f;
                    break;

                //Do the second ray from the top of the player
                case 1:
                    rayStartPosition = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
                    rayDirection = direction;
                    rayLength = 1.2f;
                    break;

                //Do the third ray from the players feet
                case 2:
                    if (isSliding == false)
                    {
                        rayStartPosition = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);
                        rayDirection = direction;
                        rayLength = 1.2f;
                    }
                    break;
                //Raycast to the left of the player
                case 3:
                    rayStartPosition = transform.position;
                    rayDirection = transform.right;
                    rayLength = 1.2f;
                    break;
                //Raycast to the right of the player
                case 4:
                    rayStartPosition = transform.position;
                    rayDirection = -transform.right;
                    rayLength = 1.2f;
                    break;
            }
            Debug.DrawRay(rayStartPosition, rayDirection, Color.red, rayLength);
            //This is the actual raycast
            if (Physics.Raycast(rayStartPosition, rayDirection, out wallHitCheck, rayLength, roofLayerMask))
            {
                if (grappleObject == null)
                {
                    return true;
                }
                else if (grappleObject != null && grappleObject != wallHitCheck.collider.gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetSprintToggleBool(bool value) //Set whether this sprint is in toggle
    {
        sprintToggleMode = value;
    }

    public bool ReturnIfSprinting() //Return whether the player is sprinting
    {
        return isInSprint;
    }

    public float ReturnDefaultJumpHeight()
    {
        return jumpHeight;
    }

    public void BossLevelQuickReset() // Will force all movement settings to standard
    {
        SetValues(true, false, 45, Camera.main, 5, 6, sprintKey, true, sprintSpeedPercentageIncrease, forwardKey, backwardKey, rightKey, leftKey, jumpKey, jumpButtonController, sprintButtonController, false, leftAnalogXAxis, leftAnalogYAxis, roofLayerMask, sprintToggleMode, false);
        //Get the default sprint allowed value
        defaultSprintAllowedValue = true;
        ChangeMovementSpeed(5);
        defaultMovementSpeed = 5;
        defaultJumpHeight = 6;
        jumpHeight = 6;
        JumpEnabled(true);
        gravityAllowed = true;
        float percentageValue = (sprintSpeedPercentageIncrease / 100) * movementSpeed;
        sprintMovementSpeed = defaultMovementSpeed + percentageValue;
        SprintEnabled(true);
    }

    public void SetValues(bool gravityIsEnabled, bool reduceMovementInAir, float gravityValueToSet, Camera setCamToUse, float setMovementSpeed, float setJumpHeight, KeyCode setSprintButton, bool setSprintAllowed, float setSprintSpeed, KeyCode forwardKeycode, KeyCode backwardKeycode, KeyCode rightKeycode, KeyCode leftKeycode, KeyCode jumpKeycode, KeyCode controllerJump, KeyCode controllerSprint, bool controllerAllowed, string leftAnalogX, string leftAnalogY, LayerMask roofMask, bool sprintToggle, bool bunnyHop) //When called this will set all the values required on the script
    {
        gravityAllowed = gravityIsEnabled;
        reduceMovementSpeedWhilstAirborne = reduceMovementInAir;
        gravityVal = gravityValueToSet;
        thisCamera = setCamToUse;
        movementSpeed = setMovementSpeed;
        jumpHeight = setJumpHeight;
        sprintKey = setSprintButton;
        isSprintAllowed = setSprintAllowed;
        sprintSpeedPercentageIncrease = setSprintSpeed;
        forwardKey = forwardKeycode;
        backwardKey = backwardKeycode;
        leftKey = leftKeycode;
        rightKey = rightKeycode;
        jumpKey = jumpKeycode;
        jumpButtonController = controllerJump;
        sprintButtonController = controllerSprint;
        controllerEnabled = controllerAllowed;
        leftAnalogYAxis = leftAnalogY;
        leftAnalogXAxis = leftAnalogX;
        roofLayerMask = roofMask;
        sprintToggleMode = sprintToggle;
        bunnyHopEnabled = bunnyHop;
    }
}
