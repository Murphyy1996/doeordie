using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    private enum rotationAxis
    {
        X,
        Y,
        Z}
    ;

    [Header("Rotation Options")]
    [SerializeField]
    private rotationAxis selectedAxis;
    [SerializeField]
    [Range(0, 2000)]
    private int rotationSpeed = 20;
    private BoxCollider childCollider, thisBoxCollider;
    private GameObject childObject;
    [Header("Simulate physics on player (Y-axis only)")]
    [SerializeField]
    private bool simulatePlayerRotationPhysics = true;

    //Set up the child object and trigger collider for checking when the player is on this object
    private void Start()
    {
        if (simulatePlayerRotationPhysics == true && selectedAxis == rotationAxis.Y)
        {
            //Hard limit the rotation speed to 250 stop physics glitchs
            if (rotationSpeed > 250)
            {
                rotationSpeed = 250;
            }

            //Create rotation zones
            GameObject rotationZoneObj;

            if (GameObject.Find("RotationTriggerZones") == null)
            {
                rotationZoneObj = new GameObject();
                rotationZoneObj.name = "RotationTriggerZones";
            }
            else
            {
                rotationZoneObj = GameObject.Find("RotationTriggerZones");
            }
            //Create the rotation zone child for this object and position it correctly
            childObject = new GameObject();
            //Place the child trigger on the ignore raycast layer
            childObject.layer = 2;
            //Name, position, rotate and parent the child object to this object
            childObject.name = this.name + " Rotation Zone";
            //If this object has a parent set the childs parent as it so the correct size for the colliers can be got
            if (transform.parent != null)
            {
                childObject.transform.SetParent(transform.parent);
            }
            //Get the size of the trigger collider
            childObject.transform.localScale = transform.localScale;
            //Nullify any existing parents on the child as they are not needed anymore
            childObject.transform.SetParent(null);
            //Set the default transform and rotation of the child objet
            childObject.transform.position = transform.position;
            childObject.transform.rotation = transform.rotation;
            //Parent the child trigger object in an empty rotation zone object to stop the scene hierarchy clogging up
            childObject.transform.SetParent(rotationZoneObj.transform);
            //Create the collider on the child object, make it a trigger and make it slightly bigger than the parent
            childCollider = childObject.AddComponent<BoxCollider>();
            childCollider.isTrigger = true;
            //If there is no box collider on this object
            if (GetComponent<BoxCollider>() == null)
            {
                //Add a temp a box collider
                thisBoxCollider = this.gameObject.AddComponent<BoxCollider>();
                thisBoxCollider.isTrigger = true;
                //Position the collider
                childCollider.center = thisBoxCollider.center;
                //Size the collider
                childCollider.size = new Vector3(thisBoxCollider.size.x, thisBoxCollider.size.y * 10, thisBoxCollider.size.z);
                //Remove the temp box collider
                Destroy(thisBoxCollider);
            }
            else //Regular collider size code
            {
                childCollider.size = new Vector3(childCollider.size.x, childCollider.size.y * 100, childCollider.size.z);
            }
            //Give the child object the rotation collision script
            childObject.AddComponent<ObjRotationCollisions>();
            childObject.GetComponent<ObjRotationCollisions>().SetParentObj(this.gameObject);
            //Make the child object only detect the player and nothing else (Reduces collisions)
            childObject.layer = 11;
        }
    }

    private void FixedUpdate() //Rotate this object on the selected axis
    {
        if (childObject != null)
        {
            childObject.transform.position = transform.position;
            childObject.transform.rotation = transform.rotation;
        }
        switch (selectedAxis)
        {
            case rotationAxis.X:
                transform.Rotate(Vector3.right * (Time.deltaTime * rotationSpeed), Space.Self);
                break;

            case rotationAxis.Y:
                transform.Rotate(Vector3.up * (Time.deltaTime * rotationSpeed), Space.Self);
                break;

            case rotationAxis.Z:
                transform.Rotate(Vector3.forward * (Time.deltaTime * rotationSpeed), Space.Self);
                break;
        }
    }

    //When this object is destroyed destroy any possible left over trigger zone
    private void OnDestroy()
    {
        if (childObject != null)
        {
            Destroy(childObject);
        }
    }

    public GameObject ReturnTriggerZone() //Return the trigger zone for scripts that need it
    {
        if (simulatePlayerRotationPhysics == true && selectedAxis == rotationAxis.Y)
        {
            return childObject;
        }
        return null;
    }

    public int ReturnRotationSpeed()
    {
        return rotationSpeed;
    }
}
