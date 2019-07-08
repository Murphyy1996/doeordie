using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class InGamePause : MonoBehaviour
{
    //Author: Kate Georgiou Date 16/10/17 - handles the pausing of the game and allows the player to return to the menu or exit the game.
    //Needs to pause game when ESC is pressed, needs to unpause when its pressed again or resume is pressed - needs to make cursor visible while paused.
    [SerializeField]
    private GameObject pauseMenuPrefab;
    private bool allowedToPause = true, shouldbeVisible = false;
    [SerializeField]
    private GameObject optionsPrefab;
    [SerializeField]
    public Canvas spawnedOptionsCanvas;
    private List<AudioSource> audioSourcesThatNeedsUnpausing = new List<AudioSource>();
    private ReusableHealth playerHealth;
    private GameObject foundDeathScreen;
    private Image deathScreenImage;
    public bool paused = false;
    public static InGamePause inst;





    private void Start()
    {
        inst = this;
        playerHealth = GetComponent<ReusableHealth>();
        if (pauseMenuPrefab != null)
        {
            GameObject pause = Instantiate(pauseMenuPrefab);
            pause.name = "Pause menu";
        }
        else
        {
            print("Pause menu variable has not been filled out");
        }
        GameObject spawnedOptionsMenu = Instantiate(optionsPrefab) as GameObject;  //THIS IS WHERE THE OPTIONS MENU IS INSTANTIATED - NEED TO USE DONTDESTROY INSTEAD
        spawnedOptionsMenu.name = "OptionsMenu";
        spawnedOptionsCanvas = spawnedOptionsMenu.GetComponent<Canvas>();
        Invoke("DelayedStart", 0.5f);
    }

    private void DelayedStart()
    {
        foundDeathScreen = GameObject.Find("DeathScreen");
        deathScreenImage = foundDeathScreen.GetComponent<Image>();
    }


    // Update is called once per frame
    private void Update()
    {
        if (deathScreenImage != null)
        {
            if (deathScreenImage.color.a == 0)
            {
                if (Input.GetKeyUp(KeyCode.Escape) && allowedToPause == true && LoadingUIManager.singleton.IsLoadingScreenActive() == false && playerHealth.healthValue >= 1)
                {
                    OpenPauseMenu();
                }
            }
        }

        if (Time.timeScale == 1)
        {
            if (spawnedOptionsCanvas.enabled == false)
            {
                shouldbeVisible = false;
            }
        }
        else
        {
            shouldbeVisible = true;
        }

        //Make fungus allow the cursor to be visible
        if (QuestManager.inst != null)
        {
            if (QuestManager.inst.inConvo == true)
            {
                shouldbeVisible = true;
            }
        }

        if (spawnedOptionsCanvas != null)
        {
            if (spawnedOptionsCanvas.enabled == true)
            {
                allowedToPause = false;
            }
            else
            {
                allowedToPause = true;
            }
        }

        //Control the visibility of the cursor
        if (shouldbeVisible == true)
        {
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;

        }
        else
        {
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
    }

    public void SetAllowedToPause(bool value)
    {
        allowedToPause = value;
    }

    public void OpenPauseMenu() //This code will open the pause menu
    {
        if (QuestManager.inst.subtitleAudioSource.isPlaying == true)
        {
            QuestManager.inst.subtitleAudioSource.Pause();
            audioSourcesThatNeedsUnpausing.Add(QuestManager.inst.subtitleAudioSource);
        }
        //Get the pause obj and fps script
        FirstPersonCamera fpsScript = Camera.main.GetComponent<FirstPersonCamera>();
        GameObject pauseObj = GameObject.Find("Pause menu");
        //Stop in game time
        Time.timeScale = 0;
        //Stop camera movement
        fpsScript.IsCameraAllowedToMove(false);
        //Enable the pause menu
        pauseObj.GetComponent<Canvas>().enabled = true;
        if (AudioManage.inst.player != null)
        {
            AudioManage.inst.player.Stop();
        }
        if (AudioManage.inst.slide != null)
        {
            AudioManage.inst.slide.Stop();
        }
        if (AudioManage.inst.grapple != null)
        {
            AudioManage.inst.grapple.Stop();
        }
        if (AudioManage.inst.pistolShot)
        {
            AudioManage.inst.pistolShot.Stop();
        }
        if (AudioManage.inst.machShot != null)
        {
            AudioManage.inst.machShot.Stop();
        }

        if (AudioManage.inst.background != null)
        {
            AudioManage.inst.background.Stop();
        }
        shouldbeVisible = true;
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePauseMenu()
    {
        if (QuestManager.inst.subtitleAudioSource.clip != null)
        {
            QuestManager.inst.subtitleAudioSource.UnPause();
        }
        foreach (AudioSource audioSource in audioSourcesThatNeedsUnpausing)
        {
            audioSource.UnPause();
        }
        //Get the pause obj and fps script
        FirstPersonCamera fpsScript = Camera.main.GetComponent<FirstPersonCamera>();
        GameObject pauseObj = GameObject.Find("Pause menu");
        //Start in game time
        Time.timeScale = 1;
        //Allow camera movement
        fpsScript.IsCameraAllowedToMove(true);
        //Disable the pause menu
        pauseObj.GetComponent<Canvas>().enabled = false;
        ////Disable the cursor again
        //Cursor.lockState = CursorLockMode.Locked;
        if (AudioManage.inst.background != null)
        {
            AudioManage.inst.background.Play();
        }
        allowedToPause = true;
    }

    //These are for the different UI buttons

    public void ContinueButton()
    {
        ClosePauseMenu();
        paused = false;
    }

    public void ReturnToMenuButton()
    {
        SceneManager.LoadScene("MainMenuTest");
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }

    public void Options()
    {
        GameObject.Find("OptionsMenu").GetComponent<Canvas>().enabled = true;
        ClosePauseMenu();
        paused = true;
        if (GameObject.Find("OptionsMenu").GetComponent<Canvas>().enabled == false)
        {
            OpenPauseMenu();
        }
        Invoke("PauseGame", 0.1f);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (QuestManager.inst.subtitleAudioSource.isPlaying == true)
        {
            QuestManager.inst.subtitleAudioSource.Pause();
        }
    }

    public void Leaderboard()
    {
        GameObject.Find("HighScore").GetComponent<GameObject>().SetActive(true);
        ClosePauseMenu();

        if (GameObject.Find("HighScore").GetComponent<Canvas>().enabled == false)
        {
            OpenPauseMenu();

        }

    }

}