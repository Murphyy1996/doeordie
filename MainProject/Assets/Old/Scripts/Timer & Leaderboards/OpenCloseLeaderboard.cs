//Author: James Murphy
//Purpose: Open or close the leaderboard
//Requirements: Link to the leaderboard obj

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenCloseLeaderboard : MonoBehaviour
{
    private GameObject leaderboardIngame;
    [SerializeField]
    private GameObject mainMenuLeaderboard;

    private bool runOnce = false;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (runOnce == false)
            {
                runOnce = true;
                leaderboardIngame = GameObject.Find("LeaderboardInGame");
                leaderboardIngame.SetActive(false);
            }
        }
    }

    public void OpenCloseLeaderboardButton()
    {
        if (leaderboardIngame != null)
        {
            //Go back to the top of the leaderboard
            if (leaderboardIngame.activeSelf == true)
            {
                leaderboardIngame.GetComponentInChildren<DisplayLeaderboards>().BackToTopOfLeaderboard();
            }
            leaderboardIngame.SetActive(!leaderboardIngame.activeSelf);
        }
        if (mainMenuLeaderboard != null)
        {
            mainMenuLeaderboard.SetActive(!mainMenuLeaderboard.activeSelf);
            //Go back to the top of the leaderboard
            if (mainMenuLeaderboard.activeSelf == true)
            {
                mainMenuLeaderboard.GetComponent<DisplayLeaderboards>().BackToTopOfLeaderboard();
            }
        }
    }
}
