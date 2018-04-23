//Author: James Murphy
//Purpose: To help the player climb ledge
//Requirements: Place on the player

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeClimbV2 : MonoBehaviour
{
    [SerializeField]
    [Range(0, 10)]
    private float ledgeClimbSpeed = 5f;
    [SerializeField]
    [Range(0, 2f)]
    private float ledgeCollisionZoneYOffset = 1.1f;
    private float reducedX, reducedY;
    [Header("Ledge auto generation settings")]
    [SerializeField]
    private bool autoGenerationEnabled = false;
    [SerializeField]
    private string objLedgeTagToUse = "Wall";
    [SerializeField]
    private bool shouldCheckForClimableLedge = false, inLedgeZone = false, scriptSetUp = false;
    private CharacterControllerMovement movement;
    //private WallClimbV2 wallClimbScript;
    private CharacterController thisCC;
    private Grapple grapple;
    private RaycastHit forwardCheck, downwardsLedgeCheck, upwardsCheck;
    private LayerMask thisLayerMask;
    private GameObject downwardsRayStartEmpty, collisionCheckObj, ledgeClimbParent;
    private CheckIfCollidingWithObject collisionCheckScript;
    private FirstPersonCamera firstPerson;
    private GameObject[] foundWalls;
    [Header("This is a temporary option until the infinite wall climb bug has been fixed")]
    [SerializeField]
    private bool overrideWallClimb = false;
    [SerializeField]
    private bool runOnce = true, isLedgeClimbing = false;
    private float timeInClimb = 0, maxTimeInClimb = 2;

    private void Awake() //Delay the component code in order to find components without error
    {
        Time.timeScale = 1;
        Invoke("DelayedAwake", 0.2f);
    }

    public void ResetToDefault() //Will reset the ledge climb to defaults to hopefully avoid issues
    {
        scriptSetUp = false;
        runOnce = false;
        shouldCheckForClimableLedge = false;
        inLedgeZone = false;
        timeInClimb = 0;
        isLedgeClimbing = false;
        if (downwardsRayStartEmpty != null)
        {
            Destroy(downwardsRayStartEmpty);
        }
        if (collisionCheckObj != null)
        {
            Destroy(collisionCheckObj);
        }
        scriptSetUp = true;
    }

    private void DelayedAwake() //Get all required components
    {
        firstPerson = GetComponentInChildren<FirstPersonCamera>();
        reducedX = firstPerson.GetCurrentXSensitivity() / 2;
        reducedY = firstPerson.GetCurrentYSensitivity() / 2;

        thisCC = GetComponent<CharacterController>();
        movement = GetComponent<CharacterControllerMovement>();
        grapple = GetComponent<Grapple>();
        if (movement != null)
        {
            thisLayerMask = movement.ReturnRoofLayerMask();
        }
        if (autoGenerationEnabled == true)
        {
            GenerateLedges();
        }
        scriptSetUp = true;
    }

    private void GenerateLedges() //This contains the code for generating the ledges
    {
        if (foundWalls == null)
        {
            //Find all ledge climbable walls in the scene
            foundWalls = GameObject.FindGameObjectsWithTag(objLedgeTagToUse);
            //Add a trigger collider onto the climbable walls
            foreach (GameObject wall in foundWalls)
            {
                if (wall.GetComponent<pb_Object>() == null && wall.GetComponent<DontCreateLedge>() != null)
                {
                    if (wall.GetComponent<Renderer>() != null)
                    {
                        //Create an empty with the same size collider as the parent
                        GameObject duplicate = new GameObject();
                        Renderer wallRenderer = wall.GetComponent<Renderer>();
                        duplicate.transform.localScale = new Vector3(wallRenderer.bounds.size.x, wallRenderer.bounds.size.y, wallRenderer.bounds.size.z);
                        duplicate.transform.SetPositionAndRotation(wall.transform.position, wall.transform.rotation);
                        duplicate.name = "LedgeClimbZone: " + wall.name;
                        //Create the ledge climb parent for any ledges that are generated
                        if (ledgeClimbParent == null)
                        {
                            ledgeClimbParent = new GameObject();
                            ledgeClimbParent.name = "Automatically generated ledges";
                        }
                        //Place it on the layer that wont cause issues and remove any tags
                        duplicate.tag = "Ledge";
                        duplicate.layer = 23;
                        //Add the trigger zone for the ledge
                        BoxCollider ledgeTrigger = duplicate.AddComponent<BoxCollider>();
                        ledgeTrigger.isTrigger = true;
                        //Size the ledge correctly
                        ledgeTrigger.size = new Vector3(ledgeTrigger.size.x * 6, ledgeTrigger.size.y / 8, ledgeTrigger.size.z * 6);
                        float wallheight = wall.GetComponent<Renderer>().bounds.size.y;
                        wallheight = wallheight / 2.3f;
                        //Position the ledge correctly
                        duplicate.transform.position = new Vector3(duplicate.transform.position.x, duplicate.transform.position.y + wallheight, duplicate.transform.position.z);
                        //Put this ledge in a ledge parent to clear up the scene
                        duplicate.transform.SetParent(null);
                        duplicate.transform.SetParent(ledgeClimbParent.transform);
                        //Add the ledge climb collision script
                        duplicate.AddComponent<LedgeCollisionDetection>().player = this.gameObject;

                    }
                    else
                    {
                        print("Cannot generate ledge for object as it does not have a renderer.");
                    }
                }
            }
        }
    }

    private void Update() //This code is used for checking button keypresses
    {
        //If space is being held and the conditions are passed, check to see if the player is standing on 
        if (Input.GetKey(KeyCode.Space) && grapple.IsCurrentlyGrappling() == false && firstPerson.isShaking == false)
        {
            shouldCheckForClimableLedge = true;
        }
        else
        {
            shouldCheckForClimableLedge = false;
        }
    }

    private void FixedUpdate() //All the ledge climb code is run at the same rate as that physics engine as it relies on collisions
    {
        //Only run this code if the script has set up
        if (scriptSetUp == true)
        {
            //Exit this if grappling
            if (grapple.IsCurrentlyGrappling() == true)
            {
                if (collisionCheckObj != null)
                {
                    Destroy(collisionCheckObj);
                }
                collisionCheckScript = null;
            }

            if (isLedgeClimbing == true)
            {
                timeInClimb = timeInClimb + Time.fixedDeltaTime;
                if (timeInClimb > maxTimeInClimb)
                {
                    CancelLedgeClimb();
                }
            }
            else
            {
                timeInClimb = 0;
            }

            if (collisionCheckObj != null)
            {
                if (collisionCheckScript != null && collisionCheckScript.CheckIfInObj() == false)
                {
                    PerformLedgeClimb();
                }
                else //Perform a reset
                {
                    if (runOnce == false)
                    {
                        movement.GravityVelocityReset();
                        firstPerson.ResetCameraSensitivity();
                        runOnce = true;
                    }
                    Destroy(collisionCheckObj);
                    collisionCheckScript = null;
                }
            }
            //Only run this code when near a ledge
            else if (inLedgeZone == true)
            {
                if (shouldCheckForClimableLedge == true && movement.ReturnObjectPlayerIsStandingOnAccurate() == null && collisionCheckObj == null)
                {
                    //Check if there is a wall infront of the player
                    if (WallInfrontOfPlayer() == true)
                    {
                        //Check if there is a ledge above the player
                        if (LedgeInfrontOfPlayer() == true)
                        {
                            if (RoomAbovePlayer() == true)
                            {
                                //Create and configure the collision check object
                                CreateCollisionCheckObj();
                                //Exit grapple
                                grapple.ExitGrapple();
                            }
                        }
                    }
                }
            }
            else //If the player is no longer in the collision zone or anything
            {
                if (runOnce == false)
                {
                    movement.GravityVelocityReset();
                    firstPerson.ResetCameraSensitivity();
                    runOnce = true;
                }
                Destroy(collisionCheckObj);
                collisionCheckScript = null;
            }
        }
    }

    public bool WallInfrontOfPlayer() //Let the game know if there is room above them
    {
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
        if (Physics.Raycast(rayStart, transform.forward, out forwardCheck, 5f, thisLayerMask))
        {
            if (forwardCheck.collider.gameObject != null)
            {
                return true;
            }
        }
        return false;
    }

    private bool RoomAbovePlayer()
    {
        Vector3 rayStart = transform.position;
        if (Physics.Raycast(rayStart, transform.up, out upwardsCheck, thisCC.height + (thisCC.height / 2), thisLayerMask))
        {
            if (forwardCheck.collider.gameObject != null)
            {
                return false;
            }
        }
        return true;
    }

    private bool LedgeInfrontOfPlayer() //Check if there is a ledge infront of the player
    {
        //Create the downwards raystart empty
        if (downwardsRayStartEmpty == null)
        {
            downwardsRayStartEmpty = new GameObject();
            downwardsRayStartEmpty.name = "Ledge Check Empty";
        }
        //Move the downwards ray start empty to the desired position
        downwardsRayStartEmpty.transform.SetParent(transform);
        downwardsRayStartEmpty.transform.localPosition = new Vector3(0, 3, 3.5f);

        //Do the raycast down from this empty
        if (Physics.Raycast(downwardsRayStartEmpty.transform.position, -transform.up, out downwardsLedgeCheck, thisLayerMask))
        {
            if (downwardsLedgeCheck.collider.gameObject != null)
            {
                return true;
            }
        }
        return false;
    }

    private void CreateCollisionCheckObj() //Create the ledge climb collision check object
    {
        collisionCheckObj = new GameObject();
        collisionCheckObj.name = "Ledge Climb Collision Check";
        collisionCheckObj.transform.SetPositionAndRotation(new Vector3(downwardsLedgeCheck.point.x, downwardsLedgeCheck.point.y + ledgeCollisionZoneYOffset, downwardsLedgeCheck.point.z), transform.rotation);
        collisionCheckObj.AddComponent<CapsuleCollider>().radius = thisCC.radius;
        collisionCheckObj.GetComponent<CapsuleCollider>().height = thisCC.height;
        collisionCheckObj.GetComponent<CapsuleCollider>().isTrigger = true;
        collisionCheckObj.AddComponent<Rigidbody>().isKinematic = true;
        collisionCheckScript = collisionCheckObj.AddComponent<CheckIfCollidingWithObject>();
        //Should make climbing moving platforms work
        if (transform.parent != null)
        {
            collisionCheckObj.transform.SetParent(transform.parent);
        }
    }

    private void PerformLedgeClimb() //This code performs the actual ledge climb
    {
        isLedgeClimbing = true;
        movement.GravityVelocityReset();
        runOnce = false;
        firstPerson.ChangeXSensitivity(reducedX);
        firstPerson.ChangeYSensitivity(reducedY);
        //Turn off player movement
        movement.IsPlayerInputEnabled(false);
        //Move the player towards the collision point
        transform.position = Vector3.Lerp(transform.position, collisionCheckObj.transform.position, ledgeClimbSpeed * Time.deltaTime);
        //If the player has reached the point, allow movement
        float distance = Vector3.Distance(transform.position, collisionCheckObj.transform.position);
        float yDistance = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, collisionCheckObj.transform.position.y, 0));

        //Exit the ledge climb if the player has reached its destination
        if (distance <= 0.5f && yDistance <= 0.05f)
        {
            movement.GravityVelocityReset();
            isLedgeClimbing = false;
            Destroy(collisionCheckObj);
            collisionCheckObj = null;
            collisionCheckScript = null;
            movement.IsPlayerInputEnabled(true);
        }
    }

    public void CancelLedgeClimb() //This will cancel the current ledge climb
    {
        if (collisionCheckObj != null)
        {
            Destroy(collisionCheckObj);
        }
    }

    public bool ReturnIfLedgeClimbing()
    {
        return isLedgeClimbing;
    }

    public void SetLedgeCollisionVariable(bool value)
    {
        inLedgeZone = value;
    }
}
