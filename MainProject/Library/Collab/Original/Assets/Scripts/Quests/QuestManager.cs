using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //Author: Kate Georgiou 17/10/17
    public static QuestManager inst;
    public QuestTemplate currentQuest;
    public List<QuestTemplate> pickedUpQuests = new List<QuestTemplate>();
    public List<QuestTemplate> complete = new List<QuestTemplate>();
    private Quaternion preDialogueCamRotation, preDialogueRotation;
    private bool preDialogueTeleportValue;

    private void Awake()
    {
        inst = this;
    }

    public void StartDialogue() //Pause the game, stop the camera and show the mouse
    {
        GetComponent<ReusableHealth>().SetInvincibleValue(true);
        GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(false);
        GetComponent<Crouch>().SetCrouchAllowedValue(false);
        GetComponent<Teleporting>().SetTeleportEnabledValue(false);
        GetComponent<Shooting>().SetShootAllowedValue(false);
        GetComponent<InGamePause>().SetAllowedToPause(false);
        Camera.main.GetComponent<FirstPersonCamera>().IsCameraAllowedToMove(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public bool CheckIfQuestCompleted(QuestTemplate questReference) //This will return whether a quest is completed or not
    {
        foreach (QuestTemplate quest in complete)
        {
            if (quest == questReference)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfQuestPickedUp(QuestTemplate questReference) //Will return true or false whether a quest has been picked up
    {
        foreach (QuestTemplate quest in pickedUpQuests)
        {
            if (quest == questReference)
            {
                return true;
            }
        }
        return false;
    }

    public void PauseGame() //Used for pausing the game
    {
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
    }

    public void UpdateQuestText()
    {
        if (UIElements.singleton.playerQuestText != null)
        {
            UIElements.singleton.playerQuestText.text = "Current Quest : " + System.Environment.NewLine + "Name: " + currentQuest.name + System.Environment.NewLine + "Description : " + currentQuest.description + System.Environment.NewLine + "Reward : " + currentQuest.reward;
        }
    }

    public void PickUpNextQuest(GameObject npc) //To be called in the rewards section and pick up the next quest on the npc
    {
        foreach (QuestTemplate quest in npc.GetComponents<QuestTemplate>())
        {
            if (!complete.Contains(quest) && CheckIfQuestPickedUp(quest) == false)
            {
                pickedUpQuests.Add(quest);
                if (currentQuest == null)
                {
                    currentQuest = quest;
                }
                break;
            }
        }
    }

    public void EndDialogue() //Pause the game, stop the camera and show the mouse
    {
        GetComponent<ReusableHealth>().SetInvincibleValue(false);
        GetComponent<Crouch>().SetCrouchAllowedValue(true);
        GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(true);
        GetComponent<Shooting>().SetShootAllowedValue(true);
        GetComponent<InGamePause>().SetAllowedToPause(true);
        GetComponent<Teleporting>().SetTeleportEnabledValue(true);
        Camera.main.GetComponent<FirstPersonCamera>().IsCameraAllowedToMove(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UnPauseGame();
    }
}
