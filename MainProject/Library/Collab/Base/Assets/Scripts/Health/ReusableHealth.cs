// Author: James Murphy
//Purpose: To keep track of the health values on the player and enemies

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class ReusableHealth : MonoBehaviour
{
    public int healthValue, armorValue = 0, maxHealth, maxArmor = 100;
    private List<Drone> droneScriptList = new List<Drone>();
    private bool bloodDecalsCreated = false, invincible = false;
    [SerializeField]
    [Header("Optional")]
    private BossHealthBar bossHealthUI;
    public bool glowWhenDamaged = false;
    public Material glowMaterialRegularHit, glowMaterialCritical, glowMaterialToUse;
    [SerializeField]
    private float glowTime = 0.5f;
    [SerializeField]
    private List<Material> defaultMaterials = new List<Material>();
    private MeshRenderer thisMesh;
    //Melee AI related
    BehaviorTree behaviorTree;
    public SharedInt behaviourHealth, currentHealth;
    public SharedBool behaviourPlayerDied;
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator meleeAnimator;
    [SerializeField] private GameObject healthBox;
    [SerializeField] private GameObject wepToDrop;
    [SerializeField] private GameObject bloods;
    [SerializeField] private ParticleSystem explos;
    private int decidingPoint;
    bool wepDropped = false, playerDied = false;
    private float glowCounter = 10, glowCounterTarget = 0;
    private bool glowRunOnce = false, returnToDefaultRunOnce = false;

    private void Start()
    {
        //Only run this code if allowed to glow when damaged
        if (glowWhenDamaged == true)
        {
            if (GetComponent<MeshRenderer>() != null)
            {
                thisMesh = GetComponent<MeshRenderer>();
            }
            int counter = 0;
            if (glowMaterialToUse == null)
            {
                glowMaterialToUse = glowMaterialRegularHit;
            }
            foreach (Material mat in thisMesh.materials)
            {
                Material newMaterial = mat;
                newMaterial.name = "material " + counter;
                defaultMaterials.Add(newMaterial);
                counter++;
            }
        }
        //Set bd health variable to health
        behaviourHealth = (SharedInt)GlobalVariables.Instance.GetVariable("health");
        behaviourHealth.Value = healthValue;

        //Set bd health variable to health
        behaviourPlayerDied = (SharedBool)GlobalVariables.Instance.GetVariable("playerDied");

        meleeAnimator = GetComponent<Animator>();
        healthBox = GameObject.Find("HealthPickup");

        //If this object is the player
        if (this.gameObject.tag == "Player")
        {

            //Remove all prior drone scripts
            droneScriptList.Clear();
            //Get all of the drones in the scene
            GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("enemy");

            foreach (GameObject enemy in foundEnemies)
            {
                if (enemy.GetComponent<Drone>() != null)
                {
                    droneScriptList.Add(enemy.GetComponent<Drone>());
                }
            }
        }
        explos = GameObject.Find("TurretMain").GetComponentInChildren<ParticleSystem>();
        explos.Stop();
    }

    public void CheckToSeeIfDead()
    {
        if (healthValue <= 0) //Check if the health value is below zero and if so, destroy the object
        {
            if (this.gameObject.tag == "Player") //If it is the player respawn at the latest checkpoint
            {
                //Fade in the game over screen
                InGameUI.inst.FadeInGameOver(1);
                //Stop movement etc
                GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(false);
                GetComponentInChildren<FirstPersonCamera>().IsCameraAllowedToMove(false);
                GetComponent<Crouch>().enabled = false;
                GetComponent<Crouch>().enabled = true;
                //Set behaviour tree reference to player died
                behaviourPlayerDied.Value = true;
                //Audio
                AudioManage.inst.death.Play();
                //Delay the actual death
                Invoke("DelayedPlayerDeathCode", 1f);
            }
            else //If this another object that isn't the player destroy it
            {
                if (this.gameObject == QuestReward.inst.RetriveTargetGO())
                {
                    QuestReward.inst.killVIPTrue();
                }
                //Destroy all bullet holes if there are any
                foreach (Transform currentTransform in transform)
                {
                    if (currentTransform.name == "decalFollowPointObj")
                    {
                        Destroy(currentTransform.gameObject);
                    }
                }

                //Destroy enemies to allow for animation unless they're drones
                if (this.gameObject.tag == "enemy" && this.gameObject.GetComponent<Drone>() == null)
                {
                    if (GetComponent<Collider>() != null)
                    {
                        GetComponent<Collider>().enabled = false;
                    }
                    //audio
                    AudioManage.inst.death.Play();
                    Invoke("DestroyGameobject", 2f);
                    if (this.gameObject.GetComponent<BehaviorTree>() != null && bloodDecalsCreated == false)
                    {
                        bloodDecalsCreated = true;
                        Invoke("SpawnBlood", 0.5f);
                    }
                }

                else
                {
                    Destroy(this.gameObject);
                }

            }
        }
    }

    private void DelayedPlayerDeathCode() //Player death code is delayed for the game over screen
    {
        //Reset the drones
        foreach (Drone droneScript in droneScriptList)
        {
            if (droneScript != null)
            {
                droneScript.ResetAI();
            }
        }
        //Stop all player movement
        Crouch crouchScript = GetComponent<Crouch>();
        crouchScript.StopSlide();
        crouchScript.StopAllCoroutines();
        GetComponent<CharacterControllerMovement>().StopAllMovement();
        GetComponent<Grapple>().ExitGrapple();

        //Set health back to default
        healthValue = maxHealth;
        armorValue = maxArmor;
        //Set AI died bool back to default
        behaviourPlayerDied.Value = false;
        //Cancel the ledge climb
        GetComponent<LedgeClimbV2>().CancelLedgeClimb();
        //Get weapon ammos back to default
        Shooting shootScript = GetComponent<Shooting>();
        AmmoManager ammoScript = GetComponent<AmmoManager>();
        //Reset all respective weapons to default ammos
        foreach (WeaponInfo weapon in shootScript.weaponList)
        {
            switch (weapon.weaponModel)
            {
                case WeaponInfo.weapon.pistol:
                    ammoScript.SetAmmoAmount(AmmoManager.ammoType.pistol, weapon.ReturnStartingAmmo());
                    break;
                case WeaponInfo.weapon.machineGun:
                    ammoScript.SetAmmoAmount(AmmoManager.ammoType.machineGun, weapon.ReturnStartingAmmo());
                    break;
            }
        }
        //Move the player back to the closest checkpoint
        if (CheckpointManager.singleton.GetCurrentCheckpoint() != null)
        {
            transform.position = CheckpointManager.singleton.GetCurrentCheckpoint().transform.position;
        }
        crouchScript.ManuallyChangeCrouchState(false);
        StartCoroutine(StandUp(crouchScript));
        //Add the Game Over Script
        InGameUI.inst.gameObject.AddComponent<GameOverScreen>();
    }

    void DestroyGameobject()
    {
        Destroy(this.gameObject);
    }

    //Manage particle effects for death
    void PlayExplosion()
    {
        if (this.gameObject.name == "TurretMain")
        {
            explos.Play();
        }
        else
        {
            return;
        }

//        if (this.gameObject.layer == LayerMask.NameToLayer("Drone"))
//        {
//            explos.Play();
//        }
//        else
//        {
//            return;
//        }
     
    }

    public void ApplyDamage(int damageValue) //This code will apply the damage value when called
    {
        if (invincible == false)
        {
            //This should make the object glow
            if (glowWhenDamaged == true)
            {
                glowCounterTarget = glowTime;
                glowCounter = 0;
                glowRunOnce = false;
                returnToDefaultRunOnce = false;
            }
            int bulletDamage = damageValue;

            //Apply damage to the boss ui
            if (transform.gameObject.name == "Truck" || transform.gameObject.name == "Phase1Turret")
            {
                if (bossHealthUI != null)
                {
                    bossHealthUI.ApplyBossDamage(bulletDamage);
                }
            }

            if (armorValue >= bulletDamage) //If there is enough armo, take half of the damage on the armor
            {
                //   InGameUI.inst.moveHitmarker(calculatedAngle);
                healthValue = healthValue - (bulletDamage / 2);
                armorValue = armorValue - (bulletDamage / 2);
            }
            else //If the player has no armor, take all of the damage onto the health value
            {
                //Remove the amount of damage from the health
                healthValue = healthValue - bulletDamage;
            }

            //If this hits a drone, then make the drone attack as soon as the player has shot it
            if (this.gameObject.tag == "enemy" && GetComponent<Drone>() != null)
            {
                GetComponent<Drone>().TriggerAttackState();
            }
            //If this hits a turret, then make the turret attack as soon as the player has shot it
            else if (this.gameObject.tag == "enemy" && GetComponent<TurretAI>() != null)
            {
                GetComponent<TurretAI>().TriggerAttackState();
            }

            //If the armor or health value goes below zero, correct it to zero
            if (healthValue < 0)
            {
                healthValue = 0;
            }
            if (armorValue < 0)
            {
                armorValue = 0;
            }

            if (this.gameObject.tag == "Player" && Time.timeScale != 0)
            {
                this.gameObject.AddComponent<BossCameraShake>().ShakeitShakeit(0.2f, 0.1f);
            }
            else if (Time.timeScale == 0)
            {
                this.gameObject.AddComponent<BossCameraShake>().ShakeitShakeit(0f, 0f);
            }

            //If this is the enemy objecgt
            if (GetComponent<ParticleSystem>() != null && this.gameObject.tag == "enemy" && ParticleEffectController.inst != null)
            {
                //call the method???
                ParticleEffectController.inst.EnemyParticle(GetComponent<ParticleSystem>());
            }

            //Check to see if the player is dead
            CheckToSeeIfDead();
        }


    }

    private IEnumerator StandUp(Crouch script)
    {
        yield return new WaitForSeconds(0.5f);
        script.ForceStandUp();
    }

    public void CheckToSeeIfMaxValuesReached() //Run this when a health / armor item has been picked up
    {
        if (healthValue > maxHealth)
        {
            healthValue = maxHealth;
        }
        if (armorValue > maxArmor)
        {
            armorValue = maxArmor;
        }
    }

    private void ControlDamageGlow()
    {
        if (glowWhenDamaged == true && glowMaterialRegularHit != null && glowMaterialCritical != null && glowMaterialToUse != null)
        {
            //Increment the glow counter
            glowCounter = glowCounter + Time.fixedDeltaTime;

            //Turn it red / yellow
            if (glowCounter <= glowCounterTarget)
            {
                //Turn the obj red
                if (glowRunOnce == false)
                {
                    glowRunOnce = true;
                    Material[] materialArrayToSet;
                    materialArrayToSet = new Material[thisMesh.materials.Length];
                    for (int i = 0; i < materialArrayToSet.Length; i++)
                    {
                        materialArrayToSet[i] = glowMaterialToUse;
                    }
                    thisMesh.materials = materialArrayToSet;
                }
            }
            else //Return to the default material
            {
                if (returnToDefaultRunOnce == false)
                {
                    returnToDefaultRunOnce = true;
                    Material[] materialArrayToSet;
                    materialArrayToSet = new Material[thisMesh.materials.Length];
                    for (int i = 0; i < materialArrayToSet.Length; i++)
                    {
                        materialArrayToSet[i] = defaultMaterials[i];
                    }
                    thisMesh.materials = materialArrayToSet;
                }
            }
        }
    }

    private void Update() //Reset player on delete (For debugging)
    {
        StartCoroutine(CheckAIDeath());

        //if (this.gameObject.tag == "Player")
        //{
        //    if (Input.GetKeyUp(KeyCode.Clear))
        //    {
        //        healthValue = 0;
        //        CheckToSeeIfDead();
        //    }
        //}

        if (this.gameObject.tag == "enemy")
        {
            if (Input.GetKeyUp(KeyCode.Delete))
            {
                healthValue = 0;

            }
        }

    }

    private void FixedUpdate()
    {
        ControlDamageGlow();
    }

    public void CalculateHitDirection(Vector3 hitPoint)
    {
        if (this.gameObject.tag == "Player")
        {
            Vector3 displacement = transform.position - hitPoint;
            float forwardAngle = Vector3.Angle(displacement, -transform.forward);
            float rightAngle = Vector3.Angle(displacement, transform.right);

            float shootAngle = 0;

            if (rightAngle >= 90) // Shot come from left side
            {
                forwardAngle = 360 - forwardAngle;
            }

            shootAngle = forwardAngle;
            //print("Angle between forward: " + forwardAngle + ". Angle between right: " + rightAngle + ". shootAngle: " + shootAngle);

            //Calculate the angle in relation to where the player is facing for the hit point
            float calculatedAngle = CalculateAngle(transform.forward, hitPoint - transform.position);
            calculatedAngle = Mathf.Round(shootAngle);
            //Show the hit point at the correct angle
            InGameUI.inst.moveHitmarker(calculatedAngle);
        }

    }

    public float CalculateAngle(Vector3 from, Vector3 to) //This method calculates the angle using Quaternions
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    public void SetInvincibleValue(bool value)
    {
        invincible = value;
    }

    IEnumerator CheckAIDeath()
    {
        behaviourHealth.Value = healthValue;
        if (this.gameObject.GetComponent<ReusableHealth>().healthValue <= 0)
        {
            Invoke("PlayExplosion", 1.65f);
            if (this.gameObject.tag == "enemy")
            {
                behaviourHealth.Value = healthValue;
             
                if (wepDropped == false)
                {
                    wepDropped = true;
                    Invoke("DropWeapon", 0.5f);
                }

            }

        }
        else
        {
            yield return null;
        }


        if (meleeAnimator && agent != null)
        {
            meleeAnimator.enabled = false;
            agent.enabled = false;
        }


    }

    //Spawn dropped weapon
    private void DropWeapon() //Author: Kate Georgiou - handles the random n umber generation so it knows what to spawn on each enemy who dies
    {
        int lowerRandomRange = 0, higherRandomRange = 25;

        int randoNumber = Random.Range(lowerRandomRange, higherRandomRange);
        GameObject spawnedObject;
        if (randoNumber <= decidingPoint && randoNumber >= lowerRandomRange) //if the random number ranges between 0 and 25 then drop the health kits...
        {
            if (healthBox != null)
            {
                spawnedObject = Instantiate(healthBox, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity) as GameObject;
                spawnedObject.name = "Dropped Item"; //name the object that is dropped this name
            }
        }
        else if (randoNumber >= decidingPoint && randoNumber <= higherRandomRange) //if the random number is more than 25 but less than 100 spawn a weapon...
        {
            if (wepToDrop != null)
            {
                //Check if null for turret tag
                spawnedObject = Instantiate(wepToDrop, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity) as GameObject;
                spawnedObject.name = "Dropped Item"; //name the object that is dropped this name
            }

        }

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

    public bool ReturnInvincibleValue() //Return if the object is invincible
    {
        return invincible;
    }

}
