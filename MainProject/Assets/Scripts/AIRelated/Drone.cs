//Author: James Murphy
// Purpose: Bring life into these drones
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private enum states
    {
        attack,
        patrol,
        idle
    }

    private enum patrolDirection
    {
        vertical,
        horizontal
    }

    [Header("AI Settings")]
    [SerializeField]
    private states currentAIState = states.attack;
    [SerializeField]
    [Range(0, 200f)]
    private float playerDetectionSensitivity = 40;
    [SerializeField]
    [Range(0, 60)]
    private float outOfRangeGiveUpTimer = 30f;
    [SerializeField]
    [Range(0, 4f)]
    private float rangeToCallOtherDronesIntoBattle = 2f;
    [SerializeField]
    private LayerMask playerDetectionRaymask;
    [Header("Patrol Settings")]
    [SerializeField]
    [Range(0, 200f)]
    private float patrolRange = 10;
    [SerializeField]
    private patrolDirection selectedPatrolDirection = patrolDirection.horizontal;
    [Header("Movement Settings")]
    [SerializeField]
    [Range(0, 100)]
    private float movementSpeed = 10f;
    [SerializeField]
    [Range(0, 20f)]
    private float turningSpeed = 10f;
    private Transform player;
    private ReusableHealth playerHealth, thisHealth;
    private float distanceFromPlayer;
    private Vector3 startingPosition;
    private GameObject patrolPoint1, patrolPoint2, patrolPointParent;
    private Transform targetPatrolPoint;
    private states startingState;
    [Header("Collision Settings")]
    [SerializeField]
    private LayerMask collisionCheckLayerMask;
    [SerializeField]
    private LayerMask stunnedCollisionLayerMask;
    [SerializeField]
    [Range(0, 5)]
    private float stunTime = 1f;
    [SerializeField]
    [Range(0, 1000)]
    private float droneBounceValue = 450;
    private bool isStunned = false;
    private Rigidbody thisRB;
    private GameObject rayStartObj;
    private ObjectRotation[] objRotationScripts;
    private List<GameObject> foundDrones = new List<GameObject>();
    private float stateTimer = 0;
    [HideInInspector]
    public bool canBeTriggeredByOtherDrones = true;
    [SerializeField] private GameObject explosObj;


    // Use this for initialization
    private void Start()
    {
        rayStartObj = new GameObject();
        rayStartObj.name = "Ray start obj";
        rayStartObj.transform.SetPositionAndRotation(transform.position, transform.rotation);
        rayStartObj.transform.SetParent(transform);
        thisRB = GetComponent<Rigidbody>();
        startingState = currentAIState;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<ReusableHealth>();
        thisHealth = GetComponent<ReusableHealth>();
        //Get the starting position
        startingPosition = transform.position;
        //Get all object rotation scripts
        objRotationScripts = GetComponentsInChildren<ObjectRotation>();
        //Get all the found drones
        GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("enemy");
        //Add all drones to the found drone list
        foreach (GameObject foundEnemy in foundEnemies)
        {
            if (foundEnemy.GetComponent<Drone>() != null)
            {
                foundDrones.Add(foundEnemy);
            }
        }

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Only allow movement if player is not null
        if (player != null)
        {
            //Work out the distance from the player
            distanceFromPlayer = Vector3.Distance(transform.position, player.position);
            //Make the raystart object always look at the player
            rayStartObj.transform.LookAt(player.transform.position);
            //Select what state to enter
            switch (currentAIState)
            {
                case states.idle:
                    IdleCode();
                    break;
                case states.attack:
                    AttackCode();
                    break;
                case states.patrol:
                    PatrolCode();
                    break;
            }
        }
        if (isStunned == true)
        {
            transform.Rotate(2, 2, 2);
            //Check if the object has hit anything
            StunnedDroneCollisions();
        }
    }

    //This code will be used to detect the player if they are within a certain radius
    private void IdleCode()
    {
        if (distanceFromPlayer <= playerDetectionSensitivity)
        {
            RaycastHit rayhit;
            if (Physics.Raycast(rayStartObj.transform.position, rayStartObj.transform.forward, out rayhit, playerDetectionSensitivity, playerDetectionRaymask))
            {
                if (rayhit.collider.tag == "Player")
                {
                    //Trigger the attack state
                    currentAIState = states.attack;
                }
            }
        }
    }

    //Attack code will be run here
    private void AttackCode()
    {
        try
        {
            //if the player has evaded this drone for a certain amount of time, then give up and go back to patrolling
            if (distanceFromPlayer >= playerDetectionSensitivity)
            {
                stateTimer = stateTimer + Time.fixedDeltaTime;
            }
            else //If the player has been found again reset the timer
            {
                stateTimer = 0;
            }

            //Only chase the player if this object isn't stunned
            if (isStunned == false)
            {
                //Tell nearby drones to attack
                foreach (GameObject drone in foundDrones)
                {
                    if (drone != null && drone != this.gameObject)
                    {
                        Drone droneScript = drone.GetComponent<Drone>();
                        if (Vector3.Distance(transform.position, drone.transform.position) <= rangeToCallOtherDronesIntoBattle && droneScript.canBeTriggeredByOtherDrones == true)
                        {
                            droneScript.TriggerAttackState();
                        }
                    }
                }
                //Rest of the attack code
                thisRB.drag = 2f;
                thisRB.angularDrag = 2f;
                //Look at the player
                Quaternion droneRotation = Quaternion.LookRotation(player.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, droneRotation, (Time.fixedDeltaTime * turningSpeed));
                //Run the collision code
                RegularDroneCollisions();
                //Always drive forwards
                transform.position += transform.forward * (Time.fixedDeltaTime * movementSpeed);
            }

            //If the player has left the drone area then set the state to patrol
            if (stateTimer > outOfRangeGiveUpTimer)
            {
                //Allow the drone to change states without other drones affecting it
                StartCoroutine(AttackStateCooldown());
                //Change to the patrol state
                currentAIState = states.patrol;
            }
        }
        catch
        {
            print("error in drone attack");
        }
    }

    //Patrol code will be run here
    private void PatrolCode()
    {
        //If the player is near, switch to attack code
        if (distanceFromPlayer <= playerDetectionSensitivity)
        {
            RaycastHit rayhit;
            if (Physics.Raycast(rayStartObj.transform.position, rayStartObj.transform.forward, out rayhit, playerDetectionSensitivity, playerDetectionRaymask))
            {
                if (rayhit.collider.tag == "Player")
                {
                    //Trigger the attack state
                    currentAIState = states.attack;
                }
            }
        }

        //Set up placement of the patrol zones
        if (patrolPoint1 == null || patrolPoint2 == null)
        {
            switch (selectedPatrolDirection)
            {
                case patrolDirection.vertical:
                    patrolPoint1 = new GameObject();
                    patrolPoint1.name = this.gameObject.name + " Patrol point 1";
                    patrolPoint1.transform.position = transform.position + (transform.up * patrolRange);
                    patrolPoint2 = new GameObject();
                    patrolPoint2.name = this.gameObject.name + " Patrol point 2";
                    patrolPoint2.transform.position = transform.position + (transform.up * -patrolRange);
                    break;
                case patrolDirection.horizontal:
                    patrolPoint1 = new GameObject();
                    patrolPoint1.name = this.gameObject.name + " Patrol point 1";
                    patrolPoint1.transform.position = transform.position + (transform.forward * patrolRange);
                    patrolPoint2 = new GameObject();
                    patrolPoint2.name = this.gameObject.name + " Patrol point 2";
                    patrolPoint2.transform.position = transform.position + (transform.forward * -patrolRange);
                    break;
            }

            //Create a patrol point parent for both of these patrol points
            patrolPointParent = new GameObject();
            patrolPointParent.name = this.gameObject.name + "Patrol points";
            patrolPoint1.transform.SetParent(patrolPointParent.transform);
            patrolPoint2.transform.SetParent(patrolPointParent.transform);
            //Create a global patrol point parent to clean up the scene
            GameObject foundObj = GameObject.Find("GlobalPatrolParent");
            if (foundObj == null)
            {
                GameObject globalPatrolPointParent = new GameObject();
                globalPatrolPointParent.name = "GlobalPatrolParent";
                patrolPointParent.transform.SetParent(globalPatrolPointParent.transform);
            }
            else
            {
                patrolPointParent.transform.SetParent(foundObj.transform);
            }
        }

        //if there is no selected target point, patrol at the first point
        if (targetPatrolPoint == null)
        {
            targetPatrolPoint = patrolPoint1.transform;
        }

        //Work out the distance to the target point
        float distanceFromPoint = Vector3.Distance(transform.position, targetPatrolPoint.position);

        //Look at the target point
        Quaternion droneRotation = Quaternion.LookRotation(targetPatrolPoint.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, droneRotation, (Time.fixedDeltaTime * turningSpeed));
        if (isStunned == false)
        {
            //Always drive forwards towards the point
            transform.position += transform.forward * (Time.fixedDeltaTime * (movementSpeed / 4));
        }

        //If the drone reaches the point, turn around and go to the other point
        if (distanceFromPoint < 0.5f)
        {
            if (targetPatrolPoint == patrolPoint1.transform)
            {
                targetPatrolPoint = patrolPoint2.transform;
            }
            else
            {
                targetPatrolPoint = patrolPoint1.transform;
            }
        }
    }

    //Destroy the patrol points
    private void OnDestroy()
    {
        explosObj.transform.SetParent(null);
        explosObj.SetActive(true);
        explosObj.AddComponent<DestroyAfterTime>();
        Destroy(thisRB);
        playerDetectionSensitivity = 0;
        currentAIState = states.idle;
        StopCoroutine(ActivateStun(stunTime));
        Destroy(patrolPointParent);

    }

    //Detect collisions
    private void RegularDroneCollisions()
    {
        if (isStunned == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, collisionCheckLayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.tag != "enemy")
                {
                    if (isStunned == false)
                    {
                        //Activate the stune
                        StartCoroutine(ActivateStun(stunTime));
                        //Add the bounce force
                        thisRB.AddForce(-transform.forward * droneBounceValue);
                    }
                }
            }
        }
    }

    //Detect collisions whilst stunned
    private void StunnedDroneCollisions()
    {
        Vector3 velocity = thisRB.velocity;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, velocity, out hit, 1f, stunnedCollisionLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.tag == "enemy" && velocity.magnitude >= 10) //Take and apply damage if hitting the enemy
            {
                int damage = Mathf.RoundToInt(velocity.magnitude / 5);
                hitObject.GetComponent<ReusableHealth>().ApplyDamage(Mathf.RoundToInt(velocity.magnitude / 8));
                thisHealth.ApplyDamage(damage);
            }
            else if (velocity.magnitude >= 10) //Take damage if hitting the environment
            {
                int damage = Mathf.RoundToInt(velocity.magnitude / 5);
                thisHealth.ApplyDamage(damage);
            }
        }
    }

    //Will stun this object for the required amount of time
    public IEnumerator ActivateStun(float suppliedStunTime)
    {
        foreach (ObjectRotation script in objRotationScripts)
        {
            script.enabled = false;
        }
        thisRB.useGravity = true;
        thisRB.drag = 0.2f;
        thisRB.angularDrag = 0f;
        isStunned = true;
        yield return new WaitForSeconds(suppliedStunTime);
        isStunned = false;
        thisRB.Sleep();
        thisRB.WakeUp();
        thisRB.useGravity = false;
        foreach (ObjectRotation script in objRotationScripts)
        {
            script.enabled = true;
        }
    }

    //Should trigger the attack state
    public void TriggerAttackState()
    {
        //Set this state to attack
        currentAIState = states.attack;
    }

    //Return whether in the attack state
    public bool CheckIfAttacking()
    {
        if (currentAIState == states.attack)
        {
            return true;
        }
        return false;
    }

    //will be called if the player dies and will set this ai back to the starting position for example
    public void ResetAI()
    {
        thisRB.Sleep();
        transform.position = startingPosition;
        currentAIState = startingState;
        thisRB.WakeUp();
    }

    public bool ReturnIfStunned() //Return if the drone is stunned
    {
        return isStunned;
    }

    private IEnumerator AttackStateCooldown() //Allow the drone to change state without being affected by other drones
    {
        canBeTriggeredByOtherDrones = false;
        yield return new WaitForSeconds(2);
        canBeTriggeredByOtherDrones = true;
    }
}
