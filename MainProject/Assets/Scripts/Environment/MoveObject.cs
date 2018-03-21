//Author: James Murphy
//Purpose: Move any object in the selected direction


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private enum directionToMove
    {
        forward,
        backward,
        left,
        right}

    ;

    [SerializeField]
    private directionToMove selectedDirection = directionToMove.forward;
    [SerializeField]
    [Range(0, 200f)]
    private float movementSpeed = 10f;
	
    // Move in the selected direction
    void FixedUpdate()
    {
        switch (selectedDirection)
        {
            case directionToMove.forward:
                transform.position += transform.forward * (Time.fixedDeltaTime * movementSpeed);
                break;
            case directionToMove.backward:
                transform.position += -transform.forward * (Time.fixedDeltaTime * movementSpeed);
                break;
            case directionToMove.left:
                transform.position += -transform.right * (Time.fixedDeltaTime * movementSpeed);
                break;
            case directionToMove.right:
                transform.position += transform.right * (Time.fixedDeltaTime * movementSpeed);
                break;
        }
    }
}
