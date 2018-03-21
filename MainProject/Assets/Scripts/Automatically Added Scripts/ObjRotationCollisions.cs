//Author: James Murphy
//Purpose: Detect collisions on this trigger
// DO NOT PLACE THIS MANUALLY (Use Object Rotation instead)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotationCollisions : MonoBehaviour
{
    private GameObject player;
    private CharacterControllerMovement ccMovement;
    private Grapple grappleScript;
    private GameObject emptyRotationCube, parentObj;
    private Vector3 defaultPlayerScale;

    //Get required components
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ccMovement = player.GetComponent<CharacterControllerMovement>();
        grappleScript = player.GetComponent<Grapple>();
        defaultPlayerScale = player.transform.localScale;
    }

    private void Update() //If the player is not grounded and still parented to this object, un parent
    {
        if (emptyRotationCube != null)
        {
            if (ccMovement.AccurateIsControllerGrounded() == false && player.transform.parent == emptyRotationCube.transform && grappleScript.IsCurrentlyGrappling() == false)
            {
                player.transform.SetParent(null);
                emptyRotationCube = null;
                //Reset player scale to default as occasional glitches happen due to intersecting colliders
                player.transform.localScale = defaultPlayerScale;
            }
        }
    }

    private void OnTriggerStay(Collider otherObject) //When the player is on this object set it as a child to gain the rotation
    {
        if (player != null)
        {
            if (ccMovement.ReturnObjectPlayerIsStandingOnAccurate() == parentObj && ccMovement.AccurateIsControllerGrounded() == true && otherObject.gameObject == player && grappleScript.IsCurrentlyGrappling() == false)
            {
                //Destroy any left over rotation cubes
                if (emptyRotationCube != null && player.transform.parent != emptyRotationCube.transform)
                {
                    player.transform.SetParent(null);
                    emptyRotationCube = null;
                }
                if (emptyRotationCube == null)
                {
                    emptyRotationCube = new GameObject();
                    emptyRotationCube.transform.position = transform.position;
                    emptyRotationCube.transform.rotation = transform.rotation;
                    emptyRotationCube.name = "EmptyRotationCube";
                    Optimization.singleton.AddRotationZoneToList(emptyRotationCube);
                }
                //Make the rotation cube follow the rotation of this object
                emptyRotationCube.transform.rotation = transform.rotation;
                //Make the player a child of the rotation cube so the scale does not get messed with
                if (player.transform.parent == null)
                {
                    player.transform.SetParent(emptyRotationCube.transform);
                    emptyRotationCube.transform.SetParent(transform);
                }
            }
        }
    }

    public void SetParentObj(GameObject objectToSet) //Set the parent object of this
    {
        parentObj = objectToSet;
    }

    public GameObject ReturnParentObj() //Return the parent obj
    {
        return parentObj;
    }
}
