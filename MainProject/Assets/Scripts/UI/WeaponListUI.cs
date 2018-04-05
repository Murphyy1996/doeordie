//Author: James Murphy
//Purpose: Update the player about ammo and selected weapon etc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponListUI : MonoBehaviour
{
    private Image slot1Image, slot2Image, slot3Image, slot4Image;
    [SerializeField]
    [Header("Sprite Resources")]
    private Sprite pistolSpite;
    [SerializeField]
    private Sprite dualPistolSprite, machinegunSprite, shotgunSprite;
    private GameObject playerObj;
    private AmmoManager ammoScript;
    private Shooting shootingScript;

    private void Start() //Get required components
    {
        slot1Image = transform.Find("slot1").GetComponent<Image>();
        slot2Image = transform.Find("slot2").GetComponent<Image>();
        slot3Image = transform.Find("slot3").GetComponent<Image>();
        slot4Image = transform.Find("slot4").GetComponent<Image>();
        playerObj = GameObject.Find("Player");
        ammoScript = playerObj.GetComponent<AmmoManager>();
        shootingScript = playerObj.GetComponent<Shooting>();

        int gunsPickedUp = shootingScript.weaponList.Count;
        EnableAmountOfSlots(gunsPickedUp);
    }

    private void FixedUpdate() //Depending on the amount of weapons keep the list updated
    {
        int gunsPickedUp = shootingScript.weaponList.Count;
        EnableAmountOfSlots(gunsPickedUp);
    }

    private void DisableAllSlots() //Disable all slots
    {
        slot1Image.enabled = false;
        slot2Image.enabled = false;
        slot3Image.enabled = false;
        slot4Image.enabled = false;
    }

    private void AssignSpritesToSlot() //Assign the required sprites to a slot
    {
        for (int i = 0; i < shootingScript.weaponList.Count; i++)
        {
            if (i == 0) //Slot 1
            {
               switch (shootingScript.weaponList[i].weaponModel)
                {
                    case WeaponInfo.weapon.pistol:
                        slot1Image.sprite = pistolSpite;
                        break;
                    case WeaponInfo.weapon.dualPistol:
                        slot1Image.sprite = dualPistolSprite;
                        break;
                    case WeaponInfo.weapon.shotgun:
                        slot1Image.sprite = shotgunSprite;
                        break;
                    case WeaponInfo.weapon.machineGun:
                        slot1Image.sprite = machinegunSprite;
                        break;
                }
            }
            else if (i == 1) //Slot 2
            {
                switch (shootingScript.weaponList[i].weaponModel)
                {
                    case WeaponInfo.weapon.pistol:
                        slot2Image.sprite = pistolSpite;
                        break;
                    case WeaponInfo.weapon.dualPistol:
                        slot2Image.sprite = dualPistolSprite;
                        break;
                    case WeaponInfo.weapon.shotgun:
                        slot2Image.sprite = shotgunSprite;
                        break;
                    case WeaponInfo.weapon.machineGun:
                        slot2Image.sprite = machinegunSprite;
                        break;
                }
            }
            if (i == 2) //Slot 3
            {
                switch (shootingScript.weaponList[i].weaponModel)
                {
                    case WeaponInfo.weapon.pistol:
                        slot3Image.sprite = pistolSpite;
                        break;
                    case WeaponInfo.weapon.dualPistol:
                        slot3Image.sprite = dualPistolSprite;
                        break;
                    case WeaponInfo.weapon.shotgun:
                        slot3Image.sprite = shotgunSprite;
                        break;
                    case WeaponInfo.weapon.machineGun:
                        slot3Image.sprite = machinegunSprite;
                        break;
                }
            }
            if (i == 3) //Slot 4
            {
                switch (shootingScript.weaponList[i].weaponModel)
                {
                    case WeaponInfo.weapon.pistol:
                        slot4Image.sprite = pistolSpite;
                        break;
                    case WeaponInfo.weapon.dualPistol:
                        slot4Image.sprite = dualPistolSprite;
                        break;
                    case WeaponInfo.weapon.shotgun:
                        slot4Image.sprite = shotgunSprite;
                        break;
                    case WeaponInfo.weapon.machineGun:
                        slot4Image.sprite = machinegunSprite;
                        break;
                }
            }
        }
    }

    private void EnableAmountOfSlots(int amountOfWeapons)
    {
        AssignSpritesToSlot();
        DisableAllSlots();
        if (amountOfWeapons == 1)
        {
            slot1Image.enabled = true;
        }
        else if (amountOfWeapons == 2)
        {
            slot1Image.enabled = true;
            slot2Image.enabled = true;
        }
        else if (amountOfWeapons == 3)
        {
            slot1Image.enabled = true;
            slot2Image.enabled = true;
            slot3Image.enabled = true;
        }
        else if (amountOfWeapons == 4)
        {
            slot1Image.enabled = true;
            slot2Image.enabled = true;
            slot3Image.enabled = true;
            slot4Image.enabled = true;
        }
    } //Enable the required amount of slots
}
