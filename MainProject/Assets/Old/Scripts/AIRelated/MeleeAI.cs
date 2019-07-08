

/*
 * Author: Ross
 * Date: 15/10/17
 * Purpose: Adds functionality for melee only AI
 * Placement: Place on a melee AI
 * game object and fill out variables (hover over variables for description on what they should be)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]

public class MeleeAI : MonoBehaviour, IDocilable, IAlertable
{

    //Visible variables
    [TextArea] public string Note = "Hover over each variable to see a small description";
    [SerializeField] [Range(0f, 50f)] [Tooltip("How close does the AI get to the waypoint before going to the next")] private float distanceToWaypoint = 1f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How close the AI stops before initiating melee")] private float distanceBeforeEnemyMelee = 5f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("Time before AI resumes to search after lost sight")] private float lostSightTimer = 10f;
    [Range(0f, 50f)] [Tooltip("How fast should the enemy move in both speed/acceleration")] public float enemyWalkSpeed = 3f;
    [Range(0f, 50f)] [Tooltip("How fast should the enemy move in both speed/acceleration")] public float alertRunSpeed = 10f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How fast should the enemy turn once it's been alerted")] private float alertTurnSpeed = 7f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How fast the AI turns when looking for the player")] private float turnSpeed = 2f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How fast the AI accelerates when looking for the player")] private float accelerateSpeed = 60f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("General delay when looking for the player")] private float generalDelay = 3f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How long AI waits at each waypoint")] private float waitTimer = 4f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How far can the enemy see")] private int maxSightDistance = 50;
    [SerializeField] [Range(0f, 50f)] [Tooltip("The maximum cone of sight")] private int maxFieldOfView = 45;
    [SerializeField] [Header("AI Search Path")] private List<GameObject> waypoints;
    [SerializeField] [Tooltip("For observing enemies only. Which points should the enemy look to?")] private Transform lookLeft, lookRight, lookForward;
    [SerializeField] private LayerMask rayLayer;
    [SerializeField] private GameObject bloods;

    //General variables
    private Vector3 looking, rayDirection, targetWaypoint;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform playerTransform;
    private SphereCollider sphereCollider;
    [HideInInspector]
    public Animator meleeAnimator;
    private Quaternion lookAtPoint;
    private float timeBetweenMelee = 3f, playerShotNearby, meleeRayDistance = 1f;
    [HideInInspector]
	public float timer, maxShotDetection = 30f;
    private int currentWaypoint;
    [SerializeField]
    private GameObject wepToDrop; //an empty variable so that the designers can choose what weapon each ai drops
    private bool wepDropped = false;
    [SerializeField]
    private GameObject healthBox;
    [SerializeField]
    [Tooltip("The lowest possible number that can be generated")]
    private int lowerRandomRange; //gonna dd descriptions to these bottom three dw
    [SerializeField]
    [Tooltip("The Highest number that can be generated")]
    private int HigherRandomRange;
    [SerializeField]
    [Tooltip("Anything below this number will spawn health kits and anything above will spawn a weapon")]
    private int decidingPoint;

    //Script References
    ReusableHealth reusableHealth;
    ReusableHealth meleeHealth;


    //FSM states
    [Header("Enemy is currently:")]
    [SerializeField]
    private States currentState;

    enum States
    {
        Idle,
        Patrolling,
        Observing,
        Alerted,
        Attacking,
        Charge,
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
        meleeAnimator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        reusableHealth = playerTransform.GetComponent<ReusableHealth>();
        meleeHealth = GetComponent<ReusableHealth>();

        currentState = States.Patrolling;
        currentWaypoint = Random.Range(0, waypoints.Count);

        healthBox = GameObject.Find("HealthPickup");
    }

    //Method for spawning blood decals
    public void SpawnBlood()
    {
        if (bloods != null)
        {
            RaycastHit rayhit;
            if (Physics.Raycast(transform.position, -transform.up, out rayhit))
            {
                GameObject spawnedBlood = Instantiate(bloods, rayhit.point, Quaternion.EulerAngles(new Vector3(0, 0, 0))) as GameObject;
                spawnedBlood.name = "SpawnedBlood";
                spawnedBlood.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        FieldOfView();
        StartCoroutine(DetectPlayer());
        StartCoroutine(CheckForDeath());
    }

    void FixedUpdate()
    {
        //Add or remove states here...
        switch (currentState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Patrolling:
                StartCoroutine(Searching());
                break;
            case States.Alerted:
                Alert();
                break;
            case States.Attacking:
                Attack();
                break;
            case States.Charge:
                Charge();
                break;
        }

    }

    //Do nothing
    void Idle()
    {
        if (agent.isActiveAndEnabled == true && agent.isOnNavMesh == true)
        {
            agent.isStopped = true;
        }
        meleeAnimator.SetBool("isIdle", true);
        meleeAnimator.SetBool("isPatrolling", false);
    }

    //Pick random waypoint on the navmesh to go to
    public IEnumerator Searching()
    {
        meleeAnimator.SetBool("isPatrolling", true);

        try
        {
            targetWaypoint = waypoints[currentWaypoint].transform.position;
        }
        catch
        {

        }

        if (targetWaypoint != null)
        {
            agent.speed = enemyWalkSpeed;
            agent.acceleration = accelerateSpeed;

            if (agent.isActiveAndEnabled == true)
            {
                //Go to first waypoint
                agent.SetDestination(targetWaypoint);
            }

            //When AI has reached first waypoint, pick a new one
            if (Vector3.Distance(transform.position, targetWaypoint) <= distanceToWaypoint)
            {
                //Idle animation here

                currentWaypoint = Random.Range(0, waypoints.Count);

                try
                {
                    targetWaypoint = waypoints[currentWaypoint].transform.position;
                }
                catch
                {

                }

                if (agent.isActiveAndEnabled == true)
                {
                    //Move to waypoint
                    agent.SetDestination(targetWaypoint);
                }

                //Stop enemy at each waypoint
                StartCoroutine(StopAtWaypoint());

                yield break;
            }

        }


    }

    //Simulates the players sight
    public void FieldOfView()
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
                                agent.speed = 10f;
                            }
                            //                            else if (currentState == States.Attacking)
                            //                            {
                            //                                //Stop for a while then revert back to normal behavior
                            //                                StartCoroutine(LookAround());
                            //                            }


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

    //The player has shot or made a noise near the AI, turn around, run at player and then initiate melee attack
    public void Alert()
    {
        Vector3 tarPos = playerTransform.position - transform.position;
        Quaternion tarRot = Quaternion.LookRotation(tarPos);

        //Turn around quickly
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, Time.smoothDeltaTime * 7f); //Add variable for alert turn speed

        //Run to the player
        meleeAnimator.SetBool("isRunning", true);

        if (agent.isActiveAndEnabled == true)
        {
            //Lock onto player
            agent.SetDestination(playerTransform.position);
        }
    }

    //Behavior that's initiated when the player is seen
    public void Attack()
    {

        //Look at player
        Vector3 tarPos = playerTransform.position - transform.position;
        Quaternion tarRot = Quaternion.LookRotation(tarPos);

        //Turn around quickly
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, Time.smoothDeltaTime * 10f); //Add variable for alert turn speed

        //Lock onto player
        agent.SetDestination(playerTransform.position);

        //Initiate run animation
        meleeAnimator.SetBool("isRunning", true);

        //Set run speed
        agent.speed = alertRunSpeed;


        //Player is within melee range
        if (Vector3.Distance(transform.position, playerTransform.position) < distanceBeforeEnemyMelee && timer >= timeBetweenMelee)
        {

            //Direction from current position to player
            rayDirection = playerTransform.position - transform.position;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, rayDirection, out hit, meleeRayDistance, rayLayer))
            {

                if (reusableHealth != null)
                {
                    //Stop in front of player briefly 
                    agent.isStopped = true;

                    //Initiate charge animation
                    meleeAnimator.SetBool("isCharging", true);

                    Invoke("Charge", 1f);

                    playerTransform.GetComponent<ReusableHealth>().CalculateHitDirection(hit.point);
                }

            }

        }

    }

    //Initiate melee attack
    public void Charge()
    {
        //Apply damage when player is hit
        reusableHealth.ApplyDamage(5);

        if (agent.isActiveAndEnabled == true)
        {
            //Reset state after player has been killed
            agent.isStopped = false;
        }

        //Find all enemies
        GameObject[] goons;
        goons = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject goon in goons)
        {
            //If the found enemy is a melee enemy
            if (goon.GetComponent<MeleeAI>() != null)
            {
                MeleeAI meleeScript = goon.GetComponent<MeleeAI>();
                meleeScript.currentState = States.Patrolling;
                meleeScript.meleeAnimator.SetBool("isRunning", false);
                meleeScript.meleeAnimator.SetBool("isCharging", false);
                meleeScript.enemyWalkSpeed = 2.5f;
                meleeScript.timer = 0f;
            }
        }
    }

    //Coroutine to give the AI some more natural behavior when the player hides behind objects and sight is lost
    public IEnumerator LookAround()
    {
        Vector3 tarPos = playerTransform.position - transform.position;
        Quaternion tarRot = Quaternion.LookRotation(tarPos);
        Quaternion lookRight = Quaternion.LookRotation(tarPos *= 5f);
        Quaternion lookLeft = Quaternion.LookRotation(tarPos *= -5f);

        //Pause and stop, then look left and right
        agent.isStopped = true;

        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, Time.smoothDeltaTime * turnSpeed);
        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRight, Time.smoothDeltaTime * turnSpeed);
        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookLeft, Time.smoothDeltaTime * turnSpeed);
        yield return new WaitForSeconds(1f);

        //Continue searching
        StartCoroutine(Delay());
    }

    //Stop enemy at each waypoint
    public IEnumerator StopAtWaypoint()
    {
        meleeAnimator.SetBool("isPatrolling", false);
        currentState = States.Idle;
        if (agent.isActiveAndEnabled == true)
        {
            agent.isStopped = true;
        }
        yield return new WaitForSeconds(waitTimer);
        if (agent.isActiveAndEnabled == true)
        {
            agent.isStopped = false;
        }
        meleeAnimator.SetBool("isPatrolling", false);
        currentState = States.Patrolling;
    }

    //Detect when the player has fired within a certain range
    IEnumerator DetectPlayer()
    {

        if (playerTransform.GetComponent<Shooting>().IsPlayerShooting())
        {
            playerShotNearby = Vector3.Distance(playerTransform.position, transform.position);

            if (playerShotNearby <= maxShotDetection)
            {
                enemyWalkSpeed = 10f;
                yield return new WaitForSeconds(0.5f);
                currentState = States.Alerted;
            }

        }

        yield break;
    }

    //Manages death 
    IEnumerator CheckForDeath()
    {
        if (meleeHealth.healthValue <= 0f)
        {
            if (wepDropped == false)
            {
                wepDropped = true;
                Invoke("DropWeapon", 0.5f);
                // wepDropped = false;
            }

            // Debug.Log("DROPPING THE LOAD");
            currentState = States.Idle;
            meleeAnimator.SetBool("isDead", true);
            yield return new WaitForSeconds(0.7f);
            meleeAnimator.SetBool("isDead", false);
            meleeAnimator.enabled = false;
            agent.enabled = false;
            //make the enemy drop a gun 


        }
        else
        {
            yield return null;
        }

    }

    //General delay
    public IEnumerator Delay()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(generalDelay);
        agent.isStopped = false;

        currentState = States.Patrolling;
    }

    //Time AI has to find the player before reset to search
    public IEnumerator LostSightDelay()
    {
        yield return new WaitForSeconds(lostSightTimer);

        currentState = States.Patrolling;
    }

    //Use sphere collider to simulate noise as the player gets too close
    public void OnTriggerEnter(Collider sphereCollider)
    {

        if (sphereCollider.tag == "Player")
        {
            currentState = States.Observing;
        }

    }

    //Player stepped out of alert/noise radius
    public void OnTriggerExit(Collider sphereCollider)
    {

        if (sphereCollider.tag == "Player")
        {
            StartCoroutine(LostSightDelay());
        }

    }

    //Show field of view in the scene
    public void OnDrawGizmos()
    {

        //        if (playerTransform == null)
        //        {
        //            return;
        //        }
        //
        //        try
        //        {
        //            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //        }
        //        catch
        //        {
        //
        //        }
        //
        //        //Keep track of player for use with sound behaviour i.e player is close and behind = attack state
        //        Debug.DrawLine(transform.position, playerTransform.position, Color.blue);
        //
        //        Vector3 frontRayPoint = /*transform.position + (transform.forward * maxSightDistance)*/ transform.forward * maxSightDistance;
        //
        //        //Visualise rough field of view
        //        Vector3 leftRayPoint = frontRayPoint;
        //        leftRayPoint.x += maxFieldOfView;
        //
        //        Vector3 rightRayPoint = frontRayPoint;
        //        rightRayPoint.x -= maxFieldOfView;
        //
        //        Debug.DrawLine(transform.position, frontRayPoint * maxSightDistance, Color.red);
        //        Debug.DrawLine(transform.position, leftRayPoint * maxSightDistance, Color.green);
        //        Debug.DrawLine(transform.position, rightRayPoint * maxSightDistance, Color.green);

    }

    //Spawn dropped weapon
    private void DropWeapon() //Author: Kate Georgiou - handles the random n umber generation so it knows what to spawn on each enemy who dies
    {
        int randoNumber = Random.Range(lowerRandomRange, HigherRandomRange);
        GameObject spawnedObject;
        if (randoNumber <= decidingPoint && randoNumber >= lowerRandomRange) //if the random number ranges between 0 and 25 then drop the health kits...
        {
            spawnedObject = Instantiate(healthBox, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity) as GameObject;
            spawnedObject.name = "Dropped Item"; //name the object that is dropped this name
        }
        else if (randoNumber >= decidingPoint && randoNumber <= HigherRandomRange) //if the random number is more than 25 but less than 100 spawn a weapon...
        {
            spawnedObject = Instantiate(wepToDrop, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity) as GameObject;
            spawnedObject.name = "Dropped Item"; //name the object that is dropped this name

        }



    }

}
