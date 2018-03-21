using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//Author: Kate Georgiou Date: 10/10/17 Purpose: contains the name of all the scenes that might need to be called in the Chooselvl script

public class LoadingDifferentScenes : MonoBehaviour {

    public static LoadingDifferentScenes inst;


    void Awake()
    {
        inst = this;
    }
    private void Start()
    {
        
    }

    public void LoadMainLevel()
    {
        SceneManager.LoadScene("Starting Area");
       
    }

    public void LoadTest()
    {
        SceneManager.LoadScene("Test");
    }

    public void LoadRoss()
    {
        SceneManager.LoadScene("Ross");
    }

    public void LoadFabio()
    {
        SceneManager.LoadScene("Level1Bl2");
    }
    public void LoadSimon()
    {
        SceneManager.LoadScene("SimonCargoLevel");
    }
	public void LoadBarracks()
	{
		SceneManager.LoadScene ("BarracksSimonLevel");
	}
	public void LoadHub()
	{
		SceneManager.LoadScene ("Hub New");
	}

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void SpeedRun()
    {
        SceneManager.LoadScene("SpeedRun");
    }
}
