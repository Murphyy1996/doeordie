//Author: James Murphy
//Purpose: to control the boss level camera
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelCamera : MonoBehaviour
{
    private Camera thisCamera;
    private float defaultLocalPositionZ;
    [SerializeField]
    private float cameraResetAmount = 15f;
    [SerializeField]
    private float movementSpeed = 2f;
    private enum movementDirections { forward, backward, up, down, left, right};
    [SerializeField]
    private movementDirections selectedGlobalMovementDirection;


    private void Start()
    {
        //Get this camera is possible
        if (GetComponent<Camera>() != null)
        {
            thisCamera = GetComponent<Camera>();
        }
        //Get the local position z
        defaultLocalPositionZ = transform.localPosition.z;
    }

    private void FixedUpdate() //Move the camera when needed
    {
       if (thisCamera != null)
        {
            //Move the camera back up to the top if it has gone past its starting position
            if (transform.localPosition.z < defaultLocalPositionZ)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + cameraResetAmount);
            }
            switch (selectedGlobalMovementDirection)
            {
                case movementDirections.forward:
                    //Move the camera when possible
                    transform.Translate(Vector3.forward * (movementSpeed * Time.deltaTime), Space.World);
                    break;
                case movementDirections.backward:
                    //Move the camera when possible
                    transform.Translate(-Vector3.forward * (movementSpeed * Time.deltaTime), Space.World);
                    break;
                case movementDirections.left:
                    //Move the camera when possible
                    transform.Translate(-Vector3.right * (movementSpeed * Time.deltaTime), Space.World);
                    break;
                case movementDirections.right:
                    //Move the camera when possible
                    transform.Translate(Vector3.right * (movementSpeed * Time.deltaTime), Space.World);
                    break;
                case movementDirections.up:
                    //Move the camera when possible
                    transform.Translate(Vector3.up * (movementSpeed * Time.deltaTime), Space.World);
                    break;
                case movementDirections.down:
                    //Move the camera when possible
                    transform.Translate(-Vector3.up * (movementSpeed * Time.deltaTime), Space.World);
                    break;
            }
        }
    }


}
