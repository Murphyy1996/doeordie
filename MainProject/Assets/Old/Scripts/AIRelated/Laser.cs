using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    private enum movementTypes
    {
        vertical,
        horizontal
    }

    ;

    [SerializeField]
    [Header("Movement")]
    private movementTypes selectedMovementType;
    [SerializeField]
    [Range(0, 10)]
    private float movementRange;
    [SerializeField]
    [Range(0, 25)]
    private float movementSpeed = 5;
    private GameObject target1, target2;
    private Transform currentTarget;
    private Ray ray;
    private RaycastHit hit;
    private ReusableHealth healthScript;
    private LineRenderer lineRenderer;
    private GameObject positionParent;
    private bool canLasersLaser = false;
    [SerializeField]
    [Range(0, 5)]
    private float timeToDelay = 0f;
    private LayerMask objsToHit;

    private void Start()
    {
        //call coroutine for laster lasers
        StopAllCoroutines();
        StartCoroutine(changeValue());
        objsToHit = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("No Teleport"));
    }


    private void Awake()
    {
        //Get the line renderer
        if (GetComponent<LineRenderer>() != null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            lineRenderer.SetWidth(0.1f, 0.1f);
            lineRenderer.SetColors(Color.red, Color.red);
        }
        //Create the targets
        target1 = new GameObject();
        target2 = new GameObject();
        //Set the initial target positions
        target1.transform.position = transform.position;
        target2.transform.position = transform.position;
        target1.transform.rotation = transform.rotation;
        target2.transform.rotation = transform.rotation;
        //Name the targets
        target1.name = this.gameObject.name + "target";
        target2.name = this.gameObject.name + "target";
        //Get an existing position parent if not make one
        GameObject foundObj = GameObject.Find("LaserParent");
        if (foundObj == null)
        {
            positionParent = new GameObject();
            positionParent.name = "LaserParent";
            foundObj = positionParent;
        }
        target1.transform.SetParent(foundObj.transform);
        target2.transform.SetParent(foundObj.transform);

        //Decide where to place the targets
        switch (selectedMovementType)
        {
            case movementTypes.vertical:
                //Create the target movement points
                target1.transform.position = new Vector3(transform.position.x, transform.position.y + movementRange, transform.position.z);
                target2.transform.position = new Vector3(transform.position.x, transform.position.y - movementRange, transform.position.z);
                break;

            case movementTypes.horizontal:
                //Create the target movement points
                target1.transform.position = transform.position + (transform.right * movementRange);
                target2.transform.position = transform.position + (transform.right * -movementRange);
                break;
        }

        //Set the current target
        currentTarget = target1.transform;
    }

    private void FixedUpdate()
    {
        //Only begin movement when a target has been set
        if (currentTarget != null)
        {
            if (canLasersLaser == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, (movementSpeed * Time.deltaTime));

                //If this object reaches its target, set a new one
                if (transform.position == currentTarget.position)
                {
                    if (currentTarget.position == target1.transform.position)
                    {
                        currentTarget = target2.transform;
                    }
                    else
                    {
                        currentTarget = target1.transform;
                    }   //Move the current object towards the target
                }
            }


            //Raycast a laser
            LaserAttack();
        }
    }

    private void LaserAttack() //This contains the code for the laser raycast
    {
        ray.origin = transform.position;
        ray.direction = transform.forward;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, hit.point);
            }

            if (hit.collider.GetComponent<ReusableHealth>() != null)
            {
                if (healthScript == null)
                {
                    healthScript = hit.collider.gameObject.GetComponent<ReusableHealth>();
                }
                healthScript.healthValue = -10000;
                healthScript.CheckToSeeIfDead();
            }
        }
    }

    IEnumerator changeValue()
    {

        yield return new WaitForSeconds(timeToDelay);

        canLasersLaser = true;
    }
}
