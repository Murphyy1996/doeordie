//Author: James Murphy && Kate Georgiou (Head bob code)
//Placement: On the player
//Purpose: Make the guns shoot

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    private AmmoManager ammoManagerScript;
    private Crouch crouchScript;
    [HideInInspector]
    public bool allowedToShoot = true;
    [SerializeField]
    private GameObject bulletPrefab, bulletHitDecalPrefab;
    private List<GameObject> bulletObjPool = new List<GameObject>();
    private List<GameObject> bulletHitObjPool = new List<GameObject>();
    private Transform gunPlacementZone;
    [SerializeField]
    private float switchWeaponCooldown = 1f;
    public WeaponInfo currentWeaponScript;
    [HideInInspector]
    public List<WeaponInfo> weaponList = new List<WeaponInfo>();
    private float currentWeaponDamage = 0, currentWeaponFireDelay = 0;
    private float currentWeaponRecoil = 0;
    [HideInInspector]
    public WeaponInfo pistolScript, dualPistolscript, machineGunScript, shotgunScript;
    private CharacterController thisCC;
    private CharacterControllerMovement movement;
    //Need to destroy this variable if the weapon has been changed
    private GameObject currentWeaponObj;
    private float selectedWeaponFloat = 0;
    private bool gunCooldownFinished = true, weaponChangeCooldownFinished = true, canSwitchWeapons = true, isHeadBobEnabled = true;
    // animate the game object from -1 to +1 and back
    [SerializeField]
    private float minimumBob = -3.0F, maximumBob = 3.0F;
    private GameObject bulletOrigin, muzzleFlash;
    // starting value for the Lerp
    static float headBobSpeed = 0.0f;
    private bool isShooting = false;
    private Animator animatorComponent;
    private KeyCode changeWeaponKeycode = KeyCode.Q;
    private Teleporting teleportScript;
    private int outOfAmmoCount = 0;
    private Text outOfAmmoPrompt;
    private Grapple grappleScript;
    private Canvas pauseMenu, optionsMenu;

    private float pickUpAmmoTimer = 999, pickUpAmmoLimit = 2;

    private void Awake() //Spawn in bullets for the object pool
    {
        //Get the crouch script
        crouchScript = GetComponent<Crouch>();
        grappleScript = GetComponent<Grapple>();
        //Create the object pool parent
        GameObject objectPool = new GameObject();
        objectPool.name = "Object Pools";
        objectPool.transform.position = transform.position;
        //Get the ammo manager script
        ammoManagerScript = GetComponent<AmmoManager>();
        //Spawn objects into the object pool
        for (int i = 0; i < 500; i++)
        {
            //Spawn the bullet, child it to the object pool and track it in the list
            GameObject spawnedBullet;
            spawnedBullet = Instantiate(bulletPrefab, objectPool.transform.position, bulletPrefab.transform.rotation);
            spawnedBullet.name = "Spawned Bullet " + i;
            spawnedBullet.transform.SetParent(objectPool.transform);
            bulletObjPool.Add(spawnedBullet);
        }
        //Spawn decals into the object pool
        for (int i = 0; i < 500; i++)
        {
            //Spawn the bullet, child it to the object pool and track it in the list
            GameObject spawnedBulletDecal;
            spawnedBulletDecal = Instantiate(bulletHitDecalPrefab, objectPool.transform.position, bulletPrefab.transform.rotation);
            spawnedBulletDecal.name = "Spawned Decal " + i;
            spawnedBulletDecal.transform.SetParent(objectPool.transform);
            bulletHitObjPool.Add(spawnedBulletDecal);
        }
        Invoke("DelayedAwake", 0.2f);
    }

    private void DelayedAwake() //Get the out of ammo prompt
    {
        outOfAmmoPrompt = GameObject.Find("OutOfAmmoPrompt").GetComponent<Text>();
        pauseMenu = GameObject.Find("Pause menu").GetComponent<Canvas>();
        optionsMenu = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
    }

    private void Start()
    {
        //Get this character controller
        thisCC = GetComponent<CharacterController>();
        //Get movement script
        movement = GetComponent<CharacterControllerMovement>();
        teleportScript = GetComponent<Teleporting>();
    }

    public void GiveGunPlacementZone(Transform placementZone)
    {
        gunPlacementZone = placementZone;
    }

    private void Update()
    {
        //Control some non shooting animations
        ControlNonShootingGunAnimations();
        //This code will switch weapons with middle mouse
        SwitchWeapons(false);
        //This code will render the first picked up weapon
        RenderWeapon();
        if (teleportScript.ReturnTeleportEmpty() == null || teleportScript.ReturnTeleportEmpty().activeSelf == false)
        {
            //This will run the shoot weapon code
            ShootWeapon();
        }
        if (isHeadBobEnabled == true)
        {
            //Gun bob code
            GunBob();
        }
    }

    private void FixedUpdate()
    {
        pickUpAmmoTimer = pickUpAmmoTimer + Time.fixedDeltaTime;
        if (pickUpAmmoTimer <= pickUpAmmoLimit)
        {

            //Show the pick up ammo thing here
            InGameUI.inst.pickedAmmo.enabled = true;
        }
        else
        {

            //Hide the pick up ammo thing here
            InGameUI.inst.pickedAmmo.enabled = false;
        }
    }

    public void ShowPickUpAmmoLabel() //Will activate the time code
    {
        pickUpAmmoTimer = 0;
        AudioManage.inst.pickUp.Play();
    }

    private void RenderWeapon() //Render the selected weapon
    {
        //if there is no weapon rendered but one has been picked up, render it
        if (currentWeaponScript != null && weaponChangeCooldownFinished == true && gunPlacementZone != null)
        {
            //Create the weapon
            if (currentWeaponObj == null)
            {
                //Rotate and spawn the weapon
                gunPlacementZone.transform.rotation = Camera.main.transform.rotation;
                currentWeaponObj = Instantiate(currentWeaponScript.weaponModelToSpawn, gunPlacementZone.transform.position, gunPlacementZone.transform.rotation);
                //Set the gun to the gun layer if it is not already
                if (currentWeaponObj.layer != 17)
                {
                    currentWeaponObj.layer = 17;
                    foreach (Transform child in currentWeaponObj.transform)
                    {
                        child.gameObject.layer = 17;
                    }
                }
                if (currentWeaponObj.GetComponentInChildren<Animator>() != null)
                {
                    animatorComponent = currentWeaponObj.GetComponentInChildren<Animator>();
                }
                else
                {
                    animatorComponent = null;
                }
                //Find where the bullet origin is
                bulletOrigin = currentWeaponObj.transform.Find("bulletOrigin").gameObject;
                try
                {
                    muzzleFlash = currentWeaponObj.transform.Find("Muzzle").gameObject;
                    muzzleFlash.transform.SetParent(animatorComponent.transform);
                    //Make sure all parts of the muzzle have the correct layer too
                    foreach (Transform child in muzzleFlash.transform)
                    {
                        child.gameObject.layer = 17;
                    }
                }
                catch
                {
                    print("can't get muzzle flash");
                }
                currentWeaponDamage = currentWeaponScript.damage;
                currentWeaponFireDelay = currentWeaponScript.fireDelay;
                currentWeaponRecoil = currentWeaponScript.recoilStrength;
            }
            //Make the gun object follow the weapon object
            currentWeaponObj.transform.SetParent(gunPlacementZone);
        }
    }

    private void SwitchWeapons(bool calledManually) //Switch weapons with middle mouse
    {
        //Only allow changing between weapons if you have more than one weapon
        if (weaponList.Count >= 2 && canSwitchWeapons == true)
        {
            //float inputValue = Input.GetAxis("Mouse ScrollWheel");
            //if (inputValue > 0)
            //{
            //inputValue = 1;
            //}
            //if (inputValue < 0)
            //{
            //inputValue = -1;
            //}
            float inputValue = 0;
            if (calledManually == true)
            {
                inputValue = 1;
            }
            if (Input.GetKeyDown(changeWeaponKeycode))
            {
                inputValue = 1;
            }
            //Only run this code if the input has been changed
            if (inputValue != 0)
            {
                //Add the input value to the selected weapon number
                selectedWeaponFloat += inputValue;
                //If the selected weapon number is bigger than the max amount of weapons
                if (selectedWeaponFloat > weaponList.Count - 1)
                {
                    selectedWeaponFloat = 0;
                }
                if (selectedWeaponFloat < 0)
                {
                    selectedWeaponFloat = weaponList.Count - 1;
                }
                //Convert the selected weapon number to an int
                int selectedWeaponInt = Mathf.RoundToInt(selectedWeaponFloat);
                //Select the current weapon
                currentWeaponScript = weaponList[selectedWeaponInt];
                DeleteRenderedWeapon();
                //If the weapon has been changed activate the cooldown
                StopCoroutine(WeaponCooldown());
                StopCoroutine(StopShotgunAnim(0.001f));
                gunCooldownFinished = true;
                StartCoroutine(SwitchWeaponCooldown());
                canSwitchWeapons = false;
                Invoke("FixInstantSwitch", 0.2f);
            }
            //Allow for switching via numbers
            //Options for selecting with numbers
            int selectedWeap = 100;
            if (Input.GetKey(KeyCode.Alpha1) && currentWeaponScript != weaponList[0])
            {
                selectedWeap = 0;
                selectedWeaponFloat = selectedWeap;
            }
            if (Input.GetKey(KeyCode.Alpha2) && currentWeaponScript != weaponList[1])
            {
                selectedWeap = 1;
                selectedWeaponFloat = selectedWeap;
            }
            if (Input.GetKey(KeyCode.Alpha3) && 2 <= weaponList.Count - 1 && currentWeaponScript != weaponList[2])
            {
                selectedWeap = 2;
                selectedWeaponFloat = selectedWeap;
            }
            if (Input.GetKey(KeyCode.Alpha4) && 3 <= weaponList.Count - 1 && currentWeaponScript != weaponList[3])
            {
                selectedWeap = 3;
                selectedWeaponFloat = selectedWeap;
            }
            if (selectedWeap != 100)
            {
                //Select the current weapon
                currentWeaponScript = weaponList[selectedWeap];
                DeleteRenderedWeapon();
                //If the weapon has been changed activate the cooldown
                StopCoroutine(WeaponCooldown());
                gunCooldownFinished = true;
                StartCoroutine(SwitchWeaponCooldown());
                canSwitchWeapons = false;
                Invoke("FixInstantSwitch", 0.2f);
            }
        }
    }

    private void ShootWeapon() //Shoot the currently selected weapon
    {
        if (pauseMenu != null && optionsMenu != null)
        {
            //Do not allow if there is no currently selected weapon
            if (currentWeaponScript != null && currentWeaponObj != null && allowedToShoot == true && CheckIfYouHaveEnoughAmmoToShoot() == true && gunCooldownFinished == true && weaponChangeCooldownFinished == true && pauseMenu.enabled == false && optionsMenu.enabled == false)
            {
                //This if will shoot the bullet and remove the relevant ammo
                if (Input.GetMouseButton(0) && Time.timeScale != 0 && currentWeaponScript.holdDownToShoot == true || Input.GetMouseButtonDown(0) && Time.timeScale != 0 && currentWeaponScript.holdDownToShoot == false)
                {
                    //Stop any previous shot animations
                    if (currentWeaponScript.weaponModel == WeaponInfo.weapon.shotgun)
                    {
                        StopShotgunAnimation();
                    }
                    StopCoroutine(StopShotgunAnim(0.001f));
                    //Change the has shot recently bool to true
                    isShooting = true;

                    //Start the weapon cooldown until the next bullet can be fired
                    StartCoroutine(WeaponCooldown());
                    //Try and make stuff work for the machine gun
                    if (animatorComponent != null)
                    {
                        animatorComponent.SetBool("shooting", true);
                        if (currentWeaponScript.weaponModel == WeaponInfo.weapon.shotgun)
                        {
                            StartCoroutine(StopShotgunAnim(0.2f));
                            //animatorComponent.SetBool("shooting", false); ;
                        }
                    }

                    //Only remove ammo if not the pistol
                    if (currentWeaponScript.holdDownToShoot == true)
                    {
                        //Remove the relevant ammo
                        RemoveWeaponAmmo();
                    }
                    //Get the bullet reference in the object pool
                    GameObject bullet = bulletObjPool[0];
                    //Get the bullet script
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    //Set the bullet origin as camera as the player camera transform
                    bulletScript.bulletOrigin = Camera.main.transform;
                    //Set the bullet recoil
                    bulletScript.recoil = currentWeaponRecoil;
                    //Set the bullet damage
                    bulletScript.bulletDamage = Mathf.RoundToInt(currentWeaponDamage);
                    //Mark the bullet as a player bullet
                    bulletScript.playerBullet = true;
                    //Remove it from the object pool
                    bulletObjPool.Remove(bullet);

                    //Muzzle flash
                    if (muzzleFlash != null)
                    {
                        //Increase reticule size
                        Reticule.inst.IncreaseReticuleSize(2f);
                        //Activate the muzzle flash
                        muzzleFlash.SetActive(true);
                        try
                        {
                            switch (currentWeaponScript.weaponModel)
                            {
                                case WeaponInfo.weapon.pistol:
                                    AudioManage.inst.pistolShot.Play();
                                    break;


                                case WeaponInfo.weapon.machineGun:
                                    AudioManage.inst.machShot.Play();
                                    break;

                                case WeaponInfo.weapon.shotgun:
                                    AudioManage.inst.shotgun.Play();
                                    break;
                            }
                        }
                        catch
                        {
                            print("Error playing weapon audio");
                        }
                        //audio for gunshot here
                        // need to make this a switch case so that it can change depending on the gun being used


                        Invoke("CloseMuzzleFlash", 0.05f);
                    }
                    //Move the object to the player
                    bullet.transform.SetPositionAndRotation(bulletOrigin.transform.position, gunPlacementZone.transform.rotation);
                    //Activate the bullet once it has been configured
                    bullet.SetActive(true);


                    //Loop shot if shotgun
                    if (currentWeaponScript.weaponModel == WeaponInfo.weapon.shotgun)
                    {
                        //Make the camera shake
                        this.gameObject.AddComponent<BossCameraShake>().ShakeitShakeit(0.1f, 1.5f);
                        //Get the default recoil values
                        int lastUseWeaponDamage = Mathf.RoundToInt(currentWeaponDamage);
                        float lastUsedWeaponRecoil = currentWeaponRecoil;
                        //Get the additional origins
                        List<Transform> additionalOrigins = new List<Transform>();
                        foreach (Transform child in bulletOrigin.transform)
                        {
                            additionalOrigins.Add(child);
                        }
                        //Set up the bullets before being shot as doing it while being shot causes glitches
                        int count = 0;
                        foreach (GameObject obj in bulletObjPool)
                        {
                            if (count <= additionalOrigins.Count)
                            {
                                Bullet extraBulletScript = obj.GetComponent<Bullet>();
                                extraBulletScript.bulletDamage = lastUseWeaponDamage;
                                extraBulletScript.recoil = lastUsedWeaponRecoil;
                                extraBulletScript.bulletOrigin = Camera.main.transform;
                                extraBulletScript.playerBullet = true;
                            }
                        }
                        //Iterate through the additional origins and activate the bullets
                        for (int i = 0; i < additionalOrigins.Count; i++)
                        {
                            //Get the bullet reference in the object pool
                            GameObject extraBullets = bulletObjPool[0];
                            //Remove it from the object pool
                            bulletObjPool.Remove(extraBullets);
                            //Move the object to the player
                            extraBullets.transform.SetPositionAndRotation(additionalOrigins[i].position, gunPlacementZone.transform.rotation);
                            //Activate the bullet once it has been configured
                            extraBullets.SetActive(true);
                        }
                        switch (currentWeaponScript.weaponModel)
                        {
                            //Force stop the shotgun anim
                            case WeaponInfo.weapon.shotgun:
                                //StartCoroutine(StopShotgunAnim(0.1f));
                                break;
                        }
                        if (animatorComponent != null)
                        {
                            //Invoke("StopShotgunAnimation", 0.2f);
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0) && CheckIfYouHaveEnoughAmmoToShoot() == false)
            {
                isShooting = false;
                if (outOfAmmoCount == 1)
                {
                    SwitchWeapons(true);
                }
                else
                {
                    outOfAmmoCount++;
                }
            }
            else if (!Input.GetMouseButton(0))
            {
                isShooting = false;
            }
        }

    }

    private void StopShotgunAnimation()
    {
        animatorComponent.SetBool("shooting", false);
    }

    private void ControlNonShootingGunAnimations() //This will control non shooting animations such as running
    {
        if (animatorComponent != null)
        {
            if (grappleScript.IsCurrentlyGrappling() == false && currentWeaponScript.weaponModel != WeaponInfo.weapon.shotgun)
            {
                if (thisCC.velocity.magnitude >= 18)
                {
                    animatorComponent.SetBool("running", true);
                }
                else
                {
                    animatorComponent.SetBool("running", false);
                }
            }
            else
            {
                animatorComponent.SetBool("running", false);
            }
        }
    }

    private void CloseMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
        Reticule.inst.ResetReticuleSize();
    }

    private void FixInstantSwitch() //Will limit the player to switching weapons every 0.1 seconds
    {
        canSwitchWeapons = true;
    }

    private void RemoveWeaponAmmo() //This method will remove ammo on weapons when shot
    {
        //This variable will hold the current amount of ammo you have
        int currentWeaponAmmo = 0;

        //Remove ammo on the relevant weapon
        if (currentWeaponScript == pistolScript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.pistol);
            ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.pistol, currentWeaponAmmo - 1);
        }
        else if (currentWeaponScript == dualPistolscript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.pistol);
            ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.pistol, currentWeaponAmmo - 1);
        }
        else if (currentWeaponScript == machineGunScript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
            ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.machineGun, currentWeaponAmmo - 1);
        }
        else if (currentWeaponScript == shotgunScript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun);
            ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.shotgun, currentWeaponAmmo - 1);
        }
    }

    private IEnumerator StopShotgunAnim(float time)
    {
        if (currentWeaponScript.weaponModel == WeaponInfo.weapon.shotgun)
        {
            yield return new WaitForSeconds(time);
            animatorComponent.SetBool("shooting", false);
        }
    }

    private IEnumerator WeaponCooldown() //This controls the weapon fire rate
    {
        gunCooldownFinished = false;
        yield return new WaitForSeconds(currentWeaponFireDelay);
        if (animatorComponent != null && currentWeaponScript.weaponModel != WeaponInfo.weapon.shotgun)
        {
            animatorComponent.SetBool("shooting", false);
        }
        gunCooldownFinished = true;
    }

    private IEnumerator SwitchWeaponCooldown() //Apply the switch weapon cooldown to stop super shooting rates when switching weapons
    {
        weaponChangeCooldownFinished = false;
        yield return new WaitForSeconds(switchWeaponCooldown);
        weaponChangeCooldownFinished = true;
    }

    private bool CheckIfYouHaveEnoughAmmoToShoot() //This function will return true or false depending if you have enough ammo
    {
        //This variable will hold the current amount of ammo you have
        int currentWeaponAmmo = 0;

        //Get the ammount of the current weapon
        if (currentWeaponScript == pistolScript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.pistol);
        }
        else if (currentWeaponScript == dualPistolscript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.pistol);
        }
        else if (currentWeaponScript == machineGunScript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
        }
        else if (currentWeaponScript == shotgunScript)
        {
            currentWeaponAmmo = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun);
        }

        //Check if the current weapon has enough ammo to shoot
        if (currentWeaponAmmo > 0)
        {
            outOfAmmoCount = 0;
            if (outOfAmmoPrompt != null)
            {
                outOfAmmoPrompt.enabled = false;
            }
            return true;
        }
        //Return false if you can't shoot
        if (outOfAmmoPrompt != null)
        {
            outOfAmmoPrompt.enabled = true;
        }
        return false;
    }

    public void DeleteRenderedWeapon()
    {
        if (currentWeaponObj != null)
        {
            Destroy(currentWeaponObj);
        }
    }

    public void SetShootAllowedValue(bool value)
    {
        allowedToShoot = value;
    }

    public List<GameObject> ReturnBulletObjectPool()
    {
        return bulletObjPool;
    }

    public List<GameObject> ReturnDecalObjectPool()
    {
        return bulletHitObjPool;
    }

    private void GunBob()
    {
        if (movement != null && gunPlacementZone != null && crouchScript != null && movement.CheckIfWalking() == true && crouchScript.IsPlayerSliding() == false)
        {
            // animate the position of the game object...
            gunPlacementZone.transform.localEulerAngles = new Vector3(Mathf.Lerp(minimumBob, maximumBob, headBobSpeed), 0, 0);

            // .. and increate the t interpolater
            headBobSpeed += 1.5f * Time.deltaTime;

            // now check if the interpolator has reached 1.0
            // and swap maximum and minimum so game object moves
            // in the opposite direction.
            if (headBobSpeed > 1.0f)
            {
                float temp = maximumBob;
                maximumBob = minimumBob;
                minimumBob = temp;
                headBobSpeed = 0.0f;

            }
        }

    }

    public bool IsPlayerShooting()
    {
        return isShooting;
    }

    public KeyCode ReturnWeaponKeycode()
    {
        return changeWeaponKeycode;
    }

    public void SetWeaponKeycode(KeyCode value)
    {
        changeWeaponKeycode = value;
    }


}
