//Author: James Murphy
//Purpose: Control the options menu in the main menu
//Requirements: Options menu and an options manager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMainMenu : MonoBehaviour
{
    [SerializeField]
    private OptionsConfig optionsSingletonReference;
    [SerializeField]
    [Header("Toggle References")]
    private Toggle sprintTogRef;
    [SerializeField]
    private Toggle zoomTogRef, crouchTogRef, invertTogRef, windowedTogRef, vSyncTogRef, postProcessingTogRef;
    [SerializeField]
    [Header("Keycode References")]
    private Toggle zoomKeycodeRef;
    [SerializeField]
    private Toggle sprintKeycodeRef, crouchKeycodeRef, grappleKeycodeRef, weaponSwapKeycodeRef;
    [SerializeField]
    private Text sprintInputText, zoomInputText, crouchInputText, grappleInputText, swapInputText;
    [SerializeField]
    [Header("General Variables")]
    private bool initialLoadCompleted = false;
    [SerializeField]
    private GameObject visualOptionsTab, controlOptionsTab, miscOptionsTab;

    private void Awake() //Get any required components if neccesary
    {
        optionsSingletonReference = GameObject.Find("OptionsManager").GetComponent<OptionsConfig>();

        //Read options file

        //Mark the initial load as completed
        initialLoadCompleted = true;
    }
    private void Update()
    {
        if (initialLoadCompleted == true)
        {
            //Control the sprint toggle option
            if (sprintTogRef.isOn == true)
            {
                optionsSingletonReference.sprintTog = true;
            }
            else
            {
                optionsSingletonReference.sprintTog = false;
            }
            //Control the zoom toggle option
            if (zoomTogRef.isOn == true)
            {
                optionsSingletonReference.zoomToggleBool = true;
            }
            else
            {
                optionsSingletonReference.zoomToggleBool = false;
            }
            //Control the crouch toggle option
            if (crouchTogRef.isOn == true)
            {
                optionsSingletonReference.CrouchTog = true;
            }
            else
            {
                optionsSingletonReference.CrouchTog = false;
            }
            //Control the invert toggle option
            if (invertTogRef.isOn == true)
            {
                optionsSingletonReference.CamInvert = true;
            }
            else
            {
                optionsSingletonReference.CamInvert = false;
            }
            //Control the post toggle option
            if (postProcessingTogRef.isOn == true)
            {
                optionsSingletonReference.postPEnabled = true;
            }
            else
            {
                optionsSingletonReference.postPEnabled = false;
            }
            //Control the vsync toggle option
            if (vSyncTogRef.isOn == true)
            {
                optionsSingletonReference.vysncbool = true;
            }
            else
            {
                optionsSingletonReference.vysncbool = false;
            }
            //Control the windowed toggle option
            if (windowedTogRef.isOn == true)
            {
                Screen.fullScreen = true;
            }
            else
            {
                Screen.fullScreen = false;
            }
            //Update the values of the button inputs
            zoomInputText.text = optionsSingletonReference.zoomKeycode.ToString();
            crouchInputText.text = optionsSingletonReference.crouchKeycode.ToString();
            grappleInputText.text = optionsSingletonReference.grappleKeycode.ToString();
            swapInputText.text = optionsSingletonReference.weaponSwapcode.ToString();
            sprintInputText.text = optionsSingletonReference.sprintKeycode.ToString();

        }
    }


    //Methods for button presses

    public void OpenVisualOptions()
    {
        visualOptionsTab.SetActive(true);
        miscOptionsTab.SetActive(false);
        controlOptionsTab.SetActive(false);
    }

    public void OpenControlOptions()
    {
        visualOptionsTab.SetActive(false);
        miscOptionsTab.SetActive(false);
        controlOptionsTab.SetActive(true);
    }

    public void OpenMiscOptions()
    {
        visualOptionsTab.SetActive(false);
        miscOptionsTab.SetActive(true);
        controlOptionsTab.SetActive(false);
    }
}