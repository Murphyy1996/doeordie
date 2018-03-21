//Author: James Murphy
//Purpose: Contain references to UI elements
//Requirements: On an object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    public static UIElements singleton;
    [SerializeField]
    [Header("UI Prefabs")]
    private GameObject playerHudPrefab;
    [Header("Manual Scene References")]
    [Header("Found Scene References")]
    public GameObject playerHUD, highscoreText;
    public Text playerQuestText, interactionText, travelIndication, ledgeClimb, timerText, bestTimeLabel, subtitle;
    public Image cooldownGrapple, cooldownTele, missionBackgroundImg;

    private void Awake()
    {
        singleton = this;

        playerHUD = Instantiate(playerHudPrefab) as GameObject;
        playerQuestText = GameObject.Find("QuestText").GetComponent<Text>();
        interactionText = GameObject.Find("InteractionText").GetComponent<Text>();
        cooldownGrapple = GameObject.Find("NewRetColourR").GetComponent<Image>();
        subtitle = GameObject.Find("SubtitleText").GetComponent<Text>();
        subtitle.text = "";
        cooldownTele = GameObject.Find("NewRetColourL").GetComponent<Image>();
        travelIndication = GameObject.Find("TravelIndication").GetComponent<Text>();
        ledgeClimb = GameObject.Find("ledgeGrabHint").GetComponent<Text>();
        missionBackgroundImg = GameObject.Find("QuestBackGround").GetComponent<Image>();
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        bestTimeLabel = GameObject.Find("BestTime").GetComponent<Text>();
        highscoreText = GameObject.Find("Highscore");
        highscoreText.SetActive(false);
        if (UIElements.singleton.missionBackgroundImg != null)
        {
            UIElements.singleton.missionBackgroundImg.enabled = false;
        }
    }

    private void Start()
    {
        travelIndication.enabled = false;
    }
}
