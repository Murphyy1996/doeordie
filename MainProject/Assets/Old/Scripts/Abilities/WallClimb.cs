/*
 * Author: Ross
 * Date: 04/10/17
 * Purpose: To enable player to scale walls
 * Placement: Place on player
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimb : MonoBehaviour
{

    //Visible Variables
    [SerializeField] private KeyCode wallClimbKey;
    [SerializeField] private bool canClimb = true;
	[SerializeField] [Range(0f, 50f)] private float climbSpeed = 5f, rayDistance = 50f, maxDistanceToWall = 1.5f, timeBetweenClimb = 3f, wallRunSpeed = 0.2f, rotationSpeed; 
	[SerializeField] [Range(0f, 50f)] [Tooltip("The maximum distance player can climb before it's pulled down")] private float maxClimbDistance = 0.5f;
	[SerializeField] private LayerMask rayLayer;


    //General Variables
    private Rigidbody rb;
    private CharacterController cc;
    private float distanceToWall, climbTimer, currentGravity;
    private bool climbing = false, slerpMe = false;
    private Quaternion defaultRot, leftRot, rightRot, currentRot, targetRot;
    private GameObject CameraHolder; 

    //Script References
    CharacterControllerMovement characterControllerMovement;
    Grapple grappleScript;

    //Delay start as rigidbody is not always spawned in
    private void Start()
    {
        Invoke("DelayedStart", 0.1f);
    }

    private void DelayedStart()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        characterControllerMovement = GetComponent<CharacterControllerMovement>();
        grappleScript = GetComponent<Grapple>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        climbTimer = 3f;
        defaultRot = this.gameObject.transform.rotation;
        targetRot = defaultRot;
        rightRot = new Quaternion(0, 0, transform.rotation.z + 0.1f, transform.rotation.w);
        leftRot = new Quaternion(0, 0, transform.rotation.z - 0.1f, transform.rotation.w);

    }

    void Update()
    {

        if (rb != null)
        {

            climbTimer += Time.deltaTime;

            FailSafes();

            DistanceCheck();

            LeftDistanceCheck();

            RightDistanceCheck();

            CheckInput();
		
        }

        
    }

    private void FixedUpdate()
    {
        targetRot = defaultRot;
        Quaternion direction = targetRot;
        if (climbing == true) //if in wall climb, raycast left and right in order to detect which direction the player is climbing the wall from so that it can tilt in the opposite direction
        {
            if (Physics.Raycast(transform.position, transform.right, 1 ,  rayLayer)) 
            {
                targetRot = leftRot;
                direction = targetRot;
                direction.x = transform.rotation.x;

                //tilt it left
                //  this.gameObject.transform.rotation
            }
            else if (Physics.Raycast(transform.position, -transform.right, 1, rayLayer))
            {
                targetRot = rightRot;
                direction = targetRot;
                direction.x = transform.rotation.x;
                //tilt right
            }
            //agent.isStopped = true;
            direction.y = transform.rotation.y;
        }

        if (climbing == false)
        {
            direction.x = 0;
            direction.y = transform.rotation.y;
            direction.z = 0;
            direction.w = transform.rotation.w;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, direction, Time.fixedDeltaTime * rotationSpeed);

    }
    //Check for players input
    void CheckInput()
    {
        
//        if (canClimb)
//        {
        if (climbTimer >= timeBetweenClimb && Input.GetKeyDown(wallClimbKey))
        {
            canClimb = true;

            DistanceCheck();

            LeftDistanceCheck();

            RightDistanceCheck();
        }
        else
        {
            return;

        }

//        }
            
    }
  
    public void StopClimbCoroutines()
    {
        StopAllCoroutines();
    }

    //Move the player up the wall
    IEnumerator Climb()
    {

        while (Input.GetKey(wallClimbKey) && canClimb)
        {
            //Scale the wall
            characterControllerMovement.TriggerForcedJump(climbSpeed);
            climbing = true;
            //Decrease climb speed to simulate wall run
            yield return new WaitForSeconds(maxClimbDistance);
            climbSpeed -= wallRunSpeed;
            StartCoroutine(Climb());
		
            yield break;

        }

        //When not climbing
        climbSpeed = 5f;
        canClimb = false;
        climbing = false;
    }
        

    //Works out the distance so the player can climb
    void DistanceCheck()
    {

        Vector3 rayPosition = new Vector3(transform.position.x, transform.position.y +0.5f, transform.position.z);

        Ray rayCenter = new Ray(rayPosition, transform.forward);

        RaycastHit hit;


        if (Physics.Raycast(rayCenter, out hit, rayDistance, rayLayer))
        {
            
            if (hit.collider.tag == "Wall")
            {
                distanceToWall = Vector3.Distance(rayPosition, hit.point);

                if (distanceToWall < maxDistanceToWall)
                {
                    StartCoroutine(Climb());
                }
                else
                {
                    return;
                    StopCoroutine(Climb());
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
        
    //Left raycast for parallel wallclimb
    void LeftDistanceCheck()
    {
        
        float fovAngle = 60f;

        Vector3 rayPosition = new Vector3(transform.position.x, transform.position.y +0.5f, transform.position.z);
        Vector3 leftRay = Quaternion.AngleAxis(-fovAngle, transform.up) * transform.forward;

        Ray rayLeft = new Ray(rayPosition, leftRay);

        RaycastHit hit;


        if (Physics.Raycast(rayLeft, out hit, rayDistance, rayLayer))
        {

            if (hit.collider.tag == "Wall")
            {
                distanceToWall = Vector3.Distance(rayPosition, hit.point);

                if (distanceToWall < maxDistanceToWall)
                {
                    StartCoroutine(Climb());
                }
                else
                {
                    return;
                    StopCoroutine(Climb());
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

    //Right raycast for parallel wallclimb
    void RightDistanceCheck()
    {

        float fovAngle = 60f;

        Vector3 rayPosition = new Vector3(transform.position.x, transform.position.y +0.5f, transform.position.z);
        Vector3 rightRay = Quaternion.AngleAxis(fovAngle, transform.up) * transform.forward; 

        Ray rayRight = new Ray(rayPosition, rightRay);

        RaycastHit hit;
    

        if (Physics.Raycast(rayRight, out hit, rayDistance, rayLayer))
        {

            if (hit.collider.tag == "Wall")
            {
                distanceToWall = Vector3.Distance(rayPosition, hit.point);

                if (distanceToWall < maxDistanceToWall)
                {
                    StartCoroutine(Climb());
                }
                else
                {
                    return;
                    StopCoroutine(Climb());
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

    //Enable to player to detach from the wall
    void FailSafes()
    {
        
        //Prevents infinite no gravity
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, rayDistance) /*|| distanceToWall > maxDistanceToWall*/)
        {
            //canClimb = false;
            characterControllerMovement.GravityEnabled(true);
            StopCoroutine(Climb());
            return;
        }
            
        //Fall if crouch is pressed
        if (distanceToWall > maxDistanceToWall /*&& Input.GetKeyDown(crouchKey)*/)
        {
            characterControllerMovement.GravityEnabled(true);
            return;
        }

        if (Input.GetKeyUp(wallClimbKey))
        {
            //            characterControllerMovement.GravityVelocityReset();
            Debug.Log("AHHHHHHHHHH");
            climbing = false;
        }

        //Disable wall climb while grappling
        if (grappleScript.IsCurrentlyGrappling())
        {
            canClimb = false;
        }

        //Reset cooldown
        if (canClimb)
        {
            climbTimer = 0f;
        }
           
        //Reset if grounded
        if (characterControllerMovement.AccurateIsControllerGrounded())
        {
            climbTimer = 3f;
        }

//        if (!characterControllerMovement.AccurateIsControllerGrounded())
//        {
//            climbTimer = 0f;
//        }


    }

    public void SetCanClimbValue(bool value)
    {
        canClimb = value;
    }

    public bool GetCanClimbValue()
    {
        return canClimb;
    }
    public bool Climbing()
    {
        return climbing;
    }
}
