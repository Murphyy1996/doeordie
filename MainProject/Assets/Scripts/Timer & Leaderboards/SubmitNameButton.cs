//Author: James Murphy
//Purpose: Contains the code for when the submit button is pressed
//Requirements: A button and an input field

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitNameButton : MonoBehaviour
{
    private InputField nameInputField;
    private GameObject loggedInPlayerField, inputNameFieldObj;
    private Text loggedInNameLabel;

    private void Start()
    {
        inputNameFieldObj = GameObject.Find("InputNameBackground");
        nameInputField = inputNameFieldObj.GetComponentInChildren<InputField>();
        loggedInPlayerField = GameObject.Find("LoggedInPlayer");
        loggedInNameLabel = GameObject.Find("loggedInNameLabel").GetComponent<Text>();
        if (loggedInPlayerField != null)
        {
            loggedInPlayerField.SetActive(false);
        }
        nameInputField = GetComponentInChildren<InputField>();
        if (LeaderboardManager.singleton != null)
        {
            if (LeaderboardManager.singleton.ReturnPlayerName() != "default")
            {
                //If the player name is something other than default then keep the current name
                LeaderboardManager.singleton.SetPlayerName(LeaderboardManager.singleton.ReturnPlayerName());
                if (loggedInPlayerField != null)
                {
                    loggedInNameLabel.text = LeaderboardManager.singleton.ReturnPlayerName();
                    loggedInPlayerField.SetActive(true);
                }
                inputNameFieldObj.SetActive(false);
            }
        }
    }
    public void PressSubmitNameButton()
    {
        LeaderboardManager.singleton.SetPlayerName(nameInputField.text);
        if (loggedInPlayerField != null)
        {
            loggedInNameLabel.text = LeaderboardManager.singleton.ReturnPlayerName();
            loggedInPlayerField.SetActive(true);
            inputNameFieldObj.SetActive(false);
        }
    }

    public void DeletePlayerProfile()
    {
        //Set the player name as default so it requests a new one
        LeaderboardManager.singleton.SetPlayerName("default");
        nameInputField.text = "";
        loggedInPlayerField.SetActive(false);
        inputNameFieldObj.SetActive(true);
    }
}
