//Author: James Murphy
//Purpose: This script will make the object pick up able
//Requirements: Nothing, this script will make sure the object has all things required

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAbleObj : MonoBehaviour
{
    private LayerMask throwCollisionMask;
    private Rigidbody thisRB;
    private Collider thisCollider;
    public float setDrag = 0;

    // Use this for initialization
    private void Start() //Set up this object
    {
        //Set up this layer mask
        throwCollisionMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Teleport") | 1 << LayerMask.NameToLayer("No Teleport") | 1 << LayerMask.NameToLayer("Drone"));
        //Set the tag
        this.gameObject.tag = "MoveableObjects";
        //Make sure this object has the required components
        if (GetComponent<Collider>() == null)
        {
            thisCollider = this.gameObject.AddComponent<BoxCollider>();
        }
        else
        {
            thisCollider = GetComponent<Collider>();
        }
        //Check if there is an existing rigidbody
        if (GetComponent<Rigidbody>() == null)
        {
            thisRB = this.gameObject.AddComponent<Rigidbody>();
        }
        else
        {
            thisRB = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        //Start the thrown code
        if (thisRB.velocity.magnitude >= 10 && setDrag <= 3 || thisRB.velocity.magnitude >= 4 && setDrag > 3)
        {
            thisRB.drag = setDrag;
            BeingThrownCode();
        }
    }

    //Detect collisions in the direction this object has been thrown
    private void BeingThrownCode()
    {
        Vector3 velocity = thisRB.velocity;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, velocity, out hit, 1.2f, throwCollisionMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.GetComponent<ReusableHealth>() != null && velocity.magnitude >= 10) //Take and apply damage if hitting the enemy
            {
                int damage = Mathf.RoundToInt(velocity.magnitude / 5);
                hitObject.GetComponent<ReusableHealth>().ApplyDamage(Mathf.RoundToInt(velocity.magnitude / 8));
            }
        }
    }
}
