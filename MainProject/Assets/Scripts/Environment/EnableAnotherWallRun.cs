//Author: James Murphy
//Purpose: Enable another wallrun
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAnotherWallRun : MonoBehaviour
{
    private WallClimbV2 wallClimbScript;

    private void Start()
    {
        //Only collide with the player
        this.gameObject.layer = 23;
        //Run the delayed start code
        Invoke("DelayedStart", 0.2f);
    }

    private void DelayedStart()
    {
        try
        {
            wallClimbScript = GameObject.FindGameObjectWithTag("Player").GetComponent<WallClimbV2>();
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<MeshRenderer>().enabled = true;
        }
        catch
        {
            Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other) //Allow another wall run
    {
        if (other.tag == "Player")
        {
            wallClimbScript.AllowAnotherRun();
        }
    }
}
