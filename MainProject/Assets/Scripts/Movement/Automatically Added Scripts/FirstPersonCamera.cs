using System;
using UnityEngine;

//Script author: James Murphy
//Date Created: 09/032017
//Script purpose: Mouse look for the camera with clamp
//Script location: On the camera object


public class FirstPersonCamera : MonoBehaviour
{
    //Sensitivity values for camera movement and the clamp value for the y axis
    private float sensitivityY, sensitivityX, yClampValue;
    //Values for default camera sensetivity
    private float defaultSensY, defaultSensX;
    //Controller values
    private bool controllerEnabled;
    private string rightAnalogX, rightAnalogY;
    //Variables containing the camera rotation around the respective axis
    private float rotationY, rotationX;
    //Component values
    private Transform thisTransform;
    //This variable will allow other scripts to enable or disable camera movement (Useful for menus)
    private bool cameraMovementAllowed = true;
    //Rotation zone variables
    private bool onRotationZone = false;
    private CharacterControllerMovement ccMovement;
    private Transform player;
    float verticalAxis = 0;
    private bool invertedCamera = false;

    private void Awake() //Get components and set defaults
    {
        //Get the required components
        thisTransform = GetComponent<Transform>();
        //Set defaults
        Vector3 thisRotation = transform.localRotation.eulerAngles;
        rotationY = 0;
        rotationX = 0;
        //Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        //Set the player variable
        player = thisTransform.parent;
        //By default disable camera movement
        IsCameraAllowedToMove(false);
        Invoke("DelayedAwake", 0.5f);
    }

    private void DelayedAwake() //Enable the camera movement
    {
        IsCameraAllowedToMove(true);
    }

    private void LateUpdate()
    {
        //Get the character controller script
        if (ccMovement == null)
        {
            //Get the character controller movement script 
            ccMovement = player.GetComponent<CharacterControllerMovement>();
        }

        if (cameraMovementAllowed == true && Time.timeScale != 0)
        {
            if (controllerEnabled == true) //Controller Look
            {
                //Get the required movement axis and the amount of rotation needed for the camera
                rotationY += Input.GetAxis(rightAnalogX) * sensitivityY;
                rotationX += Input.GetAxis(rightAnalogY) * sensitivityX;
            }
            else //Mouse Look for no gravity mode
            {
                rotationY += Input.GetAxis("Mouse X") * sensitivityY;
                rotationX += -Input.GetAxis("Mouse Y") * sensitivityX;
            }

            float tempSensY = -sensitivityY;
            if (invertedCamera == true)
            {
                tempSensY = -tempSensY;
            }
            //Clamp the vertical axis
            verticalAxis += Input.GetAxis("Mouse Y") * tempSensY;
            verticalAxis = Mathf.Clamp(verticalAxis, -yClampValue, yClampValue);

            //Run this code if gravity is enabled
            if (ccMovement.GetGravityValue() == true)
            {
                //Rotate the players horizontal axis
                player.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
                //Rotate the camera for vertical input
                transform.localEulerAngles = new Vector3(verticalAxis, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
            else //Rotate the player vertically as well and let them fly if gravity is disabled
            {
                player.rotation = Quaternion.Euler(rotationX, rotationY, 0);
            }
        }
    }

    public void IsCameraAllowedToMove(bool value) //This public method will allow the user to control the camera movement from different scripts
    {
        cameraMovementAllowed = value;
    }

    public void SetValues(float sensY, float sensX, float clampValue, bool cameraInverted, bool controllerSupport, string rightAnalogXAxis, string rightAnalogYAxis) //When this variable is called from the Set up script it will set all of the values of this script
    {
        sensitivityY = sensY;
        sensitivityX = sensX;
        defaultSensX = sensX;
        defaultSensY = sensY;
        yClampValue = clampValue;
        //If the camera has been set to be inverted, adjust the relevant sensetivity values
        if (cameraInverted == true)
        {
            sensitivityY = -sensitivityY;
        }
        controllerEnabled = controllerSupport;
        rightAnalogY = rightAnalogYAxis;
        rightAnalogX = rightAnalogXAxis;
    }

    //Methods that can be called from other scripts
    public void ResetCameraSensitivity()
    {
        sensitivityX = defaultSensX;
        sensitivityY = defaultSensY;
    }

    public float GetCurrentXSensitivity()
    {
        return sensitivityX;
    }

    public float GetCurrentYSensitivity()
    {
        return sensitivityY;
    }

    //Method for changing the sensetivity values
    public void ChangeXSensitivity(float valueToChangeTo)
    {
        sensitivityX = valueToChangeTo;
    }

    public void ChangeYSensitivity(float valueToChangeTo)
    {
        sensitivityY = valueToChangeTo;
    }

    public void SetInvertedCamera(bool value)
    {
        invertedCamera = value;
    }
}
