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
    private Outline slot1Outline, slot2Outline, slot3Outline, slot4Outline;
    private Text slot1Ammo, slot2Ammo, slot3Ammo, slot4Ammo;
    private Vector3 defaultTextSize;
    private float defaultXTextPos;

    private void Start() //Get required components
    {
        slot1Image = transform.Find("slot1").GetComponent<Image>();
        slot2Image = transform.Find("slot2").GetComponent<Image>();
        slot3Image = transform.Find("slot3").GetComponent<Image>();
        slot4Image = transform.Find("slot4").GetComponent<Image>();
        slot1Outline = slot1Image.gameObject.GetComponent<Outline>();
        slot2Outline = slot2Image.gameObject.GetComponent<Outline>();
        slot3Outline = slot3Image.gameObject.GetComponent<Outline>();
        slot4Outline = slot4Image.gameObject.GetComponent<Outline>();
        slot1Ammo = slot1Image.gameObject.GetComponentInChildren<Text>();
        slot2Ammo = slot2Image.gameObject.GetComponentInChildren<Text>();
        slot3Ammo = slot3Image.gameObject.GetComponentInChildren<Text>();
        slot4Ammo = slot4Image.gameObject.GetComponentInChildren<Text>();
        defaultTextSize = slot2Ammo.gameObject.transform.localScale;
        defaultXTextPos = slot2Ammo.rectTransform.position.x;
        playerObj = GameObject.Find("Player");
        ammoScript = playerObj.GetComponent<AmmoManager>();
        shootingScript = playerObj.GetComponent<Shooting>();

        int gunsPickedUp = shootingScript.weaponList.Count;
        EnableAmountOfSlots(gunsPickedUp);
    }

    private void FixedUpdate() //Depending on the amount of weapons keep the list updated
    {
        UpdateCurrentlySelectedWeapon();
        UpdateAmmoCounters();
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

    private void UpdateAmmoCounters() //Update the ammo counters
    {
        //Force ammo counter sizes
        slot1Ammo.transform.localScale = defaultTextSize;
        slot2Ammo.transform.localScale = defaultTextSize;
        slot3Ammo.transform.localScale = defaultTextSize;
        slot4Ammo.transform.localScale = defaultTextSize;
        //Force ammo counter positions
        slot1Ammo.rectTransform.position = new Vector3(defaultXTextPos, slot1Ammo.rectTransform.position.y, slot1Ammo.rectTransform.position.z);
        slot2Ammo.rectTransform.position = new Vector3(defaultXTextPos, slot2Ammo.rectTransform.position.y, slot2Ammo.rectTransform.position.z);
        slot3Ammo.rectTransform.position = new Vector3(defaultXTextPos, slot3Ammo.rectTransform.position.y, slot3Ammo.rectTransform.position.z);
        slot4Ammo.rectTransform.position = new Vector3(defaultXTextPos, slot4Ammo.rectTransform.position.y, slot4Ammo.rectTransform.position.z);
        //Reset the ammo counters
        slot1Ammo.text = "";
        slot2Ammo.text = "";
        slot3Ammo.text = "";
        slot4Ammo.text = "";
        //Reset ammo counter colour
        slot1Ammo.color = Color.white;
        slot2Ammo.color = Color.white;
        slot3Ammo.color = Color.white;
        slot4Ammo.color = Color.white;
        //Only update if the ammo if the weapon slots are being used
        if (slot1Image.enabled == true)
        {
            switch (shootingScript.weaponList[0].weaponModel)
            {
                case WeaponInfo.weapon.pistol:
                    slot1Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.dualPistol:
                    slot1Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.shotgun:
                    slot1Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun);
                    break;
                case WeaponInfo.weapon.machineGun:
                    slot1Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
                    break;
            }

            //Make it red if there is no ammo
            if (slot1Ammo.text == "0")
            {
                slot1Ammo.color = Color.red;
            }
        }
        if (slot2Image.enabled == true)
        {
            switch (shootingScript.weaponList[1].weaponModel)
            {
                case WeaponInfo.weapon.pistol:
                    slot2Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.dualPistol:
                    slot2Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.shotgun:
                    slot2Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun);
                    break;
                case WeaponInfo.weapon.machineGun:
                    slot2Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
                    break;
            }

            //Make it red if there is no ammo
            if (slot2Ammo.text == "0")
            {
                slot2Ammo.color = Color.red;
            }
        }
        if (slot3Image.enabled == true)
        {
            switch (shootingScript.weaponList[2].weaponModel)
            {
                case WeaponInfo.weapon.pistol:
                    slot3Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.dualPistol:
                    slot3Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.shotgun:
                    slot3Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun);
                    break;
                case WeaponInfo.weapon.machineGun:
                    slot3Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
                    break;
            }

            //Make it red if there is no ammo
            if (slot3Ammo.text == "0")
            {
                slot3Ammo.color = Color.red;
            }
        }
        if (slot4Image.enabled == true)
        {
            switch (shootingScript.weaponList[3].weaponModel)
            {
                case WeaponInfo.weapon.pistol:
                    slot4Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.dualPistol:
                    slot4Ammo.text = "∞";
                    break;
                case WeaponInfo.weapon.shotgun:
                    slot4Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun);
                    break;
                case WeaponInfo.weapon.machineGun:
                    slot4Ammo.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
                    break;
            }

            //Make it red if there is no ammo
            if (slot4Ammo.text == "0")
            {
                slot4Ammo.color = Color.red;
            }
        }
    }

    private void UpdateCurrentlySelectedWeapon() //Update the currently selected weapon
    {
        if (shootingScript != null)
        {
            for (int i = 0; i < shootingScript.weaponList.Count; i++)
            {
                if (shootingScript.weaponList[i] == shootingScript.currentWeaponScript)
                {
                    if (i == 0)
                    {
                        slot1Outline.enabled = true;
                        slot2Outline.enabled = false;
                        slot3Outline.enabled = false;
                        slot4Outline.enabled = false;
                    }
                    else if (i == 1)
                    {
                        slot2Outline.enabled = true;
                        slot1Outline.enabled = false;
                        slot3Outline.enabled = false;
                        slot4Outline.enabled = false;
                    }
                    else if (i == 2)
                    {
                        slot3Outline.enabled = true;
                        slot1Outline.enabled = false;
                        slot2Outline.enabled = false;
                        slot4Outline.enabled = false;
                    }
                    else if (i == 3)
                    {
                        slot4Outline.enabled = true;
                        slot1Outline.enabled = false;
                        slot2Outline.enabled = false;
                        slot3Outline.enabled = false;
                    }
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
