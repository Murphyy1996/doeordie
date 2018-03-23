using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//Author: Kate Georgiou Date: 10/10/17 Purpose : lets the creators (in editor mode) to attach this script to a trigger that they 
//need to get to a different scene and select which scene they want that specific area to be linked to via a dropdown in the inspector.

public class ChooseLvl : MonoBehaviour
{
    public enum LevelToLoad
    {
        main, test, simon, ross, fabio, foundry, hub, barracks, mainmenu, speedrun
    };
    [SerializeField]
    private LevelToLoad selectedLevel;


    // Use this for initialization

    void Start()
    {
        UIElements.singleton.travelIndication.enabled = false;
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            UIElements.singleton.travelIndication.enabled = true;
            if (Input.GetKey(KeyCode.E))
            {
                switch (selectedLevel)
                {
                    case LevelToLoad.fabio:
                        LoadingDifferentScenes.inst.LoadFabio();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.foundry:
                        SceneManager.LoadScene("Level3Blockout");
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.main:
                        LoadingDifferentScenes.inst.LoadMainLevel();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.ross:
                        LoadingDifferentScenes.inst.LoadRoss();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.simon:
                        LoadingDifferentScenes.inst.LoadSimon();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.test:
                        LoadingDifferentScenes.inst.LoadTest();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.hub:
                        LoadingDifferentScenes.inst.LoadHub();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.barracks:
                        LoadingDifferentScenes.inst.LoadBarracks();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.mainmenu:
                        LoadingDifferentScenes.inst.MainMenu();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                    case LevelToLoad.speedrun:
                        LoadingDifferentScenes.inst.SpeedRun();
                        UIElements.singleton.travelIndication.enabled = false;
                        break;
                }

            }


        }



    }
    private void OnTriggerExit(Collider other)
    {
        UIElements.singleton.travelIndication.enabled = false;
    }

    private void OnLevelWasLoaded(int level)
    {
        UIElements.singleton.travelIndication.enabled = false;
    }



}
