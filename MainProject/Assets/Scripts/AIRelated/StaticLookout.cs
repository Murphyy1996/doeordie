using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLookout : MonoBehaviour
{

    [SerializeField] [Tooltip("For observing enemies only. Which points should the enemy look to?")] private Transform lookLeft, lookRight, lookForward;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How fast the AI turns when looking for the player")] private float turnSpeed = 2f;

    //FSM states
    [Header("Enemy is currently:")]
    [SerializeField]
    private States currentState;

    private Quaternion lookAtPoint;


    enum States
    {
        Observing,
        End
    }

    private LookDirections currentDirection = LookDirections.Right;

    enum LookDirections
    {
        Left,
        Forward,
        Right
    }

    void Start()
    {
        currentState = States.Observing;

        if (turnSpeed > 60)
        {
            turnSpeed = 60;
        }

       

    }

    void FixedUpdate()
    {


        //Add or remove states here...
        switch (currentState)
        {

            case States.Observing:
                switch (currentDirection)
                {
                    case LookDirections.Right:
                        StartCoroutine(LookoutRight());
                        break;

                    case LookDirections.Forward:
                        StartCoroutine(LookoutForward());
                        break;

                    case LookDirections.Left:
                        StartCoroutine(LookoutLeft());
                        break;
                }
            break;

        }

    
    }
		
    IEnumerator LookoutRight()
    {

        if (currentDirection == LookDirections.Right)
        {


            //agent.isStopped = true;
            Vector3 direction = lookRight.position - transform.position;
            direction.y = 0;

            float angle = Vector3.Angle(transform.forward, direction);

            if (angle > 0.1f)
            {
                lookAtPoint = Quaternion.LookRotation(direction);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, lookAtPoint, (Time.fixedDeltaTime * turnSpeed));

            }

            yield return new WaitForSeconds(6f);
            currentDirection = LookDirections.Forward;
        }
    }

    IEnumerator LookoutForward()
    {
        if (currentDirection == LookDirections.Forward)
        {


            //agent.isStopped = true;
            Vector3 direction = lookForward.position - transform.position;
            direction.y = 0;

            float angle = Vector3.Angle(transform.forward, direction);

            if (angle > 0.1f)
            {
                lookAtPoint = Quaternion.LookRotation(direction);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, lookAtPoint, (Time.fixedDeltaTime * turnSpeed));
            }

            yield return new WaitForSeconds(6f);
            currentDirection = LookDirections.Left;

        }

    }

    IEnumerator LookoutLeft()
    {
        if (currentDirection == LookDirections.Left)
        {


            //agent.isStopped = true;
            Vector3 direction = lookLeft.position - transform.position;
            direction.y = 0;

            float angle = Vector3.Angle(transform.forward, direction);

            if (angle > 0.1f)
            {
                lookAtPoint = Quaternion.LookRotation(direction);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, lookAtPoint, (Time.fixedDeltaTime * turnSpeed));
            }

            yield return new WaitForSeconds(6f);
            currentDirection = LookDirections.Right;

        }
    }


}
