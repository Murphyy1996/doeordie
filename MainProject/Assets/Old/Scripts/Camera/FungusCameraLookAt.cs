//Author: James Murphy
//Purpose: To allow isaaac to control the camera via fungus
//Placement: On the npc you are talking to

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusCameraLookAt : MonoBehaviour
{
    private Transform lookAtPoint;
    private float rotationDuration = 2f;
    private bool rotateOnAllAxis = true;
    private Camera playerCamera;
    private Transform playerTransform;

    //Get the player camera
    private void Awake()
    {
        Invoke("DelayedAwake", 0.4f);
    }

    private void DelayedAwake()
    {
        playerCamera = Camera.main;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void LookAtObj() //Will look at the desired obj
    {
        StopAllCoroutines();
        StartCoroutine(LookTowardsObj(lookAtPoint));
    }

    public void LookAtNpc() //Will return to npc
    {
        StopAllCoroutines();
        //Look at the npc again which is this transform (The npc)
        StartCoroutine(LookTowardsObj(transform));
    }

    public void SetRotationDuration(float durationToSet) //This will set the rotation duration
    {
        rotationDuration = durationToSet;
    }

    public void AllowRotationAlongAllAxis(bool value)
    {
        rotateOnAllAxis = value;
    }

    public void SetObjToLookAt(GameObject obj) //This method will set the object to look at
    {
        lookAtPoint = obj.transform;
    }

    private IEnumerator LookTowardsObj(Transform look) //This coroutine will handle all look at functions for the main camera
    {
        if (lookAtPoint != null)
        {
            float elapsedTime = 0;
            //Player rotation
            while (elapsedTime < rotationDuration)
            {
                Vector3 directionToLook = new Vector3(look.position.x, playerTransform.position.y, look.position.z) - playerTransform.position;
                //Determine the target rotation
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                // Slerp to the desired rotation over the course of this coroutine
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, (elapsedTime / rotationDuration));
                //Keep track of the elapsed time this frame
                elapsedTime += Time.deltaTime;
                if (playerTransform.rotation == targetRotation)
                {
                    playerCamera.transform.rotation = targetRotation;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
