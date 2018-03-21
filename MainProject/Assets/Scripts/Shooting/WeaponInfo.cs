//Author: James Murphy
//Purpose to keep information about this gun
//Placement on the pickup of the gun

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WeaponInfo : MonoBehaviour
{
    private BoxCollider collider;
    private Shooting shootingScript;
    private AmmoManager ammoManagerScript;
    private int defaultAmountOfAmmo;
    [SerializeField]
    private bool ammoPickupOnly = false;

    public enum weapon
    {
        pistol,
        machineGun,
        shotgun
    }

    ;

    public GameObject weaponModelToSpawn;
    public weapon weaponModel;
    public float damage = 1f, fireDelay = 1f;
    [Range(0, 1)]
    public float recoilStrength = 1;
    [SerializeField]
    [Range(0, 200)]
    private int pickUpAmmo = 0;
    [HideInInspector]
    public bool holdDownToShoot = true;

    private void Awake()
    {
        //Get the shooting script
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        shootingScript = player.GetComponent<Shooting>();
        //Get the ammo manager
        ammoManagerScript = player.GetComponent<AmmoManager>();
        //Ignore raycasts
        transform.gameObject.layer = 23;
        //Make this object a trigger
        collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
        defaultAmountOfAmmo = pickUpAmmo;
        //If this gun is the pistol, then set hold down to shoot to false
        if (weaponModel == weapon.pistol)
        {
            holdDownToShoot = false;
        }
    }

    private void OnTriggerEnter(Collider otherobject) //When the player collides with this trigger, add the ammo and weapon to the player scripts
    {
        if (otherobject.tag == "Player")
        {
            if (shootingScript.currentWeaponScript == null)
            {
                //Give the shooting script this script
                shootingScript.currentWeaponScript = this;
            }

            //This variable will be used for storing the amount of ammo to add to the ammo manager
            int calculatedAmmoToAdd = 0;

            //Fill in any other variables as well
            switch (weaponModel)
            {
                case weapon.pistol:
                    if (shootingScript.pistolScript == null && ammoPickupOnly == false)
                    {
                        shootingScript.pistolScript = this;
                        shootingScript.weaponList.Add(this);
                    }
                    //Add the ammo picked up on this gun to the current amount of ammo
                    calculatedAmmoToAdd = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.pistol) + pickUpAmmo;
                    ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.pistol, calculatedAmmoToAdd);
                    break;
                case weapon.machineGun:
                    if (shootingScript.machineGunScript == null && ammoPickupOnly == false)
                    {
                        shootingScript.machineGunScript = this;
                        shootingScript.weaponList.Add(this);
                    }
                    //Only add ammo if you havent hit the ammo cap
                    if (ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun) < ammoManagerScript.ReturnMaxAmmoForMachineGun())
                    {
                        calculatedAmmoToAdd = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun) + pickUpAmmo;
                        ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.machineGun, calculatedAmmoToAdd);
                    }
                    break;
                case weapon.shotgun:
                    if (shootingScript.shotgunScript == null && ammoPickupOnly == false)
                    {
                        shootingScript.shotgunScript = this;
                        shootingScript.weaponList.Add(this);
                    }
                    //Only add ammo if you havent hit the ammo cap
                    if (ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun) < ammoManagerScript.ReturnMaxAmmoForShotgun())
                    {
                        calculatedAmmoToAdd = ammoManagerScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun) + pickUpAmmo;
                        ammoManagerScript.SetAmmoAmount(AmmoManager.ammoType.shotgun, calculatedAmmoToAdd);
                    }
                    break;
            }
            //Destroy this object as its no longer needed
            collider.enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public int ReturnStartingAmmo()
    {
        return defaultAmountOfAmmo;
    }
}
