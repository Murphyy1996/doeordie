//Author: James Murphy
//Purpose: To provide a way of holding and saving high scores 
//Requirements: Placed on an empty game object in the main menu and nowhere else

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager singleton;
    [SerializeField]
    private string playerName = "default";
    private int currentLvl = 0;
    [SerializeField]
    private List<string> unsortedLeaderboard = new List<string>(), sortedLeadboard = new List<string>();
    private List<string> tempUnsortedLeaderboard = new List<string>();
    private string currentBest;


    private void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
        }
    }

    private void OnLevelWasLoaded() //Run any code when a scene was loaded
    {
        //Get the current level number for information when saving the score
        currentLvl = SceneManager.GetActiveScene().buildIndex;
        //Sort the leaderboard if it starts with values
        if (unsortedLeaderboard.Count != 0)
        {
            SortTheLeaderboard();
             // sets the highest score
            currentBest = sortedLeadboard[0];
        }
        //Display the best time in game
        if (UIElements.singleton != null && UIElements.singleton.bestTimeLabel != null && sortedLeadboard.Count > 0)
        {
            //Split the unsorted entry into 3 parts: level, name and time
            Char splitter = ',';
            String[] splitStrings = sortedLeadboard[0].Split(splitter);
            string name = splitStrings[1];
            float time = float.Parse(splitStrings[2]);
            UIElements.singleton.bestTimeLabel.text = "Best time: " + name + " " + time;
        }
    }

    private void Start()
    {
       
        //Name this object
        this.gameObject.name = "LeaderboardManager";
        //Don't destroy on load so it can be accessed in any scene
        DontDestroyOnLoad(this);
    }

    public void SetPlayerName(string name) //Set the player name
    {
        if (name == "" || name == " ")
        {
            name = "default";
        }
        playerName = name;
    }

    public string ReturnPlayerName() //Return the player name
    {
        return playerName;
    }

    public List<string> ReturnSortedLeaderboard() //Return the sorted leadboard
    {
        return sortedLeadboard;
    }

    public void AddTimeToUnsortedLeaderboard(string time) //Add the unsorted time to the leaderboard
    {
        unsortedLeaderboard.Add(currentLvl + "," + playerName + "," + time);
        //Sort the leaderboard
        SortTheLeaderboard();
    }

    public void SortTheLeaderboard() //This code will sort the leaderboard
    {
        //Only sort if the lists aren't empty
        if (unsortedLeaderboard.Count != 0 || sortedLeadboard.Count != 0)
        {
            //Make sure all previous records are kept and not cleared by placing them in the unsorted leaderboard
            if (unsortedLeaderboard.Count <= 1)
            {
                foreach (string sortedRecord in sortedLeadboard)
                {
                    unsortedLeaderboard.Add(sortedRecord);
                }
            }
            //Make sure the leaderboard values are not duplicated
            sortedLeadboard.Clear();
            //Make a temporary list of unsorted leadboards allowing it to be changed
            tempUnsortedLeaderboard = unsortedLeaderboard;

            //Filter the times depending on what scene they are in and based on how many in the game
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                //Link the names and levels back up with the times
                foreach (float currentTime in SortTimesOfCertainLevel(i))
                {
                    //Convert the current time to a string
                    string currentTimeString = currentTime.ToString();
                    //Search through the unsorted records and find a matching time and then add it to the leader boards
                    foreach (string record in tempUnsortedLeaderboard)
                    {
                        if (record.Contains(currentTimeString))
                        {
                            //Add the record to the sorted leaderboard
                            sortedLeadboard.Add(record);
                            //Remove the record to make future searches quicker and reduce possible duplication
                            tempUnsortedLeaderboard.Remove(record);
                            break;
                        }
                    }
                }
            }
            //Display the best time in game incase it has been changed
            if (UIElements.singleton != null && UIElements.singleton.bestTimeLabel != null && sortedLeadboard.Count > 0)
            {
                Char splitter = ',';
                //split up the current best time string and extract the float
                String[] splitBest = currentBest.Split(splitter);
                float currentBestTime = float.Parse(splitBest[2]);
                //Split the unsorted entry into 3 parts: level, name and time
                String[] splitStrings = sortedLeadboard[0].Split(splitter);
                string name = splitStrings[1];
                float time = float.Parse(splitStrings[2]);
                //Pop up saying new best time if needed
                if (currentBestTime > time)
                {
                    //Pop up the new record text
                    StartCoroutine(callText());
                    //Set the top time as the best
                    currentBest = sortedLeadboard[0];
                }
                //Update the best time
                UIElements.singleton.bestTimeLabel.text = "Best time: " + name + " " + time;
            }
        }
    }

    private List<float> SortTimesOfCertainLevel(int levelID) //This function will split and sort the times of the desired level
    {
        //Seperate list to hold times and nothing else
        List<float> timesList = new List<float>();
        //Walk though the unsorted leaderboard and collect the times
        foreach (string record in unsortedLeaderboard)
        {
            //Split the unsorted entry into 3 parts: level, name and time
            Char splitter = ',';
            String[] splitStrings = record.Split(splitter);
            //The different parts stored as a variable for easy access
            int level = int.Parse(splitStrings[0]);
            float time = float.Parse(splitStrings[2]);
            if (level == levelID)
            {
                //Add the time to a seperate list
                timesList.Add(time);
            }
        }
        //Sort the list of times numerically
        timesList.Sort();
        //Return the times list
        return timesList;
    }

    IEnumerator callText()
    {
        UIElements.singleton.highscoreText.SetActive(true);
        yield return new WaitForSeconds(2f);
        UIElements.singleton.highscoreText.SetActive(false);
    }
}
