using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectBetweenTwoPoints : MonoBehaviour
{
    [SerializeField]
    private GameObject pointA, pointB;
    private Transform currentTarget;
    [SerializeField]
    private float movementSpeed = 10f, timeToWaitAtPoint = 0;
    [SerializeField]
    private bool lockYAxisMovement = false;
    private float timer = 0;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Destroy(this);
        }
        currentTarget = pointA.transform;
    }

    private void FixedUpdate()
    {
        //if the current target isn't null
        if (currentTarget != null)
        {
            //Move to the current target at the desired rate
            if (lockYAxisMovement == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z), movementSpeed * Time.fixedDeltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, movementSpeed * Time.fixedDeltaTime);
            }
            //If you have reached the target / got near it then switch the position
            if (Vector3.Distance(transform.position, currentTarget.transform.position) <= 0.5f)
            {
                if (currentTarget == pointA.transform)
                {
                    currentTarget = pointB.transform;
                }
                else
                {
                    currentTarget = pointA.transform;
                }
            }
        }
    }
}
