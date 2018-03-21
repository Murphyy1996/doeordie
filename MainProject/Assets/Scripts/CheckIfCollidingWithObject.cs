//Author: James Murphy
//Purpose: To Return whether this object is colliding with anything
//Requirements: A collider and rigidbody

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfCollidingWithObject : MonoBehaviour
{
    [SerializeField]
    private bool isInObject = false;

    public bool CheckIfInObj()
    {
        return isInObject;
    }

    private void OnTriggerStay(Collider otherObject)
    {
        if (otherObject.tag != "Player" && otherObject.tag != "enemy")
        {
            isInObject = true;
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.tag != "Player" && otherObject.tag != "enemy")
        {
            isInObject = false;
        }
    }


}
