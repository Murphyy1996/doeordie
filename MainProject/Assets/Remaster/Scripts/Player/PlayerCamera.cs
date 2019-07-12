using System;
using UnityEngine;

//Script author: James Murphy
//Date Created: 09/03/2017
//Script purpose: Mouse look for the camera with clamp
//Script location: On the player object

public class PlayerCamera : MonoBehaviour
{
    //Inspector variables
    [Header("Configuration")]
    public GameObject cameraPrefab;
    public bool cameraEnabled = true, invert = false;
    public float fov = 75, sensUpDown, sensLeftRight, clampUpDownAngle = 100;
    //Non inspector variables that contain various position and rotation variables
    private float defaultFOV, defaultSensUpDown, defaultSensLeftRight;
    private float upDownAxis = 0;
    //Non inspector variables for transforms and scripts
    private InputManager inputManager;
    private Transform camTransform, playerTransform;
    private PlayerMovement playerMovement;

    private void Awake() //Get components and set defaults
    {
        //Get default camera settings
        defaultFOV = fov;
        defaultSensUpDown = sensUpDown;
        defaultSensLeftRight = sensLeftRight;
        //Get the player transform and movement
        playerTransform = transform;
        playerMovement = GetComponent<PlayerMovement>();
        //If possible spawn the camera
        if (cameraPrefab != null)
        {
            GameObject spawnedCamera = Instantiate(cameraPrefab, transform.position, transform.rotation) as GameObject;
            spawnedCamera.transform.SetParent(transform);
            spawnedCamera.name = "Player Camera";
            spawnedCamera.tag = "MainCamera";
            camTransform = spawnedCamera.transform;
            camTransform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
        }
        else
        {
            print("Camera prefab empty, cannot spawn");
        }
        //Get the input manager script
        inputManager = InputManager.singleton;
    }

    private void LateUpdate() //Camera movement code is here
    {
        if (cameraEnabled == true && Time.timeScale != 0)
        {
            float tempSensUpDown = sensUpDown;
            if (invert == true)
            {
                tempSensUpDown = -sensUpDown;
            }
            //Clamp the vertical axis
            upDownAxis += Input.GetAxis(inputManager.lookUpDown) * -tempSensUpDown;
            if (clampUpDownAngle > 0)
            {
                upDownAxis = Mathf.Clamp(upDownAxis, -clampUpDownAngle, clampUpDownAngle);
            }
            //Control the camera and the player rotations
            camTransform.eulerAngles = new Vector3(upDownAxis, camTransform.eulerAngles.y, camTransform.eulerAngles.z);
            transform.Rotate(0f, Input.GetAxis(inputManager.lookLeftRight) * sensLeftRight, 0f, Space.World);
        }
    }

    //Methods that can be called from other scripts
    public void ResetCameraSensitivity()
    {
        sensLeftRight = defaultSensLeftRight;
        sensUpDown = defaultSensUpDown;
    }
}
