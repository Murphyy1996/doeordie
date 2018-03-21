/*
 * Author: Ross
 * Date: 10/9/17
 * Purpose: Adds functionality for AI enemies
 * Placement: Place on an enemy game object or prefab game object (navmesh and rigidbody will be automatically added)
 */

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

public class AI : MonoBehaviour
{
    //Visibles variables
    [SerializeField] [Range(0f, 50f)] [Header("How long AI waits at each waypoint")] private float waitTimer = 4f;
    [SerializeField] [Range(0f, 50f)] [Header("Time before AI resumes to search after lost sight")] private float lostSightTimer = 10f;
    [SerializeField] [Range(0f, 50f)] [Header("General delay when looking for the player")] private float generalDelay = 3f;
    [SerializeField] [Range(0f, 50f)] [Header("How close does the AI get to the waypoint before going to the next")] private float distanceToWaypoint = 1f;
    [SerializeField] [Range(0f, 50f)] [Header("How fast the AI turns when looking for the player")] private float turnSpeed = 2f;
    [SerializeField] [Header("How far can the enemy see")] private int maxSightDistance = 50;
    [SerializeField] [Header("The maximum cone of sight")] private int maxFieldOfView = 45;
    [SerializeField] [Header("how far the AI will explore when alert")] private float walkRadius;
    [SerializeField] private float distanceEnemyCanShoot = 50f, distanceBeforeEnemyMelee = 0.1f;
    [SerializeField] [Header("AI Search Path")] private List<GameObject> waypoints;
    [SerializeField] private GameObject bullet;

    //General variables
    private Vector3 looking, rayDirection, targetWaypoint;
    private Transform playerTransform;
    private NavMeshAgent agent;
    private SphereCollider sphereCollider;
    private int currentWaypoint;
    private float timer;
    private float timeBetweenShots = 2f, timeBetweenMelee = 3f;
    private bool hasSeenPlayer, isCharging = false;

    //Script References
    Shooting shooting;
    ReusableHealth reusableHealth;

    //FSM states
    [Header("Enemy is currently:")]
    [SerializeField]
    private States currentState;

    enum States
    {
        Searching,
        Alert,
        Attacking,
        End
    }

    void Start()
    {

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        shooting = playerTransform.GetComponent<Shooting>();

        reusableHealth = playerTransform.GetComponent<ReusableHealth>();

        agent = GetComponent<NavMeshAgent>();
        currentState = States.Searching;
        currentWaypoint = Random.Range(0, waypoints.Count);

        //walkRadius = (Vector3.Distance(transform.position, playerTransform.position));
    }

    void Update()
    {

        timer += Time.deltaTime;

        FieldOfView();

//        if (reusableHealth != null)
//        {
        if (playerTransform == null)
        {
//				if (reusableHealth.healthValue <= 0)
//				{
            agent.isStopped = false;
            ResetState();
            reusableHealth.CheckToSeeIfDead();
            //Delay();
//				}

//			}
         
        }


        //Add or remove states here...
        switch (currentState)
        {
            case States.Searching:
                StartCoroutine(Searching());
                break;
            case States.Alert:
                Alert();
                break;
            case States.Attacking:
                Attacking();
                break;
        }

    }

    //Pick random waypoint on the navmesh to go to
    IEnumerator Searching()
    {
        agent.isStopped = false;

        try
        {
            targetWaypoint = waypoints[currentWaypoint].transform.position;
        }
        catch
        {

        }

        if (targetWaypoint != null)
        {
            agent.speed = 5f;
            agent.acceleration = 8f;

            //Go to first waypoint
            agent.SetDestination(targetWaypoint);

            //When AI has reached first waypoint, pick a new one
            if (Vector3.Distance(transform.position, targetWaypoint) <= distanceToWaypoint)
            {

                currentWaypoint = Random.Range(0, waypoints.Count);

                try
                {
                    targetWaypoint = waypoints[currentWaypoint].transform.position;
                }
                catch
                {

                }

                agent.SetDestination(targetWaypoint);

                //Stop enemy at each waypoint
                StartCoroutine(StopAtWaypoint());

                yield break;
            }

        }


    }

    //AI has been alerted, search for player in a frantic manner
    void Alert()
    {
        NavMeshHit navHit;

        //Increase speed to match behavior
        agent.speed = 10f;
        agent.acceleration = 20f;

        //Generate random waypoint
        Vector3 randomDir = Random.insideUnitSphere * walkRadius;
        randomDir += transform.position;
        NavMesh.SamplePosition(randomDir, out navHit, walkRadius, -1);
        Vector3 finalPosition = navHit.position;
        agent.SetDestination(finalPosition);
    }

    //Behavior that's initiated when the player is seen
    void Attacking()
    {
        //Lock onto player
        agent.SetDestination(playerTransform.position);

        //AI is close to the player, initiate attack
        if (Vector3.Distance(transform.position, playerTransform.position) < distanceEnemyCanShoot && timer >= timeBetweenShots)
        {
            //Shoot();
        }
        else
        {
            return;
        }

        //Player is within melee range
        if (Vector3.Distance(transform.position, playerTransform.position) < distanceBeforeEnemyMelee && timer >= timeBetweenMelee)
        {

            if (reusableHealth != null)
            {
                //Stop in front of player briefly 
                agent.isStopped = true;

                Invoke("Charge", 1f);
            }

        }

    }

    //Simulates the players sight
    private void FieldOfView()
    {
        RaycastHit fovHit;

        try
        {
            //Track player location
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        catch
        {

        }


        if (playerTransform != null)
        {

            //Direction from current position to player
            rayDirection = playerTransform.position - transform.position;

            //Check angle between AI's forward vector and the direction vector between player and AI
            if (Vector3.Angle(rayDirection, transform.forward) < maxFieldOfView)
            {
                Debug.DrawLine(transform.position, playerTransform.position, Color.blue);

                if (Physics.Raycast(transform.position, rayDirection, out fovHit, maxSightDistance))
                {
                    //Check if any collider interrupts the ray from the enemy to the player
                    if (Physics.Linecast(transform.position, playerTransform.position, out fovHit))
                    {

                        if (fovHit.collider != null)
                        {

                            //The player is in the fov
                            if (fovHit.transform.tag == "Player")
                            {
                                //Follow and attack the player
                                currentState = States.Attacking;
                            }
                            else if (currentState == States.Attacking)
                            {
                                //Stop for a while then revert back to normal behavior
                                StartCoroutine(LookAround());
                            }


                        }


                    }
                    else
                    {
                        return;
                    }


                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }


        }


    }

    //Get a bullet from the object pool and fire the bullet
    void Shoot()
    {

        if (shooting != null)
        {
            bullet = shooting.ReturnBulletObjectPool()[0];

            bullet.GetComponent<Bullet>().bulletDamage = 50;
            bullet.GetComponent<Bullet>().playerBullet = false;

            shooting.ReturnBulletObjectPool().Remove(bullet);

            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            bullet.SetActive(true);

            reusableHealth.ApplyDamage(10);

            timer = 0f;
        }


    }

    //Use sphere collider to simulate noise as the player gets too close
    void OnTriggerEnter(Collider sphereCollider)
    {

        if (sphereCollider.tag == "Player")
        {
            currentState = States.Alert;
        }

    }

    //Player stepped out of alert/noise radius
    void OnTriggerExit(Collider sphereCollider)
    {

        if (sphereCollider.tag == "Player")
        {
            StartCoroutine(LostSightDelay());
        }

    }

    void Charge()
    {

        agent.speed = 5f;
        agent.acceleration = 8f;

        //Apply damage when player is hit
        reusableHealth.ApplyDamage(1000);

        //Reset state after player has been killed
        agent.isStopped = false;
        currentState = States.Searching;
        timer = 0f;
    }
        

    //Coroutine to give the AI some more natural behavior
    IEnumerator LookAround()
    {
        Vector3 tarPos = playerTransform.position - transform.position;
        Quaternion tarRot = Quaternion.LookRotation(tarPos);
        Quaternion lookRight = Quaternion.LookRotation(tarPos *= 5f);
        Quaternion lookLeft = Quaternion.LookRotation(tarPos *= -5f);

        //Pause and stop, then look left and right
        agent.isStopped = true;

        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, turnSpeed * Time.deltaTime);
        yield return new WaitForSeconds(0.5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRight, turnSpeed * Time.deltaTime);
        yield return new WaitForSeconds(0.5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookLeft, turnSpeed * Time.deltaTime);

        //Continue searching
        StartCoroutine(Delay());
    }

    //Time AI has to find the player before reset to search
    IEnumerator LostSightDelay()
    {
        yield return new WaitForSeconds(lostSightTimer);

        currentState = States.Searching;
    }

    //General delay
    IEnumerator Delay()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(generalDelay);
        agent.isStopped = false;
        currentState = States.Searching;
    }

    //Stop enemy at each waypoint
    IEnumerator StopAtWaypoint()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(waitTimer);
        agent.isStopped = false;
    }

    //Show field of view in the scene
    void OnDrawGizmos()
    {

        if (playerTransform == null)
        {
            return;
        }

        try
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        catch
        {

        }

        //Keep track of player for use with sound behaviour i.e player is close and behind = attack state
        Debug.DrawLine(transform.position, playerTransform.position, Color.blue);

        Vector3 frontRayPoint = /*transform.position + (transform.forward * maxSightDistance)*/ transform.forward * maxSightDistance;

        //Visualise rough field of view
        Vector3 leftRayPoint = frontRayPoint;
        leftRayPoint.x += maxFieldOfView /** 0.5f*/;

        Vector3 rightRayPoint = frontRayPoint;
        rightRayPoint.x -= maxFieldOfView /** 0.5f*/;

        Debug.DrawLine(transform.position, frontRayPoint * maxSightDistance, Color.red);
        Debug.DrawLine(transform.position, leftRayPoint * maxSightDistance, Color.green);
        Debug.DrawLine(transform.position, rightRayPoint * maxSightDistance, Color.green);

    }

    //Reset back to the default state and continue moving
    public void ResetState()
    {
        agent.isStopped = false;
        agent.SetDestination(Vector3.zero);
        currentState = States.Searching;
    }


}
