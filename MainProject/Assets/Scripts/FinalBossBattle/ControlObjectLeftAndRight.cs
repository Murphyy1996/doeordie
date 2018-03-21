using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Author: James Murphy
//Purpose: To allow the player to move the forklift left and right


public class ControlObjectLeftAndRight : MonoBehaviour
{
    [SerializeField]
    private float maxMovementAmount = 10, movementSpeed = 10;
    private Vector3 startingPosition;

    private void Start() //get the default position
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        //Create a potential movement position
        Vector3 potentialMovmentPosition = transform.position;
        if (Input.GetKey(KeyCode.A)) //Move left
        {
            potentialMovmentPosition += -transform.right * (Time.deltaTime * movementSpeed);
        }
        else if (Input.GetKey(KeyCode.D)) //Move right
        {
            potentialMovmentPosition += transform.right * (Time.deltaTime * movementSpeed);
        }
        //Clamp the xvalue
        float xValue = Mathf.Clamp(potentialMovmentPosition.x, startingPosition.x - maxMovementAmount, startingPosition.x + maxMovementAmount);
        //Force to new position
        transform.position = new Vector3(xValue, transform.position.y, transform.position.z);
    }
}
