//Author: James Murphy
//Purpose: To provide a way of holding and saving high scores 
//Requirements: Placed on an empty game object in the main menu and nowhere else

using System;
using System.IO;
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
    private string lastKnownBestTime;
    private int lastKnownScene;
    private string filePath;
    private bool runOnce = false;


    private void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(gameObject);
        }
        else //Make this object the singleton
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
            LoadLeaderboard();
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
        }
        //Display the best time in game
        if (UIElements.singleton != null && UIElements.singleton.bestTimeLabel != null && sortedLeadboard.Count > 0)
        {
            //Split the unsorted entry into 3 parts: level, name and time
            Char splitter = ',';
            String[] splitStrings = SortedListForSpecifiedLevel(currentLvl)[0].Split(splitter);
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
            //Only run this if once per scene to get the last known best time
            if (lastKnownScene != SceneManager.GetActiveScene().buildIndex)
            {
                //Set current best as the first in the sorted leaderboard
                lastKnownBestTime = sortedLeadboard[0];
                //Record the last known scene as this one so it doesn't run again this scene
                lastKnownScene = SceneManager.GetActiveScene().buildIndex;
            }
            //Display the best time in game incase it has been changed
            if (UIElements.singleton != null && UIElements.singleton.bestTimeLabel != null && sortedLeadboard.Count > 0)
            {
                Char splitter = ',';
                //split up the current best time string and extract the float
                String[] splitBest = this.lastKnownBestTime.Split(splitter);
                float lastKnownBestTime = float.Parse(splitBest[2]);
                //Split the unsorted entry into 3 parts: level, name and time
                String[] splitStrings = SortedListForSpecifiedLevel(lastKnownScene)[0].Split(splitter);
                string name = splitStrings[1];
                float currentBestTime = float.Parse(splitStrings[2]);
                //Pop up saying new best time if needed
                if (lastKnownBestTime > currentBestTime)
                {
                    //Pop up the new record text
                    StartCoroutine(OpenNewHighScoreText());
                    //Set the top time as the best
                    this.lastKnownBestTime = sortedLeadboard[0];
                }
                //Update the best time
                UIElements.singleton.bestTimeLabel.text = "Best time: " + name + " " + currentBestTime;
            }
            if (sortedLeadboard.Count > 0)
            {
                //Save the leaderboard
                SaveLeaderboard();
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

    public List<string> SortedListForSpecifiedLevel(int levelID) //Will get the leaderboard by level
    {
        //Create a new list for this sorted level leaderboard
        List<string> sortedLevelLeaderboard = new List<string>();
        //Only add to the sorted leaderboard if it has results
        if (sortedLeadboard.Count != 0)
        {
            //Convert the int to string
            string levelIDToSearchFor = levelID.ToString();
            //Only add scores to the list with the level number as the first character
            foreach (string record in sortedLeadboard)
            {
                if (record != null)
                {
                    //Make the record a char array for easy access of each character
                    Char[] recordAsArray = record.ToCharArray();
                    //Compare the first character of this record to the level id
                    if (recordAsArray[0].ToString() == levelIDToSearchFor)
                    {
                        //If they are the same, add it to the sorted specific level leaderboard
                        sortedLevelLeaderboard.Add(record);
                    }
                }
            }
        }
        //Return the sorted level leaderboard
        return sortedLevelLeaderboard;
    }

    private void SaveLeaderboard() //Save the current sorted list into the leaderboard file
    {
        //Get the file path of the leadboard file
        filePath = Environment.CurrentDirectory + @"\leaderboard.txt";

        //Check if the file path exists and if not create it
        if (!File.Exists(filePath))
        {
            // Create a file to write to.
            using (StreamWriter currentLine = File.CreateText(filePath))
            {
                foreach(string entry in unsortedLeaderboard)
                {
                    currentLine.WriteLine(entry);
                }
            }
        }
        else //if the file already exists
        {
            if (File.Exists(filePath))
            {
                // Create a file to write to.
                using (StreamWriter currentLine = File.CreateText(filePath))
                {
                    //write each entry in the leaderboard text file
                    foreach (string entry in sortedLeadboard)
                    {
                        currentLine.WriteLine(entry);
                    }
                }
            }
        }
    }

    private void LoadLeaderboard() //Will load leaderboard if possible
    {
        //Get the file path of the leadboard file
        filePath = Environment.CurrentDirectory + @"\leaderboard.txt";
        if (File.Exists(filePath))
        {
            //Clear existing leaderboards
            unsortedLeaderboard.Clear();
            sortedLeadboard.Clear();
            //Open the leaderboard file to read from.
            foreach (string line in File.ReadAllLines(filePath))
            {
                if (line != "")
                {
                    unsortedLeaderboard.Add(line);
                }
            }
        }
    }

    IEnumerator OpenNewHighScoreText()
    {
        UIElements.singleton.highscoreText.SetActive(true);
        yield return new WaitForSeconds(4f);
        UIElements.singleton.highscoreText.SetActive(false);
    }
}
