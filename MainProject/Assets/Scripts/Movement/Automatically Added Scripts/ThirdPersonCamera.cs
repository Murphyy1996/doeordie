using UnityEngine;
using System.Collections;

//Script Author: James Murphy
//Date Created: 20/04/2017
//Script Purpose: To create a third person camera around the player
//Where the script should be placed: On the camera

public class ThirdPersonCamera : MonoBehaviour
{
    //Component variables
    private Transform player, thisTransform;
    //Variables for colliding with the wall
    private BoxCollider thisCollider;
    private Rigidbody thisRigidbody;
    private string wallTag;
    //Variables for settings the player can change
    private float sensitivityX, sensitivityY, yClampValue, distanceFromPlayer, defaultDistanceFromPlayer, cameraFriction, verticalOffset, horizontalOffset;
    private bool isGravityEnabled, zoomAllowed, controllerEnabled;
    private string rightAnalogX, rightAnalogY;
    private KeyCode cameraZoomInControllerButton, cameraZoomOutControllerButton;
    //Global variables required for the camera code
    private float rotationY, rotationX, initialX, initialY;

    private void Awake() //Get any components when the script has loaded
    {
        thisTransform = this.transform;

        //Get the starting rotation of the the camera
        rotationY = thisTransform.eulerAngles.y;
        rotationX = thisTransform.eulerAngles.x;

        //Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        //Add the required components
        thisCollider = gameObject.AddComponent<BoxCollider>();
        thisCollider.isTrigger = true;
        thisRigidbody = gameObject.AddComponent<Rigidbody>();
        thisRigidbody.useGravity = false;
    }

    private void LateUpdate()
    {
        //Zoom code is below
        if (zoomAllowed == true)
        {
            if (controllerEnabled == false) //For scroll wheel zoom
            {
                //Increase or decrease the distance from player value based off the value of the scroll wheel
                distanceFromPlayer += -Input.GetAxis("Mouse ScrollWheel") * 10;
            }
            else //For controller zoom
            {
                if (Input.GetKey(cameraZoomOutControllerButton)) //Zoom out
                {
                    distanceFromPlayer++;
                }
                else if (Input.GetKey(cameraZoomInControllerButton)) //Zoom in
                {
                    distanceFromPlayer--;
                }
            }

            //If the player is manually scrolling while the default zoom lerp is being applied, stop the co routine
            if (Input.GetKey(cameraZoomOutControllerButton) || Input.GetKey(cameraZoomInControllerButton) || Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                StopAllCoroutines();
            }

            //Clamp the zoom to stop visual glitches
            distanceFromPlayer = Mathf.Clamp(distanceFromPlayer, 0, defaultDistanceFromPlayer * 2.5f);
        }

        //Work out the amount of camera friction required
        float camFriction = cameraFriction * Time.deltaTime;


        if (controllerEnabled == true) //Input for the controller
        {
            //Get the required movement axis and apply the sensetivity value (Reduce sensetivity by dividing by 10 in order to even out variables among the set up) to the existing mouse position
            initialX += Input.GetAxis(rightAnalogX) * sensitivityX / 10;
            initialY += -Input.GetAxis(rightAnalogY) * sensitivityY / 10;
        }
        else //Input for the mouse
        {
            //Get the required movement axis and apply the sensetivity value (Reduce sensetivity by dividing by 10 in order to even out variables among the set up) to the existing mouse position
            initialX += Input.GetAxis("Mouse X") * sensitivityX / 10;
            initialY += Input.GetAxis("Mouse Y") * sensitivityY / 10;
        }

        //Smooth the camera movement by adding the "camera friction"
        initialX = Mathf.Lerp(initialX, 0, camFriction);
        initialY = Mathf.Lerp(initialY, 0, camFriction);

        //Calculate the amount of rotation required for the camera
        rotationY += initialX;
        rotationX -= initialY;

        //Clamp the amount of rotation for the camera based on whether the player has gravity enabled or not (To allow flying etc and look right up)
        float minClampValue = player.position.y - 5f;
        if (isGravityEnabled == false)
        {
            minClampValue = -yClampValue;
        }
        //This is the actual clamp code
        rotationX = Mathf.Clamp(rotationX, minClampValue, yClampValue);

        //Work out the rotation and position of the camera, whilst factoring in the distance from the player and any specified offsets
        Quaternion cameraRotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 cameraPosition = cameraRotation * new Vector3(-horizontalOffset, verticalOffset, -distanceFromPlayer) + player.position;

        //Apply the camera rotation and positons
        thisTransform.position = cameraPosition;
        thisTransform.rotation = cameraRotation;
    } //This is the code where the camera will actually be moved

    private void OnTriggerStay(Collider other) //When the camera collides with the wall, move it towards the player
    {
        if (other.gameObject.tag == wallTag)
        {
            StopAllCoroutines();
            distanceFromPlayer = distanceFromPlayer - 0.2f;
        }
    }

    private void OnTriggerExit(Collider other) //When the camera no longer collides with the wall
    {
        if (other.gameObject.tag == wallTag)
        {
            StartCoroutine(ReturnToDefaultZoom());
        }
    }

    private IEnumerator ReturnToDefaultZoom() //Lerp the camera back to default zoom
    {
        yield return new WaitForSeconds(1);

        float elapsedTime = 0;

        while (elapsedTime < 1f)
        {
            distanceFromPlayer = Mathf.Lerp(distanceFromPlayer, defaultDistanceFromPlayer, (elapsedTime / 1f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetValues(Transform playerToLookAt, float sensY, float sensX, float clampValue, float cameraDistanceFromPlayer, float cameraFrictionValue, float heightOffset, float widthOffset, bool isGravityBeingUsed, bool cameraInverted, bool isZoomAllowed, bool isControllerEnabled, string xAxisRight, string yAxisRight, KeyCode cameraZoomInButton, KeyCode cameraZoomOutButton, string wallTagToSet) //When this variable is called from the Set up script it will set all of the values of this script
    {
        player = playerToLookAt;
        sensitivityY = sensY;
        sensitivityX = sensX;
        yClampValue = clampValue;
        distanceFromPlayer = cameraDistanceFromPlayer;
        defaultDistanceFromPlayer = cameraDistanceFromPlayer;
        cameraFriction = cameraFrictionValue;
        verticalOffset = heightOffset;
        horizontalOffset = widthOffset;
        isGravityEnabled = isGravityBeingUsed;
        //If the camera has been set to be inverted, adjust the relevant sensetivity values
        if (cameraInverted == true)
        {
            sensitivityY = -sensitivityY;
        }
        zoomAllowed = isZoomAllowed;
        controllerEnabled = isControllerEnabled;
        rightAnalogY = yAxisRight;
        rightAnalogX = xAxisRight;
        cameraZoomInControllerButton = cameraZoomInButton;
        cameraZoomOutControllerButton = cameraZoomOutButton;
        wallTag = wallTagToSet;
    }
}
