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
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentScene"));
    }

}
