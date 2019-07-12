//Author: James Murphy 12/07/19
//Purpose: Primarily for player rotation code when standing on a rotating platform
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyObjRotation : MonoBehaviour
{
    //The target obj to rotation
    public Transform targetObj;

    private void LateUpdate()
    {
        if (targetObj != null)
        {
            //Code that performs the rotation
            transform.rotation = targetObj.rotation;
        }
    }
}
