//Author: James Murphy

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Grapple : MonoBehaviour
{
    [SerializeField]
    private bool isGrappleAllowed = true;
    [SerializeField]
    [Range(0, 500f)]
    private float grappleRange = 150f;
    [Range(0, 100f)]
    [SerializeField]
    private float grappleSpeed = 20f;
    [SerializeField]
    [Range(0, 10)]
    private float grappleCooldown;
    private GameObject grapplePoint, playerCamera, grappleHitPoint, grapplePointPlayerZone;
    private RaycastHit rayhit, intersectionHit;
    [SerializeField]
    private int grapplePointLayer;
    [SerializeField]
    private LayerMask grappleLayerMask, intersectionLayerMask, grappleCollisionMask;
    private bool isGrappling = false;
    private CharacterController thisCC;
    private CharacterControllerMovement movement;
    private FirstPersonCamera fpsCamScript;
    private LineRenderer lineRenderer;
    private float defaultJumpHeight;
    private bool inCooldown = false;
    private GameObject playerParentEmpty, rotationTriggerZone, grapplePointParent, playerRayStart;
    private Crouch crouchScript;
    [SerializeField]
    private GameObject grapplePointPrefab;
    private float momentumNumber = 0, rotatingObjSpeed = 0;
    private bool forceGrappleMomentum = false, grappleBroken = false;
    private GameObject grappleMomentumEmpty, grappleCollisionZone;
    [SerializeField]
    private float grappleTime = 0;
    private float gravityValue;
    private float verticalSpeed;
    private bool cameraSensetivityReduced = false;
    private bool grappleButtonPressed = false, exitGrappleButtonPressed = false, officialMomentumSlide = false;
    //Variables for the enemy pull
    private Transform holdZone;
    private GameObject currentlyGrappledEnemyObj;
    private bool isCarryingObj = false, throwPending = false;
    private GameObject droneThrowPoint;
    [SerializeField]
    private bool inRangeForGrapple = false;
    private Teleporting teleportScript;
    private bool isAtGrappleDestination = true;
    private KeyCode grappleKeycode = KeyCode.Mouse2;
    private PlayerFloorRemoval playerFloorRemovalScript;
    private GameObject emptyHolder, hitpointEmpty;
    private WallClimbV2 wallClimbScript;


    //Get all required components
    private void Start()
    {
        isAtGrappleDestination = true;
        droneThrowPoint = GameObject.Find("LookAtPointThrow");
        holdZone = GameObject.Find("HoldZone").transform;
        playerCamera = Camera.main.gameObject;
        movement = GetComponent<CharacterControllerMovement>();
        gravityValue = movement.GetCurrentGravityStrength();
        crouchScript = GetComponent<Crouch>();
        defaultJumpHeight = movement.ReturnJumpHeight();
        thisCC = GetComponent<CharacterController>();
        teleportScript = GetComponent<Teleporting>();
        playerFloorRemovalScript = GetComponentInChildren<PlayerFloorRemoval>();
        //Get the line renderer
        if (GetComponent<LineRenderer>() != null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            lineRenderer.SetWidth(0.1f, 0.1f);
            lineRenderer.SetColors(Color.black, Color.black);
        }
        fpsCamScript = Camera.main.GetComponent<FirstPersonCamera>();
        wallClimbScript = GetComponent<WallClimbV2>();
        //Make sure all children are layered player
        foreach (Transform t in transform)
        {
            t.gameObject.layer = 8;
        }
    }

    private void FixedUpdate() //Run the check intersection code here
    {
        if (movement != null)
        {
            defaultJumpHeight = movement.ReturnDefaultJumpHeight();
        }
        if (isGrappling == true)
        {
            //Check if anything is in the way of the grapple and if so cancel the grapple
            CheckIfGrappleIntersected();
        }

        //Keep the movement script updated whether it is grappling or not
        movement.SetIsGrapplingValue(isGrappling);

        if (forceGrappleMomentum == true)
        {
            GrappleMomentumCode();
        }

        //Only allow grapple if the bool is true
        if (isGrappleAllowed == true)
        {
            //Check to see if the grapple is allowed
            RangeRaycast();
            //If middle mouse pressed
            if (grappleButtonPressed == true && inCooldown == false)
            {
                grappleButtonPressed = false;
                CreateGrapplePoint();
            }

            //If the grapple point is not null, pull the player towards the point
            if (isGrappling == true)
            {
                GrapplePullTowards();
                //Make sure the grapple hook is rendered
                RenderGrappleHook();
            }
            else //Disable the grapple line
            {
                //Only run this code if there is a line renderer component
                if (lineRenderer != null && isCarryingObj == false)
                {
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    private void RangeRaycast() //Raycast the range of the grapple
    {
        RaycastHit localRayHit;
        if (Physics.Raycast(transform.position, playerCamera.transform.forward, out localRayHit, grappleRange, grappleLayerMask))
        {
            if (localRayHit.collider.gameObject.tag == "enemy" || localRayHit.collider.gameObject.tag == "MoveableObjects")
            {
                inRangeForGrapple = true;
            }
            else if (localRayHit.collider.gameObject.layer != 12 && localRayHit.collider.gameObject.layer != 22)
            {
                inRangeForGrapple = true;
            }
        }
        else //If the player cannot grapple
        {
            inRangeForGrapple = false;
        }

        //Display the grapple cooldown to indicate whether the player can grapple or not
        if (UIElements.singleton.cooldownGrapple != null)
        {
            if (inRangeForGrapple == true && localRayHit.collider.gameObject.layer != 12)
            {
                UIElements.singleton.cooldownGrapple.enabled = true;
            }
            else
            {
                UIElements.singleton.cooldownGrapple.enabled = false;
            }
        }
    }

    //Key press will be checked here
    private void Update()
    {
        if (Input.GetKeyDown(grappleKeycode) && inRangeForGrapple == true && isAtGrappleDestination == true && teleportScript.ReturnIfTeleporting() == false && inCooldown == false && isCarryingObj == false && teleportScript.ReturnIfTeleportButtonHeld() == false)
        {
            grappleButtonPressed = true;
        }

        //Exit carry obj if needed
        if (Input.GetKeyDown(grappleKeycode) && isCarryingObj == true)
        {
            grappleButtonPressed = false;
            ExitCarryObj();
        }

        if (isCarryingObj == true && currentlyGrappledEnemyObj != null) //Pull enemy towards the player
        {
            PullObjTowardsSelf();
            //If the player is dragging the enemy, render the grapple hook
            RenderGrappleHook();
        }

        //If the player is grappling check for the exit code
        if (isGrappling == true)
        {
            CheckForExitCode();
        }

    }

    private void GrappleMomentumCode() //This is where the player will have momentum when jumping off the grapple
    {
        if (grappleMomentumEmpty != null)
        {
            //Apply the player momentum
            grappleMomentumEmpty.transform.Translate(-Vector3.right * (momentumNumber / 6 * Time.deltaTime), Space.Self);
            //If the grapple is broken reduce the amount of momentum
            if (grappleBroken == true)
            {
                grappleMomentumEmpty.transform.Translate(-Vector3.right * (momentumNumber / 10 * Time.deltaTime), Space.Self);
            }
            transform.SetParent(grappleMomentumEmpty.transform);

            //Only apply gravity here if it is not applied in the movement script
            if (movement.CheckIfPlayerInputEnabled() == false)
            {
                //Apply gravity to the player as gravity might not be applied
                Vector3 direction = Vector3.zero;
                //Apply gravity acceleration to vertical speed
                verticalSpeed -= gravityValue / 80 * Time.deltaTime;
                //Set the vertical speed
                direction.y = verticalSpeed;
                thisCC.Move(direction);
                //If the player has landed
            }

            //if there is an error stop sliding
            try
            {
                if (rayhit.collider.gameObject != null)
                {
                    if (movement.CheckIfHitWall(-grappleMomentumEmpty.transform.right, false, rayhit.collider.gameObject) == true)
                    {
                        momentumNumber = 0;
                    }
                }
            }
            catch //This will stop the sliduing if there is an error
            {
                momentumNumber = 0;
            }

            //Do not allow player movement if the player is going to fast
            if (momentumNumber >= 50)
            {
                movement.IsPlayerInputEnabled(false);
            }
            else //Allow player movement if the player is going slow enough
            {
                movement.IsPlayerInputEnabled(true);
            }

            //If the player has hit the ground
            if (movement.ReturnObjectPlayerIsStandingOnAccurate() != null)
            {
                if (grappleBroken == false && momentumNumber >= 0 && movement.CheckIfHitWall(-grappleMomentumEmpty.transform.right, false, null) == false)
                {
                    if (cameraSensetivityReduced == false)
                    {
                        cameraSensetivityReduced = true;
                        //Reduce the camera movement speed
                        fpsCamScript.ChangeXSensitivity((fpsCamScript.GetCurrentXSensitivity() / 6));
                        fpsCamScript.ChangeYSensitivity((fpsCamScript.GetCurrentYSensitivity() / 5));
                    }
                    //Do not allow player movement if the player is going to fast
                    if (momentumNumber >= 50)
                    {
                        movement.IsPlayerInputEnabled(false);
                    }
                    else //Allow player movement if the player is going slow enough
                    {
                        movement.IsPlayerInputEnabled(true);
                        CancelGrappleMomentum();
                    }
                    //If you land and are crouching enter a slide
                    if (crouchScript.IsPlayerCrouching() == true)
                    {
                        officialMomentumSlide = true;
                        crouchScript.ManuallyChangeCrouchState(true);
                        momentumNumber = momentumNumber - (Time.deltaTime * 120);
                    }
                    else //If you land and are no crouching dont enter a slide
                    {
                        officialMomentumSlide = false;
                        momentumNumber = momentumNumber - (Time.deltaTime * 270);
                    }

                    grappleMomentumEmpty.transform.Translate(-Vector3.right * (momentumNumber / 10 * Time.deltaTime), Space.Self);
                }
                else //If the landing slide has ended then stop moving
                {
                    officialMomentumSlide = false;
                    cameraSensetivityReduced = false;
                    //Reset camera sensetivity
                    fpsCamScript.ResetCameraSensitivity();
                    verticalSpeed = 0;
                    movement.IsPlayerInputEnabled(true);
                    momentumNumber = 0;
                    forceGrappleMomentum = false;
                    transform.SetParent(null);
                    Destroy(grappleMomentumEmpty);
                    if (crouchScript.AllowedToExitCrouch() == true)
                    {
                        //Make the player just slightly whilst landing to get them out of any surface
                        movement.TriggerForcedDescent(2f);
                        movement.TriggerForcedJump(2f);
                    }
                }
            }
        }
        else //If there is no grapple momentum block pulling the player
        {
            officialMomentumSlide = false;
            momentumNumber = 0;
            forceGrappleMomentum = false;
            //Reset camera sensetivity
            fpsCamScript.ResetCameraSensitivity();
        }
    }

    private void CancelGrappleMomentum() //This will cancel grapple momentum if the player is going slow enough
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space)) //If any of the movement 
        {
            verticalSpeed = 0;
            movement.IsPlayerInputEnabled(true);
            momentumNumber = 0;
            forceGrappleMomentum = false;
            transform.SetParent(null);
            if (grappleMomentumEmpty != null)
            {
                Destroy(grappleMomentumEmpty);
            }
            //Make the player just slightly whilst landing to get them out of any surface
            movement.TriggerForcedJump(1);
        }
    }

    public void ForceCancelGrappleMomentum() //Cancels any momentum from grapple
    {
        verticalSpeed = 0;
        movement.IsPlayerInputEnabled(true);
        momentumNumber = 0;
        forceGrappleMomentum = false;
        transform.SetParent(null);
        if (grappleMomentumEmpty != null)
        {
            Destroy(grappleMomentumEmpty);
        }
        //Make the player just slightly whilst landing to get them out of any surface
        movement.TriggerForcedJump(1);
    }

    private void RenderGrappleHook() //Render the grapple hook
    {
        //Only run this code if there is a referenced line renderer component
        if (lineRenderer != null && grappleHitPoint != null)
        {
            //Set the origin point
            lineRenderer.SetPosition(0, transform.position);
            //Set the hit point
            lineRenderer.SetPosition(1, grappleHitPoint.transform.position);
            //Enable the grapple line
            lineRenderer.enabled = true;
            AudioManage.inst.grapple.Play();
        }
    }

    private void CheckForExitCode() //This will check for key inputs 
    {
        //If the player has jumped, force a limited jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //If you are close to the grapple point (allow the player to jump off to simulate a wall jump for example)
            if (Vector3.Distance(transform.position, grapplePointPlayerZone.transform.position) <= 0.3f)
            {
                //movement.TriggerForcedJump(defaultJumpHeight);
            }
            movement.GravityVelocityReset();
            //Regardless exit grapple
            ExitGrapple();
        }
        else if (movement.ReturnObjectPlayerIsStandingOnGrappleCheck() != null && grappleTime >= 0.2f)
        {
            movement.GravityVelocityReset();
            //movement.TriggerForcedJump(4f);
            ExitGrapple();
        }

    }

    private void CreateGrapplePoint() //Grapple code will be run here
    {
        //Create the raycast at the specified position
        if (Physics.Raycast(transform.position, playerCamera.transform.forward, out rayhit, grappleRange, grappleLayerMask))
        {
            if (rayhit.collider.gameObject.tag == "enemy" || rayhit.collider.gameObject.tag == "MoveableObjects")
            {
                //If the enemy is a drone
                if (rayhit.collider.gameObject.GetComponent<Drone>() != null || rayhit.collider.GetComponent<PickupAbleObj>() != null)
                {
                    emptyHolder = null;
                    hitpointEmpty = null;
                    //Exit any prior grapples
                    ExitGrapple();
                    ExitCarryObj();
                    //Set this enemy as the current grappled enemy
                    currentlyGrappledEnemyObj = rayhit.collider.gameObject;
                    //Run the enemy pull towards code
                    SetUpObjPullTowards();
                }
            }
            //If the object layer isn't on the stop grapple layer
            else if (rayhit.collider.gameObject.layer != 12 && rayhit.collider.gameObject.layer != 22)
            {
                //Temporarily disable the floor removal raycast to remove some grapple jank with small objects
                playerFloorRemovalScript.TempDisable(0.05f);
                //Stop all current movement
                movement.GravityVelocityReset();
                ExitCarryObj();
                grappleTime = 0;
                StopCoroutine(DelayMomentum());
                //Mark grapple as unbroken
                grappleBroken = false;
                //Set grapple momentum to 0
                forceGrappleMomentum = false;
                momentumNumber = 0;
                rotatingObjSpeed = 0;
                //Force exit other grapples
                ExitGrapple();
                //Wipe rotation zones from the triggers as they are no longer needed
                Optimization.singleton.RemoveAllRotationZones();
                //Cancel prior grapples
                isGrappling = false;
                //Unparent the player 
                transform.SetParent(null);
                //Disable movement
                movement.IsPlayerInputEnabled(false);
                //Disable jump
                movement.JumpEnabled(false);
                //Destroy the previous grapple point
                if (grapplePoint != null)
                {
                    Destroy(grapplePoint);
                }
                //Create the new grapple point
                this.gameObject.transform.SetParent(null);
                grapplePoint = Instantiate(grapplePointPrefab);
                grapplePoint.transform.rotation = transform.rotation;
                //Get the child zone
                grapplePointPlayerZone = grapplePoint.transform.GetChild(0).gameObject;
                //Create the grapple point hit
                grappleHitPoint = new GameObject();
                grappleHitPoint.name = "Grapple Hit point";
                //Create and position the grapple hit point (Used for rendering the line)
                grappleHitPoint.transform.position = rayhit.point;
                //Make a rayhit empty
                if (hitpointEmpty != null)
                {
                    Destroy(hitpointEmpty);
                }
                hitpointEmpty = new GameObject();
                hitpointEmpty.name = "Grapple rayhit empty";
                hitpointEmpty.transform.position = rayhit.point;
                hitpointEmpty.transform.SetParent(rayhit.collider.transform);
                hitpointEmpty.transform.localScale = new Vector3(1, 1, 1);
                //Hit point empty holder
                if (emptyHolder != null)
                {
                    Destroy(emptyHolder);
                }
                emptyHolder = new GameObject();
                emptyHolder.name = "Hit Point Empty Holder";
                emptyHolder.transform.position = grappleHitPoint.transform.position;
                grappleHitPoint.transform.SetParent(emptyHolder.transform);
                //Create the grapple point empty 
                if (grapplePointParent != null)
                {
                    Destroy(grapplePointParent);
                }
                grapplePointParent = new GameObject();
                grapplePointParent.name = "GrapplePointParent";
                grapplePointParent.transform.position = grappleHitPoint.transform.position;
                grapplePointParent.transform.rotation = grappleHitPoint.transform.rotation;
                grapplePointParent.transform.localScale = new Vector3(1, 1, 1);
                //Configure the grapple point
                grapplePoint.GetComponent<MeshRenderer>().enabled = false;
                grapplePoint.name = "Grapple point";
                grapplePoint.layer = grapplePointLayer;
                grapplePoint.transform.position = rayhit.point;
                grapplePoint.transform.SetParent(grappleHitPoint.transform);
                //Spawn the grapple collision zone
                if (grappleCollisionZone == null)
                {
                    grappleCollisionZone = new GameObject();
                    grappleCollisionZone.name = "Grapple Collision Zone";
                    grappleCollisionZone.AddComponent<GrappleCollisions>();
                    grappleCollisionZone.GetComponent<GrappleCollisions>().playerCC = thisCC;
                    grappleCollisionZone.GetComponent<GrappleCollisions>().rayLayerMask = grappleCollisionMask;
                    grappleCollisionZone.SetActive(true);
                }
                //Set the grapple collider position and rotation
                grappleCollisionZone.transform.SetPositionAndRotation(transform.position, transform.rotation);
                grappleCollisionZone.GetComponent<GrappleCollisions>().objectGrappledTo = rayhit.collider.gameObject;
                //If it is a rotation zone get the rotation zone object
                rotationTriggerZone = null;
                //Check to see if it is a rotating object
                if (rayhit.collider.gameObject.GetComponent<ObjectRotation>() != null)
                {
                    rotationTriggerZone = rayhit.collider.gameObject.GetComponent<ObjectRotation>().ReturnTriggerZone();
                    rotatingObjSpeed = rayhit.collider.gameObject.GetComponent<ObjectRotation>().ReturnRotationSpeed();
                    //IF OBJECT ROTATING ON Y THEN RUN THIS
                    if (rotationTriggerZone != null)
                    {
                        //Make the rotation empty
                        playerParentEmpty = new GameObject();
                        playerParentEmpty.name = "PlayerParentEmpty";
                        playerParentEmpty.transform.localScale = transform.localScale;
                        playerParentEmpty.transform.position = rotationTriggerZone.transform.localPosition;
                        playerParentEmpty.transform.rotation = rotationTriggerZone.transform.localRotation;
                        grapplePoint.transform.SetParent(grapplePointParent.transform);
                        transform.SetParent(playerParentEmpty.transform);

                    }
                }
                else //Destroy the parent object if it is not needed
                {
                    Destroy(grapplePointParent);
                }
                //Start grappling again
                inCooldown = true;
                StopCoroutine(Cooldown());
                StartCoroutine(Cooldown());
                isGrappling = true;
            }
        }
    }

    private void SetUpObjPullTowards()
    {
        //Create the grapple hit point
        grappleHitPoint = new GameObject();
        grappleHitPoint.name = "Grapple Hit point";
        //Create and position the grapple hit point (Used for rendering the line)
        grappleHitPoint.transform.position = currentlyGrappledEnemyObj.transform.position;
        grappleHitPoint.transform.SetParent(currentlyGrappledEnemyObj.transform);
        //Set up the object
        if (currentlyGrappledEnemyObj.GetComponent<Drone>() != null)
        {
            //Turn off the drone movement script
            currentlyGrappledEnemyObj.GetComponent<Drone>().enabled = false;
        }
        //Set up the triggers
        currentlyGrappledEnemyObj.GetComponent<Collider>().isTrigger = true;
        currentlyGrappledEnemyObj.GetComponent<Rigidbody>().isKinematic = true;
        //Mark this as true so the line renderer will... render
        isCarryingObj = true;
    }

    private void PullObjTowardsSelf() //This code will pull the enemy towards the player
    {
        //Drop the enemy when sliding
        if (crouchScript.IsPlayerSliding() == true)
        {
            ExitCarryObj();
        }
        try
        {
            if (currentlyGrappledEnemyObj != null)
            {
                //Check if the grapple has been intersected
                CheckIfGrappleIntersected();
                //This line pulls the enemy towards the player hold zone
                currentlyGrappledEnemyObj.transform.SetPositionAndRotation(Vector3.MoveTowards(rayhit.collider.transform.position, holdZone.position, (grappleSpeed * Time.deltaTime)), holdZone.rotation);
            }
        }
        catch //if there is an error exit grapple
        {
            ExitCarryObj();
        }

        //If the player right clicks whilst holding the enemy exit the grapple and throw them
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            //Mark the throw pending as true
            throwPending = true;
            //Exit grapple
            ExitCarryObj();
        }
    }

    private void GrapplePullTowards() //This code will actually pull the player towards
    {
        //Only run this code if grappling is true
        if (isGrappling == true)
        {
            //Make sure the empty is at the correct place
            if (emptyHolder != null && hitpointEmpty != null)
            {
                emptyHolder.transform.position = hitpointEmpty.transform.position;
            }
            //Increase the amount of time in grapple
            grappleTime += Time.deltaTime;
            //Increase the momentum number
            momentumNumber += rotatingObjSpeed * (Time.deltaTime * 2);
            if (momentumNumber > rotatingObjSpeed)
            {
                momentumNumber = rotatingObjSpeed;
            }
            //Force the grapple size to be the size of the player
            grapplePoint.transform.localScale = new Vector3(2, 2, 2);
            //Only pull the player in if they are not close enough to the grapple zone
            if (Vector3.Distance(transform.position, grapplePointPlayerZone.transform.position) >= 0.05f)
            {
                isAtGrappleDestination = false;
                //Turn off the grapple collision zone
                grappleCollisionZone.SetActive(true);
                //This is the actual grapple code
                transform.position = Vector3.MoveTowards(transform.position, grapplePointPlayerZone.transform.position, (grappleSpeed * Time.deltaTime));
            }
            else //If the player is at the grapple destination
            {
                isAtGrappleDestination = true;
                //Turn off the grapple collision zone
                grappleCollisionZone.SetActive(false);
            }
            //Make the player parent zone follow the trigger rotation
            if (playerParentEmpty != null && rotationTriggerZone != null)
            {
                playerParentEmpty.transform.position = rotationTriggerZone.transform.localPosition;
                playerParentEmpty.transform.rotation = rotationTriggerZone.transform.localRotation;
            }
            if (grapplePointParent != null && grappleHitPoint != null)
            {
                grapplePointParent.transform.position = grappleHitPoint.transform.position;
                grapplePointParent.transform.rotation = grappleHitPoint.transform.rotation;
            }
            try //if there is an error exit grapple
            {
                //This should help grapple not go through surfaces
                if (rayhit.collider.gameObject != null && movement.ReturnObjectPlayerIsStandingOnAccurate() != null)
                {
                    if (movement.ReturnObjectPlayerIsStandingOnGrappleCheck() == rayhit.collider.gameObject)
                    {
                        ExitGrapple();
                    }
                }
            }
            catch
            {
                ExitGrapple();
            }
        }
    }

    private void CheckIfGrappleIntersected() //Will check if anything is in the way of the grapple
    {
        if (playerRayStart == null)
        {
            playerRayStart = new GameObject();
            playerRayStart.name = "Player Ray Start";
        }
        playerRayStart.transform.position = transform.position;
        playerRayStart.transform.LookAt(grappleHitPoint.transform.position);
        //Raycast point
        if (Physics.Raycast(playerRayStart.transform.position, playerRayStart.transform.forward, out intersectionHit, grappleRange, grappleLayerMask))
        {
            try
            {
                if (rayhit.collider.gameObject != null && intersectionHit.collider.gameObject != null && intersectionHit.collider.gameObject != rayhit.collider.gameObject)
                {
                    grappleBroken = true;
                    ExitGrapple();
                    ExitCarryObj();
                }
            }
            catch
            {
                ExitGrapple();
                ExitCarryObj();
            }
        }
    }

    private void ExitCarryObj() //Will exit holding an object
    {
        //This code is for dropping an enemy
        if (isCarryingObj == true)
        {
            isAtGrappleDestination = true;
            //Activate the enemy again if needed
            if (currentlyGrappledEnemyObj != null)
            {
                if (currentlyGrappledEnemyObj.GetComponent<Drone>() != null) //Only if it is an enemy
                {
                    Drone droneScript = currentlyGrappledEnemyObj.GetComponent<Drone>();
                    //Enable and stun the enemy
                    droneScript.enabled = true;
                }
                currentlyGrappledEnemyObj.GetComponent<Collider>().isTrigger = false;
                currentlyGrappledEnemyObj.GetComponent<Rigidbody>().isKinematic = false;
                //Throw the enemy if needed
                if (throwPending == true)
                {
                    if (currentlyGrappledEnemyObj.GetComponent<Drone>() != null) //Only if enemy
                    {
                        Drone droneScript = currentlyGrappledEnemyObj.GetComponent<Drone>();
                        //Deactivate any previous stuns
                        droneScript.StopAllCoroutines();
                        //Only stun the enemy when thrown
                        droneScript.StartCoroutine(droneScript.ActivateStun(5f));
                    }
                    //Throw the enemy forward
                    GameObject objThrowEmpty = new GameObject();
                    objThrowEmpty.name = "droneThrowEmpty";
                    objThrowEmpty.transform.SetPositionAndRotation(currentlyGrappledEnemyObj.transform.position, currentlyGrappledEnemyObj.transform.rotation);
                    objThrowEmpty.transform.SetParent(playerCamera.transform);

                    objThrowEmpty.transform.LookAt(droneThrowPoint.transform.position);
                    currentlyGrappledEnemyObj.GetComponent<Rigidbody>().AddForce(objThrowEmpty.transform.forward * 3500);
                    throwPending = false;
                }
                else //Throw the enemy off to one side
                {
                    if (currentlyGrappledEnemyObj.GetComponent<Drone>() != null) //Only if enemy
                    {
                        Drone droneScript = currentlyGrappledEnemyObj.GetComponent<Drone>();
                        //Deactivate any previous stuns
                        droneScript.StopAllCoroutines();
                        //Only stun the enemy when thrown
                        droneScript.StartCoroutine(droneScript.ActivateStun(2f));
                    }
                    //Throw the enemy forward
                    currentlyGrappledEnemyObj.GetComponent<Rigidbody>().AddForce((-playerCamera.transform.right + transform.forward) * 500);
                }
                //Nullify the enemy variable                   
                currentlyGrappledEnemyObj = null;
            }
            lineRenderer.enabled = false;
            isCarryingObj = false;
        }
    }

    public void ExitGrapple() //This code will exit grapple and enable movement again
    {
        isAtGrappleDestination = true;
        grappleTime = 0;
        if (momentumNumber != 0 && forceGrappleMomentum == false)
        {
            if (grappleMomentumEmpty != null)
            {
                Destroy(grappleMomentumEmpty);
            }
            grappleMomentumEmpty = new GameObject();
            grappleMomentumEmpty.name = "grappleMomentumEmpty";
            grappleMomentumEmpty.transform.position = transform.position;
            grappleMomentumEmpty.transform.LookAt(rotationTriggerZone.transform);
            StartCoroutine(DelayMomentum());
        }
        if (grappleCollisionZone != null)
        {
            grappleCollisionZone.SetActive(false);
        }
        isGrappling = false;
        transform.SetParent(null);
        movement.IsPlayerInputEnabled(true);
        if (grapplePointParent != null)
        {
            Destroy(grapplePointParent);
        }
        if (emptyHolder != null)
        {
            Destroy(emptyHolder);
        }
        if (hitpointEmpty != null)
        {
            Destroy(hitpointEmpty);
        }
        if (playerParentEmpty != null)
        {
            transform.SetParent(null);
            Destroy(playerParentEmpty);
        }
        if (grapplePoint != null)
        {
            Destroy(grapplePoint);
        }
        if (grappleHitPoint != null)
        {
            Destroy(grappleHitPoint);
        }
        movement.JumpEnabled(true);
        movement.SetJumpHeight(6);
        if (wallClimbScript != null) //Allow another wallrun
        {
            wallClimbScript.AllowAnotherRun();
        }
    }

    private IEnumerator DelayMomentum() //Give the player some movement time so they can get up on high platforms
    {
        yield return new WaitForSeconds(0.01f);
        forceGrappleMomentum = true;
    }

    private IEnumerator Cooldown() //Cooldown for the grapple
    {
        StopCoroutine(FillUIElement());
        StartCoroutine(FillUIElement());
        yield return new WaitForSeconds(grappleCooldown);
        inCooldown = false;
    }

    private IEnumerator FillUIElement() //Fill the teleport UI element
    {
        if (UIElements.singleton.gameObject != null && UIElements.singleton.cooldownGrapple != null)
        {
            float time = grappleCooldown;
            //While there is still a cool down
            while (time > 0)
            {
                //Reduce the time so that the while loop will not crash the game
                time -= Time.deltaTime;
                //Fill the image
                UIElements.singleton.cooldownGrapple.fillAmount = (grappleCooldown - time) / grappleCooldown;
                //  Debug.Log("RUN RUN RUN ");
                yield return null;
            }
        }
    }

    private IEnumerator EnableJumpAgain() //Enable jump again with a delay to stop double jumping when coming off the grapple
    {
        yield return new WaitForSeconds(1f);
        movement.JumpEnabled(true);
    }

    public void SetGrappleAllowedValue(bool value)
    {
        isGrappleAllowed = value;
    }

    public bool ReturnIfGrappleSliding()
    {
        return officialMomentumSlide;
    }

    public bool IsCurrentlyGrappling() //Return the value of whether the player is grappling
    {
        return isGrappling;
    }

    public bool IsGrappleAllowed()
    {
        return isGrappleAllowed;
    }

    public float ReturnGrappleRange()
    {
        return grappleRange;
    }

    public void SetGrappleRange(float range)
    {
        grappleRange = range;
    }

    public bool isMomentumSliding() //Return a bool on whether the player is momentum sliding
    {
        return forceGrappleMomentum;
    }

    public bool isHoldingEnemy() //Return the bool for holding the enemy
    {
        return isCarryingObj;
    }

    public void SetGrappleKeycode(KeyCode keyToSet) //Allow the user to set the grapple keycode
    {
        grappleKeycode = keyToSet;
    }

}
