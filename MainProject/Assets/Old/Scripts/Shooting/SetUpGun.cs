//Author: James Murphy
//Purpose to set up the gun placement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpGun : MonoBehaviour
{
    private void Awake() // Run the awake code slightly delayed
    {
        Invoke("DelayedAwake", 0.01f);
    }

    private void DelayedAwake() //Set this parent as the main camera
    {
        //Give the shooting script this transform
        transform.parent.GetComponent<Shooting>().GiveGunPlacementZone(transform);
        //Set this parent as the main camera
        transform.TransformDirection(Camera.main.transform.forward);
        transform.SetParent(Camera.main.transform);
        //Destroy this script as its no longer needed
        Destroy(this);
    }
}
