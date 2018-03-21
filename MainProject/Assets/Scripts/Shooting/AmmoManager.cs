//Author: James Murphy
//Purpose: Track ammo on the player

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public enum ammoType
    {
        pistol,
        machineGun,
        shotgun

    }

    [SerializeField]
    [Header("Current Ammo Values")]
    private int pistolAmmo = 0;
    [SerializeField]
    private int machineGunAmmo = 0, shotgunAmmo = 0;
    [SerializeField]
    [Header("Max Ammo Values")]
    private int mgMaxAmmo = 40;
    [SerializeField]
    private int shotgunMaxAmmo = 12;

    public void SetAmmoAmount(ammoType weaponType, int amountOfAmmoToSet) //Set the desired amount of ammo
    {
        switch (weaponType)
        {
            case ammoType.pistol:
                pistolAmmo = amountOfAmmoToSet;
                break;

            case ammoType.machineGun:
                machineGunAmmo = amountOfAmmoToSet;
                break;

            case ammoType.shotgun:
                shotgunAmmo = amountOfAmmoToSet;
                break;
        }
    }

    private void FixedUpdate() //Cap the current ammo if it is over the max ammo
    {
        if (machineGunAmmo > mgMaxAmmo)
        {
            machineGunAmmo = mgMaxAmmo;
        }
        if (shotgunAmmo > shotgunMaxAmmo)
        {
            shotgunAmmo = shotgunMaxAmmo;
        }
    }

    public int ReturnAmountOfAmmoForWeapon(ammoType weaponType) //Will return the amount of ammo on the selected weapon
    {
        switch (weaponType)
        {
            case ammoType.pistol:
                return pistolAmmo;

            case ammoType.machineGun:
                return machineGunAmmo;

            case ammoType.shotgun:
                return shotgunAmmo;
        }
        return 0;
    }

    public int ReturnMaxAmmoForMachineGun()
    {
        return mgMaxAmmo;
    }

    public int ReturnMaxAmmoForShotgun()
    {
        return shotgunMaxAmmo;
    }
}
