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
            player.GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(true);
            //Allow camera movement again
            player.GetComponentInChildren<FirstPersonCamera>().IsCameraAllowedToMove(false);
            //Rotate the player 
            player.transform.rotation = CheckpointManager.singleton.GetCurrentCheckpoint().transform.rotation;
            //Force the player camera back in line
            Camera.main.transform.localPosition = new Vector3(0, Camera.main.transform.localPosition.y, 0);
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
