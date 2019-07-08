//Author: James Murphy
//Purpose: To set up the gun camera

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpGunCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject gunCameraPrefab;
    private GameObject gunCameraObj;
    private Camera gunCamera;

    // Use this for initialization
    private void Awake()
    {
        //Create the gun camera
        gunCameraObj = Instantiate(gunCameraPrefab) as GameObject;
        gunCameraObj.name = "Gun Camera";
        //Set the position and rotation of the gun camera
        gunCameraObj.transform.SetParent(transform.parent);
        gunCameraObj.transform.SetPositionAndRotation(transform.position, transform.rotation);
        //Make the gun camera a child of this camera
        gunCameraObj.transform.SetParent(transform);
        //Destroy this set up script as its no longer needed
        Destroy(this);
    }
}
