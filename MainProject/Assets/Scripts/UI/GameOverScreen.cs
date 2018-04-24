//Author: James Murphy
//Purpose: To let the player reload the game
//Requirements: To be placed on an object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    private bool runOnce = false;
    private void Update() //Check for button presses
    {
        if (Input.GetKeyUp(KeyCode.Space) && runOnce == false)
        {
            runOnce = true;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //Move the player back to the closest checkpoint
            if (CheckpointManager.singleton.GetCurrentCheckpoint() != null)
            {
                player.transform.SetPositionAndRotation(CheckpointManager.singleton.GetCurrentCheckpoint().transform.position, CheckpointManager.singleton.GetCurrentCheckpoint().transform.rotation);
            }
            player.GetComponent<Teleporting>().CancelTeleport();
            player.GetComponent<Teleporting>().SetTeleportEnabledValue(false);
            player.GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(true);
            //Refresh player ammo
            AmmoManager ammo = player.GetComponent<AmmoManager>();
            ammo.SetAmmoAmount(AmmoManager.ammoType.machineGun, ammo.ReturnMaxAmmoForMachineGun() / 2);
            ammo.SetAmmoAmount(AmmoManager.ammoType.shotgun, ammo.ReturnMaxAmmoForShotgun() / 2);
            //Stop combat moosic
            AudioManage.inst.combatMusic.Stop();
            //Allow camera movement again
            player.GetComponentInChildren<FirstPersonCamera>().IsCameraAllowedToMove(false);
            //Reset camera
            Camera.main.GetComponent<FirstPersonCamera>().RecenterCameraVertical();
            //Find every drone and reset it's position
            GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject enemy in foundEnemies)
            {
                if (enemy.layer == 19)
                {
                    enemy.GetComponent<Drone>().ResetAI();
                }
            }
            //Allow camera movement again
            player.GetComponentInChildren<FirstPersonCamera>().IsCameraAllowedToMove(true);
            //Fade out the game over screen
            InGameUI.inst.FadeOutGameOver(2f);
            //Unpause the game
            QuestManager.inst.UnPauseGame();
            //Allow the player to shoot
            player.GetComponent<Shooting>().allowedToShoot = true;
            player.GetComponent<Teleporting>().SetTeleportEnabledValue(true);
            player.GetComponent<ReusableHealth>().SetInvincibleValue(false);
            player.GetComponent<ReusableHealth>().playerIsDead = false;
            player.GetComponent<ReusableHealth>().runOnce = false;
            Destroy(this);
        }
    }

    private void DestroyMe()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //Don't make the player invinicble any more
        player.GetComponent<ReusableHealth>().SetInvincibleValue(false);
        //Destroy this script as its no longer needed

    }
}
