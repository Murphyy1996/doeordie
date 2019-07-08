using UnityEngine;
using System.Collections;

//Script author: James Murphy
//Date Created: 09/03/2017
//Script purpose: Move the rigidbody player around
//Script location: On the player game object

public class RigidMovement : MonoBehaviour
{
    //Variables for required components
    private Rigidbody thisRigidbody;
    private Transform thisTransform, thisCameraTransform;
    private Camera thisCamera;
    //Variables for movement speed etc
    private float defaultMovementSpeed, sprintMovementSpeed, movementSpeed, jumpHeight, gravityValue, sprintSpeedPercentage;
    private KeyCode sprintKey, forwardKey, backwardKey, leftKey, rightKey, jumpKey, jumpButtonController, sprintButtonController;
    private string leftAnalogXAxis, leftAnalogYAxis;
    private bool isGravityEnabled, sprintEnabled = false, controllerEnabled, grounded = false;

    private void Awake() //Get references to all required components when this script loads
    {
        thisRigidbody = GetComponent<Rigidbody>();
        thisTransform = GetComponent<Transform>();
    }

    private void Start() //Get any values that can't be got on awake
    {
        //Get this camera transform
        thisCameraTransform = thisCamera.transform;

        //Get the default speed value
        defaultMovementSpeed = movementSpeed;
        //Work out the sprint speed
        float percentageValue = (sprintSpeedPercentage / 100) * movementSpeed;
        sprintMovementSpeed = defaultMovementSpeed + percentageValue;

        //If the player has input a null value for gravity and forgot to toggle it off, this line of code will automatically do so
        if (gravityValue == 0)
        {
            thisRigidbody.useGravity = false;
        }

        //Place rotation restrictions on the rigidbody
        thisRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate() //Decide what movement code will be run
    {
        //Decide whether to run the movement code or not
        if (grounded == true || isGravityEnabled == false)
        {
            MovementCode();
        }
    }

    private void MovementCode() //This method contains the movement code for rigidbody
    {
        //If the sprint button is pressed...
        if (Input.GetKey(sprintKey) || Input.GetKey(sprintButtonController) && controllerEnabled == true)
        {
            //Set the movement speed to sprint speed
            movementSpeed = sprintMovementSpeed;
        }
        else //Set the movement speed to the normal movement speed
        {
            movementSpeed = defaultMovementSpeed;
        }

        //Adjust the movement speed value due to the fact that the set up script is a global value for a character controller and rigidbody, causing a disparity in movement speeds
        float speed = movementSpeed * 150;

        //These variables hold the value of the joystick
        float leftAnalogXAxisValue = 0, leftAnalogYAxisValue = 0;
        //Get the value of controller axis if the controller is enabled 
        if (controllerEnabled == true)
        {
            leftAnalogXAxisValue = Input.GetAxis(leftAnalogXAxis);
            leftAnalogYAxisValue = Input.GetAxis(leftAnalogYAxis);
        }

        //This variable is for the direction the rigidbody will move to
        Vector3 direction = Vector3.zero;

        //The below code will work out the direction to go in
        //Four directional movement
        if (Input.GetKey(forwardKey) || leftAnalogYAxisValue <= -0.6f) { direction = (thisTransform.forward); }
        if (Input.GetKey(backwardKey) || leftAnalogYAxisValue >= 0.6f) { direction = (-thisTransform.forward); }
        if (Input.GetKey(leftKey) || leftAnalogXAxisValue <= -0.6f) { direction = (-thisTransform.right); }
        if (Input.GetKey(rightKey) || leftAnalogXAxisValue >= 0.6f) { direction = (thisTransform.right); }
        //Diagonal movement
        if (Input.GetKey(forwardKey) && Input.GetKey(rightKey) || leftAnalogYAxisValue <= -0.4f && leftAnalogXAxisValue >= 0.4f) { direction = (thisTransform.forward + thisTransform.right); }
        if (Input.GetKey(forwardKey) && Input.GetKey(leftKey) || leftAnalogYAxisValue <= -0.4f && leftAnalogXAxisValue <= -0.4f) { direction = (thisTransform.forward + -thisTransform.right); }
        if (Input.GetKey(backwardKey) && Input.GetKey(rightKey) || leftAnalogYAxisValue >= 0.4f && leftAnalogXAxisValue >= 0.4f) { direction = (-thisTransform.forward + thisTransform.right); }
        if (Input.GetKey(backwardKey) && Input.GetKey(leftKey) || leftAnalogYAxisValue >= 0.4f && leftAnalogXAxisValue <= -0.4f) { direction = (-thisTransform.forward + -thisTransform.right); }

        //Normalize the direction in order to avoid double movement speed when moving diagonally
        direction = (direction.normalized) * speed;

        //Although the rigidbody will not be able to move in any other direction while jumping, this is a concious decision as the rigidbody is for a more realistic character controller
        if (Input.GetKey(jumpKey) && grounded == true && isGravityEnabled == true || Input.GetKey(jumpButtonController) && controllerEnabled == true && grounded == true && isGravityEnabled == true)
        {
            //This will affect the jump height and it is adjusted by 500 to maintain parity with the character controller
            direction.y = jumpHeight * 500;
        }

        //Apply the movement
        thisRigidbody.AddForce(direction);
    }

    //The collision methods are for working out whether the player is grounded

    private void OnCollisionStay() //Work out if grounded
    {
        grounded = true;
    }

    private void OnCollisionExit() //Work out if grounded
    {
        grounded = false;
    }

    //Any public methods are below

    public void SetValues(float setMovementSpeed, float setJumpHeight, KeyCode setSprintButton, bool setSprintAllowed, float setSprintSpeed, Camera setCamToUse, bool isGravityOn, float setGravityValue, KeyCode forwardKeycode, KeyCode backwardKeycode, KeyCode rightKeycode, KeyCode leftKeycode, KeyCode jumpKeycode, KeyCode controllerJump, KeyCode controllerSprint, bool controllerAllowed, string leftAnalogX, string leftAnalogY) //When this public method is called in the set up script, all of the variables for movement will be filled
    {
        movementSpeed = setMovementSpeed;
        jumpHeight = setJumpHeight;
        thisCamera = setCamToUse;
        this.gravityValue = setGravityValue;
        isGravityEnabled = isGravityOn;
        sprintKey = setSprintButton;
        sprintEnabled = setSprintAllowed;
        sprintSpeedPercentage = setSprintSpeed;
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
    }
}
