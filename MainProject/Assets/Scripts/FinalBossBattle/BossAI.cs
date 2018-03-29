//Author: James Murphy
//Purpose: To control the boss ai

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour
{
    [SerializeField]
    [Header("Required Object References")]
    private Canvas healthCanvas;
    [SerializeField]
    private GameObject turretObjRef;
    private BossCameraShake cameraShakeScript;
    [SerializeField]
    private Transform frontWheels, backWheels, bossModelTurret, phase4Turret, engineParticle, rocketParticle, sakerDriver;
    [SerializeField]
    private Rigidbody truckRigidbody;
    private enum bossPhases { phase1, phase2, phase3, phase4 };
    [SerializeField]
    private BossHealthBar bossHealthBar;
    [SerializeField]
    private GameObject wholeBossLevelObj, platformingLevelObj, playerSpawnPoint, bigExplosion;
    [SerializeField]
    [Header("Boss Information")]
    private bossPhases currentBossPhase = bossPhases.phase1;
    [Header("Movement Settings")]
    [SerializeField]
    private float movementSpeed = 10f;
    [SerializeField]
    private float movementRange = 10f, miniumCooldownTime = 2f, maxiumumCooldownTime = 4f;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private GameObject directionDecideCube, targetPositionCube;
    private float timer = 0, timerTarget = 0;
    private bool coolDown = false, movementAllowed = true;
    [SerializeField]
    [Header("Attack Information")]
    private LayerMask rayAttackLayer;
    [SerializeField]
    private int meleeDamage = 20;
    private ReusableHealth playerHealthScript, bossReusableHealth;
    [Header("Misc")]
    [SerializeField]
    private int ammoToReplenishEachPhase = 40;
    //Variables for the final phase
    [Header("Phase 4 References")]
    [SerializeField]
    private Transform[] movementTargetArray;
    private int targetID = 0;
    [SerializeField]
    private bool finalPhaseReady = false;
    private float bossFlipTargetRotation = 20f;
    [SerializeField]
    private int randomSeed;
    private Quaternion targetRotation;
    private bool bossDead = false;
    [SerializeField]
    private bool skipBoss = false;
    private bool beganTransition = false;
    private AmmoManager ammoManagerScript;
    private float secondPhaseDodgeTimer = 0;

    private void Start() //Get the default position and rotations
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
        //Create the direction decide cube
        directionDecideCube = new GameObject();
        directionDecideCube.transform.SetParent(transform);
        directionDecideCube.name = "directionDecideCube";
        directionDecideCube.transform.SetPositionAndRotation(transform.position, transform.rotation);
        bossReusableHealth = GetComponentInChildren<ReusableHealth>();
        bossReusableHealth.enabled = false;
        Invoke("DelayedStart", 0.2f);
        randomSeed = System.DateTime.Now.Millisecond;
        Random.InitState(randomSeed);
    }

    private void DelayedStart()
    {
        //Get the character controller movement script
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ReusableHealth>();
        ammoManagerScript = playerHealthScript.gameObject.GetComponent<AmmoManager>();
        if (skipBoss == true)
        {
            if (beganTransition == false)
            {
                StartCoroutine(BeginPlatformingTransition(2f));
            }
        }
        //Give player vague instructions
        QuestManager.inst.SubtitleText("Kingsley: Okay, focus your fire on the turret!", 6f);
    }

    private void FixedUpdate()
    {
        //This will run the wheel rotation code
        ControlWheelRotation();
        //Increment the timer
        if (coolDown == true)
        {
            timer = timer + Time.fixedDeltaTime;
        }
        //Decide what phase to enter
        switch (currentBossPhase)
        {
            case bossPhases.phase1:
                Phase1();
                break;
            case bossPhases.phase2:
                Phase2();
                break;
            case bossPhases.phase3:
                Phase3();
                break;
            case bossPhases.phase4:
                Phase4();
                break;
        }
    }

    private void ReplenishAmmo() //Replenish player ammo
    {
        ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.machineGun, ammoToReplenishEachPhase);
        ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.shotgun, ammoToReplenishEachPhase);
        //Make kingsley say you have some ammo
        QuestManager.inst.SubtitleText("Heres some ammo!", 3f);
    }

    private void ControlWheelRotation() //Control the car wheel rotation
    {
        if (frontWheels != null && directionDecideCube != null)
        {
            float targetYRotation = 0;
            if (directionDecideCube.transform.rotation.y > 0)
            {
                targetYRotation = 16;
            }
            else if (directionDecideCube.transform.rotation.y < 0)
            {
                targetYRotation = -16;
            }
            frontWheels.rotation = Quaternion.Slerp(frontWheels.rotation, Quaternion.Euler(frontWheels.rotation.x, targetYRotation, frontWheels.rotation.z), Time.fixedDeltaTime * 4F);
        }
    }

    private IEnumerator TemporarilyStopMovement(float value) //Temporarily stop the player movement
    {
        movementAllowed = false;
        yield return new WaitForSeconds(value);
        coolDown = true;
        movementAllowed = true;
    }

    private void Phase1() //Causes the car to move about and shoot the player, will move to phase 2 when the turret has been destroyed
    {
        //Turn off the healthscript
        bossReusableHealth.SetInvincibleValue(true);
        //if the turret has been destroy go into phase 2
        if (turretObjRef == null)
        {
            //Stop the pause movement coroutine
            StopAllCoroutines();
            //Reset the timer
            coolDown = false;
            timer = 0;
            timerTarget = 0;
            //Only allow phase 2 once the object has return to the default position
            if (Vector3.Distance(transform.position, defaultPosition) < 0.5f)
            {
                bossReusableHealth.SetInvincibleValue(false);
                //Turn on the boss health
                bossReusableHealth.enabled = true;
                //Increase the movement speed of the boss by 25%
                movementSpeed = movementSpeed + (movementSpeed / 4);
                //Replenish Ammo
                ReplenishAmmo();
                //Give player vague instructions
                QuestManager.inst.SubtitleText("Kingsley: That'll teach him! Now just shoot the truck, shooting the tires and the big ass rocket will do extra damage!", 7);
                currentBossPhase = bossPhases.phase2;
            }
            //Set the target move position to default
            else if (targetPositionCube != null)
            {
                //Allow movement
                movementAllowed = true;
                //Reset the target position
                targetPositionCube.transform.position = defaultPosition;
            }
        }
        //Delete the position cube if the cooldown has been finished and the timer has reached its target
        if (coolDown == true && timer >= timerTarget)
        {
            Destroy(targetPositionCube);
            targetPositionCube = null;
            timer = 0;
            timerTarget = 0;
            coolDown = false;
        }
        //Create the target position for the ai if needed
        if (targetPositionCube == null)
        {
            //Work out the direction the movement point will be in
            directionDecideCube.transform.rotation = Quaternion.Euler(transform.rotation.x, Random.Range(-90, 90), transform.rotation.z);
            //Create the movement point
            targetPositionCube = new GameObject();
            targetPositionCube.name = "Target Position Cube";
            targetPositionCube.transform.position = directionDecideCube.transform.position + directionDecideCube.transform.forward * movementRange;
        }
        //Move towards the target position
        if (targetPositionCube != null && movementAllowed == true)
        {
            float step = movementSpeed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPositionCube.transform.position, step);
        }

        //If the enemy has reached the target go back to the default position and activate the cooldown
        if (Vector3.Distance(transform.position, targetPositionCube.transform.position) <= 0.5f)
        {
            if (coolDown == false)
            {
                targetPositionCube.transform.position = defaultPosition;
                //enter the cooldown
                timer = 0;
                //Randomley select the cooldown time
                timerTarget = Random.Range(miniumCooldownTime, maxiumumCooldownTime);
                //enable the cooldown
                StartCoroutine(TemporarilyStopMovement(2));
            }
            else //Set the wheels back to the default rotation
            {
                directionDecideCube.transform.rotation = Quaternion.Euler(directionDecideCube.transform.rotation.x, 0, directionDecideCube.transform.rotation.z);
            }
        }
    }

    private void Phase2() //Car will deploy spikes and bump into the player
    {
        //The conditions needed for phase 3
        if (bossReusableHealth.healthValue <= 50) //If the boss has less than 30 health
        {
            //Stop the pause movement coroutine
            StopAllCoroutines();
            //Reset the timer
            coolDown = false;
            timer = 0;
            timerTarget = 0;
            //Increase the health of the car again
            bossReusableHealth.maxHealth = 400;
            bossReusableHealth.maxArmor = 400;
            bossReusableHealth.healthValue = 49;
            bossReusableHealth.armorValue = 400;
            //Only allow phase 3 once the object has return to the default position
            if (Vector3.Distance(transform.position, defaultPosition) < 0.5f)
            {
                //Turn off boss car health
                bossReusableHealth.enabled = true;
                if (frontWheels != null && backWheels != null)
                {
                    frontWheels.tag = "enemy";
                    backWheels.tag = "enemy";
                }
                //Set the target as the default position
                targetPositionCube.transform.position = defaultPosition;
                //Increase the movement speed of the boss by 25%
                movementSpeed = movementSpeed + (movementSpeed / 4);
                //Turn the boss to shoot on
                bossModelTurret.gameObject.SetActive(true);
                //Increase the health of the car again
                bossReusableHealth.maxHealth = 400;
                bossReusableHealth.maxArmor = 400;
                bossReusableHealth.healthValue = 250;
                bossReusableHealth.armorValue = 400;
                bossReusableHealth.glowMaterialToUse = bossReusableHealth.glowMaterialRegularHit;
                //Replenish Ammo
                ReplenishAmmo();
                //Remove driver saker
                if (sakerDriver != null)
                {
                    sakerDriver.gameObject.SetActive(false);
                }
                //Enter phase 3
                currentBossPhase = bossPhases.phase3;
            }
            //Set the target move position to default
            else if (targetPositionCube != null)
            {
                //Allow movement
                movementAllowed = true;
                //Reset the target position
                targetPositionCube.transform.position = defaultPosition;
            }
        }
        //Reset cooldown if needed
        if (timer > timerTarget && coolDown == true)
        {
            coolDown = false;
            timer = 0;
        }
        if (coolDown == false)
        {
            //If allowed to dodge
            secondPhaseDodgeTimer = secondPhaseDodgeTimer + Time.fixedDeltaTime;
        }

        float distanceBetweenThisAndPlayer = Vector3.Distance(transform.position, new Vector3(playerHealthScript.transform.position.x, transform.position.y, playerHealthScript.transform.position.z));
        //if the cooldown is active and the target position is the default position, then set a new position
        if (coolDown == true && targetPositionCube.transform.position == defaultPosition)
        {
            targetPositionCube.transform.position = new Vector3(defaultPosition.x + Random.Range(-4, 4), defaultPosition.y, defaultPosition.z);
        }
        else if (coolDown == false && targetPositionCube.transform.position != defaultPosition) //Set the boss to attack the player
        {
            //Set the target as the player
            targetPositionCube.transform.position = new Vector3(playerHealthScript.transform.position.x, transform.position.y, playerHealthScript.transform.position.z);
        }

        if (distanceBetweenThisAndPlayer <= 5f && coolDown == false) //Hit the player
        {
            //Set the target as default
            targetPositionCube.transform.position = defaultPosition;
            //Set the cooldown as on
            timer = 0;
            timerTarget = Random.Range(1, 6);
            coolDown = true;
            if (secondPhaseDodgeTimer <= 2.4)
            {
                //Need to shake player here
                cameraShakeScript = this.gameObject.AddComponent<BossCameraShake>();
                cameraShakeScript.shakeDuration = 0.3f;
                cameraShakeScript.shakeAmount = 0.2f;
                //Damage the player
                playerHealthScript.CalculateHitDirection(transform.position);
                playerHealthScript.ApplyDamage(20);
            }
            secondPhaseDodgeTimer = 0;
        }




        //If movement is allowed then run the movement code
        if (movementAllowed == true)
        {
            float step = movementSpeed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPositionCube.transform.position, step);
        }
    }

    private void Phase3() //Phase 1 movement but shorter breaks and more shooting
    {
        //If the health is less than 50
        if (bossReusableHealth.healthValue <= 50)
        {
            bossModelTurret.gameObject.SetActive(false);
            phase4Turret.gameObject.SetActive(true);
            //Increase the movement speed of the boss by 25%
            movementSpeed = movementSpeed * 2;
            //Replenish Ammo
            ReplenishAmmo();
            currentBossPhase = bossPhases.phase4;
            bossReusableHealth.healthValue = 130;
        }
        //Get the distance between the boss and the target
        float targetDistance = Vector3.Distance(transform.position, targetPositionCube.transform.position);
        //If the distance is less than 0.5 metres then find a new target
        if (targetDistance <= 0.5f)
        {
            //Set the direction cube position as the default position
            directionDecideCube.transform.position = defaultPosition;
            //Work out the direction the movement point will be in
            directionDecideCube.transform.rotation = Quaternion.Euler(transform.rotation.x, Random.Range(-90, 90), transform.rotation.z);
            //Set the target position cube
            targetPositionCube.transform.position = directionDecideCube.transform.position + directionDecideCube.transform.forward * movementRange;
        }
        //If movement is allowed then run the movement code
        if (movementAllowed == true)
        {
            float step = movementSpeed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPositionCube.transform.position, step);
        }
    }

    private void Phase4() //Men in black style driving around the tunnel
    {
        //Move to the default position and force it if needed
        if (transform.position != defaultPosition && finalPhaseReady == false && bossReusableHealth.ReturnInvincibleValue() == false)
        {
            //Set the tyre rotation to null
            if (directionDecideCube != null)
            {
                Destroy(directionDecideCube);
                frontWheels.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            //Force this object to the correct position
            if (Vector3.Distance(transform.position, defaultPosition) <= 1f)
            {
                //Force to default position
                transform.position = defaultPosition;
            }
            else //Move towards the default position
            {
                float step = movementSpeed * Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, defaultPosition, step);
            }
        }
        else if (finalPhaseReady == false && transform.position == defaultPosition && bossReusableHealth.ReturnInvincibleValue() == false)
        {
            //Set the target to move towards
            targetID = 0;
            //Mark the final phase as ready
            finalPhaseReady = true;
        }
        //If the boss is dead then make it invincible
        if (bossReusableHealth.healthValue <= 60)
        {
            bossReusableHealth.SetInvincibleValue(true);
            bossReusableHealth.healthValue = 999999;
            bossReusableHealth.enabled = false;
        }
        //Move to the selected target and drive all around the tunnel
        if (finalPhaseReady == true)
        {
            //If the target exists
            if (movementTargetArray[targetID] != null)
            {
                //Activate the rocket
                if (rocketParticle != null && rocketParticle.gameObject.activeSelf == false)
                {
                    rocketParticle.gameObject.SetActive(true);
                }
                //Move towards the target
                float step = movementSpeed * Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, movementTargetArray[targetID].transform.position, step);
                //If the boss has reached the target then rotate it slightly
                if (Vector3.Distance(transform.position, movementTargetArray[targetID].transform.position) < 0.5f)
                {
                    bossFlipTargetRotation = bossFlipTargetRotation + 10f;
                    targetRotation = Quaternion.Euler(transform.rotation.x, 180f, bossFlipTargetRotation);
                    //Next target
                    targetID++;
                    //if the target ID is bigger than the array, then go back to the beggining
                    if (targetID == movementTargetArray.Length)
                    {
                        targetID = 0;
                        bossFlipTargetRotation = 0;
                        targetRotation = targetRotation = Quaternion.Euler(transform.rotation.x, 180f, bossFlipTargetRotation);
                        transform.rotation = Quaternion.Euler(transform.rotation.x, 180f, 0);
                        finalPhaseReady = false;
                    }
                }
                //Only lerp when needed
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * movementSpeed);
            }
        }
        else if (finalPhaseReady == false && bossReusableHealth.ReturnInvincibleValue() == true) //Drive the boss to it's death
        {
            if (bossDead == false)
            {
                if (bossReusableHealth != null)
                {
                    bossReusableHealth.enabled = false;
                }
                //Mark the engine as broken
                if (engineParticle != null && engineParticle.gameObject.activeSelf == false)
                {
                    //Enable the engine particle effect
                    engineParticle.gameObject.SetActive(true);
                }
                //Move towards the target
                float step = movementSpeed * Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, movementTargetArray[targetID].transform.position, step);
                //If the boss has reached the target then rotate it slightly
                if (Vector3.Distance(transform.position, movementTargetArray[targetID].transform.position) < 0.5f)
                {
                    //Only go to a certain point
                    if (targetID < 14)
                    {
                        bossFlipTargetRotation = bossFlipTargetRotation + 10f;
                        //Next target
                        targetID++;
                    }
                    else
                    {
                        //Make all children convex too
                        foreach (Transform tran in truckRigidbody.transform)
                        {
                            if (tran.GetComponent<MeshCollider>() != null)
                            {
                                tran.GetComponent<MeshCollider>().convex = true;
                            }
                        }
                        truckRigidbody.gameObject.GetComponent<MeshCollider>().convex = true;
                        truckRigidbody.useGravity = true;
                        truckRigidbody.isKinematic = false;
                        bossDead = true;
                        truckRigidbody.AddForce(0, -100, 9999);
                    }
                    targetRotation = Quaternion.Euler(transform.rotation.x, 180f, bossFlipTargetRotation);
                }
                //Only lerp when needed
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * movementSpeed);
            }
            else
            {
                if (beganTransition == false)
                {
                    StartCoroutine(Explosion(0.8f));
                    StartCoroutine(BeginPlatformingTransition(2f));
                }
            }
        }
    }

    private IEnumerator Explosion(float time)
    {
        yield return new WaitForSeconds(time);
        bigExplosion.SetActive(true);
    }

    private IEnumerator BeginPlatformingTransition(float transitionTime) //Start the platforming transition
    {
        beganTransition = true;
        healthCanvas.enabled = false;
        yield return new WaitForSeconds(transitionTime);
        TransitionToPlatformingSection();
    }

    private void TransitionToPlatformingSection() //Transition to the on foot section
    {
        SceneManager.LoadScene("SpeedRun");
    }
}
