using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    //Author: Kate Georgiou Date: 10/10/17  Purpose: gives the different buttons on the main menu some functionality.
    public Canvas options, menu;

    private void Update()
    {
        if (options.enabled == true && Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToMenu();
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Level1Bl2");
    }

    public void LoadBossLevelTemp() //Load the boss level
    {
        SceneManager.LoadScene(3);
    }

    public void LoadOptions()
    {
        //open and close canvas'
        options.enabled = true;
        menu.enabled = false;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void GoBack()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (options == null)
            {
                options = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
            }
            if (options != null)
            {
                options.enabled = false;
            }
        }
    }

    public void ResetOptionsToDefault()
    {
        OptionsConfig.inst.InputReturnToDefaultButton();
    }

    public void GoBackToMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //options.enabled = false;
            OptionsConfig.inst.SaveOptions();
            if (menu != null)
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    menu.enabled = true;
                }
                else
                {
                    GameObject.Find("Player").GetComponent<InGamePause>().OpenPauseMenu();
                }
            }

        }
      
    }

    private void OnLevelWasLoaded(int level)
    {
        // if (menu == null)
        //  {

        // }
        // else
        // {
        //     return;
        // }
        // if (options == null)
        // {

        // }
        // else
        // {
        //    return;
        //}
        Invoke("delayedLoad", 2);
    }

    void delayedLoad()
    {
        if (menu == null)
        {
            menu = GameObject.Find("Pause menu").GetComponent<Canvas>();
        }
        if (options == null)
        {
            options = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
        }

    }

}
