using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamControl : MonoBehaviour
{

    //Author: James Cook
    //Created: 13/02/18
    //Purpose: When you press a button in the main menu, it takes the caemra to a new position

    //floats/ints
    [SerializeField]
    float speedFactor = 0.1f;

    //bools
    bool mainMenu = false;
    bool titleCard = false;
    bool levelSelect = false;
    bool options = false;
    bool leaderboards = false;
    bool credits = false;

    bool controlPlayer = false;

    bool stillMoving = false;

    //game objects
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject menuCamera;

    //Transforms
    [SerializeField]
    Transform mainMenuPosition;
    [SerializeField]
    Transform titleCardPosition;
    [SerializeField]
    Transform playerPosition;
    [SerializeField]
    Transform levelSelectPosition;
    [SerializeField]
    Transform optionsPosition;
    [SerializeField]
    Transform leaderboardsPosition;
    [SerializeField]
    Transform creditsPosition;

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        //All the if statements that involve moving the camera
        if (mainMenu == true)
        {
            transform.position = Vector3.Lerp(transform.position, mainMenuPosition.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, mainMenuPosition.rotation, speedFactor);
            //Force the main menu position
            if (Vector3.Distance(transform.position, mainMenuPosition.position) <= 3f)
            {
                transform.SetPositionAndRotation(mainMenuPosition.position, mainMenuPosition.rotation);
            }
            //            StartCoroutine(TurnOffBools());
        }

        if (titleCard == true)
        {
            transform.position = Vector3.Lerp(transform.position, titleCardPosition.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, titleCardPosition.rotation, speedFactor);
            if (Vector3.Distance(transform.position, titleCardPosition.position) <= 3f)
            {
                transform.SetPositionAndRotation(titleCardPosition.position, titleCardPosition.rotation);
            }
            //            StartCoroutine(TurnOffBools());
        }

        if (levelSelect == true)
        {
            transform.position = Vector3.Lerp(transform.position, levelSelectPosition.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, levelSelectPosition.rotation, speedFactor);
            if (Vector3.Distance(transform.position, levelSelectPosition.position) <= 3f)
            {
                transform.SetPositionAndRotation(levelSelectPosition.position, levelSelectPosition.rotation);
            }
            //            StartCoroutine(TurnOffBools());
        }

        if (options == true)
        {
            transform.position = Vector3.Lerp(transform.position, optionsPosition.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, optionsPosition.rotation, speedFactor);
            if (Vector3.Distance(transform.position, optionsPosition.position) <= 3f)
            {
                transform.SetPositionAndRotation(optionsPosition.position, optionsPosition.rotation);
            }
        }

        if (leaderboards == true)
        {
            transform.position = Vector3.Lerp(transform.position, leaderboardsPosition.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, leaderboardsPosition.rotation, speedFactor);
            if (Vector3.Distance(transform.position, leaderboardsPosition.position) <= 3f)
            {
                transform.SetPositionAndRotation(leaderboardsPosition.position, leaderboardsPosition.rotation);
            }
        }

        if (credits == true)
        {
            transform.position = Vector3.Lerp(transform.position, creditsPosition.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, creditsPosition.rotation, speedFactor);
            if (Vector3.Distance(transform.position, creditsPosition.position) <= 3f)
            {
                transform.SetPositionAndRotation(creditsPosition.position, creditsPosition.rotation);
            }
        }

        //If that stops the player from clicking things
        if (stillMoving == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //If that lets the player click
        if (stillMoving == false)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //Statement that allows you to take control of the player
        if (controlPlayer == true)
        {
            transform.position = Vector3.Lerp(transform.position, playerPosition.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, playerPosition.rotation, speedFactor);
            StartCoroutine(StartPlayer());
        }

    }





    //Public voids that tell which camera position to move to. Have to turn off all the bools somehow and at somepoint
    public void SwitchToMainMenu()
    {
        mainMenu = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToTitleCard()
    {
        titleCard = true;
        StartCoroutine(TurnOffBools());

    }

    public void SwitchToLevelSelect()
    {
        levelSelect = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToOptions()
    {
        options = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToLeaderboards()
    {
        leaderboards = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToCredits()
    {
        credits = true;
        StartCoroutine(TurnOffBools());
    }






    //Public void that allows control of the player
    public void SwitchToPlayer()
    {
        controlPlayer = true;
    }





    //Coroutine that turns off all bools, so you can go backwards
    private IEnumerator TurnOffBools()
    {
        Time.timeScale = 1;
        //When true, stops the player pressing buttons
        stillMoving = true;
        yield return new WaitForSeconds(1.5f);
        //Turns off all bools
        mainMenu = false;
        titleCard = false;
        levelSelect = false;
        options = false;
        leaderboards = false;
        credits = false;

        //allows the player to click again
        stillMoving = false;
        Time.timeScale = 0;
        //make sure the cursor is visible



        // yield return null;
    }

    private IEnumerator StartPlayer()
    {
        yield return new WaitForSeconds(0.2f);

        //        controlPlayer = !controlPlayer;
        player.SetActive(true);
        menuCamera.SetActive(false);

        yield return null;
    }
}