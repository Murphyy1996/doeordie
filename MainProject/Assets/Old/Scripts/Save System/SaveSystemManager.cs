//Author: Kate Georgiou
//Purpose: Load and save the game
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveSystemManager : MonoBehaviour
{
    public static SaveSystemManager inst;

    private void Awake()
    {
        if (inst)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            inst = this;
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("CurrentScene", SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadGame()
    {
        //By default load the tutorial
        string sceneNameToLoad = "LevelBl2";
        //Only load from preferences if the key exists
        if (PlayerPrefs.HasKey("CurrentScene") == true)
        {
            int loadedSceneID = PlayerPrefs.GetInt("CurrentScene");
            switch (loadedSceneID)
            {
                case 1:
                    sceneNameToLoad = "Level1Bl2";
                    break;
                case 2:
                    sceneNameToLoad = "JURPHYV3";
                    break;
                case 3:
                    sceneNameToLoad = "Boss";
                    break;
                case 4:
                    sceneNameToLoad = "SpeedRun";
                    break;
            }
        }
        //If the player is in the scene and this method is accessed via the pause screen, then close the pause menu
        if (GameObject.Find("Player") != null)
        {
            GameObject.Find("Player").GetComponent<InGamePause>().ClosePauseMenu();
        }
        //Load scene by name
        LoadingUIManager.singleton.ShowLoadingScreen(sceneNameToLoad);
    }

}
