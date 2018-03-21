using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class QuestManager : MonoBehaviour
{
    //Author: Kate Georgiou 17/10/17
    public static QuestManager inst;
    public QuestTemplate currentQuest;
    public List<QuestTemplate> pickedUpQuests = new List<QuestTemplate>();
    public List<QuestTemplate> complete = new List<QuestTemplate>();
    private Quaternion preDialogueCamRotation, preDialogueRotation;
    private bool preConvoGrappleVal = false, preConvoTeleportVal = false;
  //  private NavMeshObstacle playerNav;
    private Camera mainCamera;
    private FirstPersonCamera fpsScript;
    public bool inConvo = false;
    private float timer = 0, timerTarget = 0;
    [SerializeField]
    private AudioSource subtitleAudioSource;

    private void Awake()
    {
        inst = this;
        Invoke("DelayedAwake", 0.2f);
    }

    private void DelayedAwake()
    {
        mainCamera = Camera.main;
        fpsScript = mainCamera.GetComponent<FirstPersonCamera>();
    }

    private void Start()
    {
       // playerNav = GetComponent<NavMeshObstacle>();
       // playerNav.enabled = false;
    }

    private void Update()
    {
        //Dont allow camera movement if in conversation
        if (fpsScript != null && inConvo == true)
        {
            fpsScript.IsCameraAllowedToMove(false);
        }
        else if (inConvo == true)
        {
            if (Camera.main.GetComponent<FirstPersonCamera>() != null)
            {
                fpsScript = Camera.main.GetComponent<FirstPersonCamera>();
                fpsScript.IsCameraAllowedToMove(false);
            }
        }
    }

    public void StartDialogue() //Pause the game, stop the camera and show the mouse
    {
        inConvo = true;
       // playerNav.enabled = true;
        preConvoGrappleVal = GetComponent<Grapple>().IsGrappleAllowed();
        preConvoTeleportVal = GetComponent<Teleporting>().ReturnIfTeleportEnabled();
        GetComponent<ReusableHealth>().SetInvincibleValue(true);
        GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(false);
        GetComponent<Crouch>().SetCrouchAllowedValue(false);
        GetComponent<Teleporting>().SetTeleportEnabledValue(false);
        GetComponent<Grapple>().SetGrappleAllowedValue(false);
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
            UIElements.singleton.playerQuestText.text = "Mission Objective: " + System.Environment.NewLine + "" + currentQuest.name + System.Environment.NewLine + "" + currentQuest.description; //+ System.Environment.NewLine + "Reward : " + currentQuest.reward;
            if (UIElements.singleton.missionBackgroundImg != null)
            {
                if (UIElements.singleton.playerQuestText.text == "")
                {
                    UIElements.singleton.missionBackgroundImg.enabled = false;
                }
                else
                {
                    UIElements.singleton.missionBackgroundImg.enabled = true;
                }
            }
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
        inConvo = false;
        GetComponent<ReusableHealth>().SetInvincibleValue(false);
        GetComponent<Crouch>().SetCrouchAllowedValue(true);
        GetComponent<CharacterControllerMovement>().IsPlayerInputEnabled(true);
        GetComponent<Shooting>().SetShootAllowedValue(true);
        GetComponent<InGamePause>().SetAllowedToPause(true);
        GetComponent<Teleporting>().SetTeleportEnabledValue(preConvoTeleportVal);
        GetComponent<Grapple>().SetGrappleAllowedValue(preConvoGrappleVal);
        Camera.main.GetComponent<FirstPersonCamera>().IsCameraAllowedToMove(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UnPauseGame();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale > 0)
        {
            //Increment the timer
            timer = timer + Time.fixedDeltaTime;
            //If the timer is bigger than the timer target
            if (timer > timerTarget)
            {
                //Wipe the text
                UIElements.singleton.subtitle.text = "";
            }
        }
    }

    public void SubtitleText(string writing, float displayTime) //Display the subtitles for the specified amount of time
    {
        timer = 0;
        timerTarget = displayTime;
        UIElements.singleton.subtitle.text = writing;
    }

    public void SubtitleVoiceClip(AudioClip subtitleAudioClip, float subtitleVolume) //Subtitle voice clip is now seperate
    {
        //Configure and play the matching audio clip
        if (subtitleAudioSource != null && subtitleAudioClip != null)
        {
            subtitleAudioSource.Stop();
            if (subtitleAudioClip != null)
            {
                subtitleAudioSource.clip = subtitleAudioClip;
                subtitleAudioSource.volume = subtitleVolume;
                subtitleAudioSource.Play();
            }
        }
    }

    public void CancelPreviousSubtitle() //This should cancel previous subtitles
    {
        timer = 999999999999999;
    }


}
