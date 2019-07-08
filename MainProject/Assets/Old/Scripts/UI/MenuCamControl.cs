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
    //if passed the time out, do the force lerp
    [SerializeField]
    private float counter = 0, timeout = 2f;


    private void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Increase the time 
        counter = counter + Time.fixedDeltaTime;
        float lerpSpeed = speedFactor * Time.fixedDeltaTime;
        Cursor.visible = true;
        //All the if statements that involve moving the camera
        if (mainMenu == true)
        {
            if (transform.position != mainMenuPosition.position && transform.rotation != mainMenuPosition.rotation)
            {
                transform.position = Vector3.Lerp(transform.position, mainMenuPosition.position, lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, mainMenuPosition.rotation, lerpSpeed);
            }
            else
            {
                mainMenu = false;
                stillMoving = false;
            }
            //Force the main menu position
            if (counter > timeout)
            {
                transform.SetPositionAndRotation(mainMenuPosition.position, mainMenuPosition.rotation);
                mainMenu = false;
                stillMoving = false;
            }
        }

        if (titleCard == true)
        {
            if (transform.position != titleCardPosition.position && transform.rotation != titleCardPosition.rotation)
            {
                transform.position = Vector3.Lerp(transform.position, titleCardPosition.position, lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, titleCardPosition.rotation, lerpSpeed);
            }
            else
            {
                titleCard = false;
                stillMoving = false;
            }
            if (counter > timeout)
            {
                transform.SetPositionAndRotation(titleCardPosition.position, titleCardPosition.rotation);
                titleCard = false;
                stillMoving = false;
            }
        }

        if (levelSelect == true)
        {
            if (transform.position != levelSelectPosition.position && transform.rotation != levelSelectPosition.rotation)
            {
                transform.position = Vector3.Lerp(transform.position, levelSelectPosition.position, lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, levelSelectPosition.rotation, lerpSpeed);
            }
            else
            {
                levelSelect = false;
                stillMoving = false;
            }
            if (counter > timeout)
            {
                transform.SetPositionAndRotation(levelSelectPosition.position, levelSelectPosition.rotation);
                levelSelect = false;
                stillMoving = false;
            }
        }

        if (options == true)
        {
            if (transform.position != optionsPosition.position && transform.rotation != optionsPosition.rotation)
            {
                transform.position = Vector3.Lerp(transform.position, optionsPosition.position, lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, optionsPosition.rotation, lerpSpeed);
            }
            else
            {
                options = false;
                stillMoving = false;
            }
            if (counter > timeout)
            {
                transform.SetPositionAndRotation(optionsPosition.position, optionsPosition.rotation);
                options = false;
                stillMoving = false;
            }
        }

        if (leaderboards == true)
        {
            if (transform.position != leaderboardsPosition.position && transform.rotation != leaderboardsPosition.rotation)
            {
                transform.position = Vector3.Lerp(transform.position, leaderboardsPosition.position, lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, leaderboardsPosition.rotation, lerpSpeed);
            }
            else
            {
                leaderboards = false;
                stillMoving = false;
            }
            if (counter > timeout)
            {
                transform.SetPositionAndRotation(leaderboardsPosition.position, leaderboardsPosition.rotation);
                leaderboards = false;
                stillMoving = false;
            }
        }

        if (credits == true)
        {
            if (transform.position != creditsPosition.position && transform.rotation != creditsPosition.rotation)
            {
                transform.position = Vector3.Lerp(transform.position, creditsPosition.position, lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, creditsPosition.rotation, lerpSpeed);
            }
            else
            {
                credits = false;
                stillMoving = false;
            }
            if (counter > timeout)
            {
                transform.SetPositionAndRotation(creditsPosition.position, creditsPosition.rotation);
                credits = false;
                stillMoving = false;
            }
        }

        //If that stops the player from clicking things
        if (stillMoving == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }





    //Public voids that tell which camera position to move to. Have to turn off all the bools somehow and at somepoint
    public void SwitchToMainMenu()
    {
        counter = 0;
        mainMenu = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToTitleCard()
    {
        counter = 0;
        titleCard = true;
        StartCoroutine(TurnOffBools());

    }

    public void SwitchToLevelSelect()
    {
        counter = 0;
        levelSelect = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToOptions()
    {
        counter = 0;
        options = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToLeaderboards()
    {
        counter = 0;
        leaderboards = true;
        StartCoroutine(TurnOffBools());
    }

    public void SwitchToCredits()
    {
        counter = 0;
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
        counter = 0;
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