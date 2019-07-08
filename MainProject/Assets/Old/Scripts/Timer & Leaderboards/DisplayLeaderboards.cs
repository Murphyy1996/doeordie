//Author: James Murphy
//Purpose: Load the leaderboord 
//Requirements: To be placed on the leaderboard obj

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DisplayLeaderboards : MonoBehaviour
{
    private Text leadboardText, timeTexts, positionText, pageText;
    private int levelToFilterTo = 1;
    [SerializeField]
    [Header("Leaderboard Scroll Options")]
    private float secondsToScrollFor = 2;
    [SerializeField]
    private float scrollSpeed = 2;
    private bool allowedToScroll = false, scrollUp = true, runOnce = false;
    private Vector3 nameTextDefaultPos, timeTextDefaultPos;
    private Button nextPageResultsButton, previousPageResultsButton;
    private int currentPageOnLeaderboard = 1, countOfTheCurrentFilteredLeaderboard = 0, amountOfPages = 1;

    private void OnEnable() //Run code when enabled and display the leaderboard
    {
        //Sort the leaderboard
        LeaderboardManager.singleton.SortTheLeaderboard();
        //Get the leaderboard text
        if (leadboardText == null)
        {
            leadboardText = GameObject.Find("LeaderboardText").GetComponent<Text>();
            timeTexts = GameObject.Find("LeaderboardTimeTexts").GetComponent<Text>();
            positionText = GameObject.Find("positionText").GetComponent<Text>();
            nextPageResultsButton = GameObject.Find("Next10Results").GetComponent<Button>();
            previousPageResultsButton = GameObject.Find("Previous10Results").GetComponent<Button>();
            pageText = GameObject.Find("pageNumber").GetComponent<Text>();
            //Populate the leaderboard with level 1 by default
            PopulateTheLeaderboard(1);
        }
        //Get default positions
        nameTextDefaultPos = leadboardText.transform.position;
        timeTextDefaultPos = timeTexts.transform.position;
        //Run once to get default positions etc
        if (runOnce == false)
        {
            runOnce = true;
        }
        //Move the texts to default positions 
        MoveTextToDefaultPosition();
        //Get the count of the current filtered leaderboard
        countOfTheCurrentFilteredLeaderboard = LeaderboardManager.singleton.SortedListForSpecifiedLevel(1).Count;
        //Work out the current amount of pages
        float temp = countOfTheCurrentFilteredLeaderboard / 10f;
        amountOfPages = Mathf.RoundToInt(temp) + 1; ;
        pageText.text = "Page: " + currentPageOnLeaderboard.ToString() + " / " + amountOfPages;
    }

    public void BackToTopOfLeaderboard() //Bring the player back to the top of the leaderboard
    {
        allowedToScroll = false;
        previousPageResultsButton.interactable = false;
        currentPageOnLeaderboard = 1;
        MoveTextToDefaultPosition();
    }

    private void MoveTextToDefaultPosition() //Resets the text to default position
    {
        leadboardText.transform.position = nameTextDefaultPos;
        timeTexts.transform.position = timeTextDefaultPos;
    }

    public void PressIncreaseLevelFilterButton() //Will increase the filter
    {
        //Stop text scrolling
        allowedToScroll = false;
        //Reset text to default position
        MoveTextToDefaultPosition();
        //Set level to 1
        currentPageOnLeaderboard = 1;
        //if the level filter is bigger than the scene in the build index then take it back to the beggining
        levelToFilterTo++;
        if (levelToFilterTo >= SceneManager.sceneCountInBuildSettings)
        {
            levelToFilterTo = 1;
        }
        PopulateTheLeaderboard(levelToFilterTo);
    }

    public void ScrollDownwards() //Move the leaderboard texts and times go up
    {
        if (allowedToScroll == false)
        {
            scrollUp = false;
            allowedToScroll = true;
            currentPageOnLeaderboard++;
            Time.timeScale = 1;
            Invoke("StopScroll", secondsToScrollFor);
        }
    }

    public void ScrollUpwards() //Move the leaderboard texts and times go up
    {
        if (allowedToScroll == false)
        {
            scrollUp = true;
            currentPageOnLeaderboard--;
            allowedToScroll = true;
            Time.timeScale = 1;
            Invoke("StopScroll", secondsToScrollFor);
        }
    }

    private void FixedUpdate() //Movement code in fixed update so it is constant accross all pcs
    {
        //Display the current page and how many pages there are
        pageText.text = "Page: " + currentPageOnLeaderboard.ToString() + " / " + amountOfPages;
        //Make the next page button available if there are more results
        if (countOfTheCurrentFilteredLeaderboard > currentPageOnLeaderboard * 10)
        {
            nextPageResultsButton.interactable = true;
        }
        else //Do not make the button available if there are not results
        {
            nextPageResultsButton.interactable = false;
        }
        //Only scroll when needed
        if (allowedToScroll == true)
        {
            if (scrollUp == false)
            {
                leadboardText.transform.position = new Vector3(leadboardText.transform.position.x, leadboardText.transform.position.y + (scrollSpeed * Time.fixedDeltaTime), leadboardText.transform.position.z);
                timeTexts.transform.position = new Vector3(timeTexts.transform.position.x, timeTexts.transform.position.y + (scrollSpeed * Time.fixedDeltaTime), timeTexts.transform.position.z);
            }
            else
            {
                leadboardText.transform.position = new Vector3(leadboardText.transform.position.x, leadboardText.transform.position.y - (scrollSpeed * Time.fixedDeltaTime), leadboardText.transform.position.z);
                timeTexts.transform.position = new Vector3(timeTexts.transform.position.x, timeTexts.transform.position.y - (scrollSpeed * Time.fixedDeltaTime), timeTexts.transform.position.z);
            }
        }
    }

    private void StopScroll() //Stop scrolling
    {
        allowedToScroll = false;
        //If this is the first page on the leaderboard then 
        if (currentPageOnLeaderboard <= 1)
        {
            previousPageResultsButton.interactable = false;
        }
        else
        {
            previousPageResultsButton.interactable = true;
        }
    }

    private void PopulateTheLeaderboard(int levelToShow)
    {
        //Set the current page as 1
        currentPageOnLeaderboard = 1;
        //If this is the first page on the leaderboard then 
        if (currentPageOnLeaderboard <= 1)
        {
            previousPageResultsButton.interactable = false;
        }
        else
        {
            previousPageResultsButton.interactable = true;
        }
        //Get the count of the current filtered leaderboard
        countOfTheCurrentFilteredLeaderboard = LeaderboardManager.singleton.SortedListForSpecifiedLevel(levelToShow).Count;
        //Work out the current amount of pages
        float temp = countOfTheCurrentFilteredLeaderboard / 10f;
        amountOfPages = Mathf.RoundToInt(temp) + 1; ;
        //If the leaderboard does not have enough entries for a next page don't let you do it
        if (countOfTheCurrentFilteredLeaderboard <= 10)
        {
            nextPageResultsButton.interactable = false;
        }
        else
        {
            nextPageResultsButton.interactable = true;
        }
        //If the leaderboard text has succesfully been found
        if (leadboardText != null && timeTexts != null)
        {
            //Clear the previous texts
            leadboardText.text = "";
            timeTexts.text = "";
            positionText.text = "";
            //Display each record in the leaderboard
            for (int i = 0; i < countOfTheCurrentFilteredLeaderboard; i++)
            {
                //Split the unsorted entry into 3 parts: level, name and time
                Char splitter = ',';
                String[] splitStrings = LeaderboardManager.singleton.SortedListForSpecifiedLevel(levelToShow)[i].Split(splitter);
                int level = int.Parse(splitStrings[0]);
                string name = splitStrings[1];
                float time = float.Parse(splitStrings[2]);
                //Fill the leaderboard
                positionText.text = positionText.text + Environment.NewLine + (i + 1) + ".";
                leadboardText.text = leadboardText.text + Environment.NewLine + "                  " + level + "                   " + name;
                timeTexts.text = timeTexts.text + Environment.NewLine + time;
            }
        }
    } //This will display the leaderboards based off what level you have selected
}
