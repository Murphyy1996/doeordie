//Author: James Murphy
//Purpose: To let the player reload the game
//Requirements: To be placed on an object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    private void Update() //Check for button presses
    {
        if (Input.GetKey(KeyCode.Space))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Teleporting>().CancelTeleport();
            player.GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(true);
            //Refresh player ammo
            AmmoManager ammo = player.GetComponent<AmmoManager>();
            ammo.SetAmmoAmount(AmmoManager.ammoType.machineGun, ammo.ReturnMaxAmmoForMachineGun() / 2);
            ammo.SetAmmoAmount(AmmoManager.ammoType.shotgun, ammo.ReturnMaxAmmoForShotgun() / 2);
            //Allow camera movement again
            player.GetComponentInChildren<FirstPersonCamera>().IsCameraAllowedToMove(false);
            //Reset camera
            Camera.main.GetComponent<FirstPersonCamera>().RecenterCameraVertical();
            //Allow camera movement again
            player.GetComponentInChildren<FirstPersonCamera>().IsCameraAllowedToMove(true);
            //Fade out the game over screen
            InGameUI.inst.FadeOutGameOver(2f);
            //Unpause the game
            QuestManager.inst.UnPauseGame();
            //Destroy this script as its no longer needed
            Destroy(this);
        }
    }
}
