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
    private bool waitingForInput = false;
    private Event keyevent; //the event that occurs when a new key is being chosen.
    private KeyCode newKey = KeyCode.Numlock; //the refrence for the new keycode input that is added.
    [SerializeField]
    private Dropdown resolutionDropdown;
    [SerializeField]
    private Slider volume;

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

            //If an input has been pressed, stop other toggles being interactable
            if (zoomKeycodeRef.isOn == true)
            {
                waitingForInput = true;
                zoomKeycodeRef.interactable = true;
                sprintKeycodeRef.interactable = false;
                crouchKeycodeRef.interactable = false;
                grappleKeycodeRef.interactable = false;
                weaponSwapKeycodeRef.interactable = false;
                if (newKey != KeyCode.Numlock)
                {
                    //Set the key to the new key
                    optionsSingletonReference.zoomKeycode = newKey;
                    //Make sure no more inputs can be made
                    waitingForInput = false;
                    //Turn off this toggle as its no longer needed
                    zoomKeycodeRef.isOn = false;
                    //Set new key back to the default key
                    newKey = KeyCode.Numlock;
                }
            }
            else if (sprintKeycodeRef.isOn == true)
            {
                waitingForInput = true;
                zoomKeycodeRef.interactable = false;
                sprintKeycodeRef.interactable = true;
                crouchKeycodeRef.interactable = false;
                grappleKeycodeRef.interactable = false;
                weaponSwapKeycodeRef.interactable = false;
                if (newKey != KeyCode.Numlock)
                {
                    //Set the key to the new key
                    optionsSingletonReference.sprintKeycode = newKey;
                    //Make sure no more inputs can be made
                    waitingForInput = false;
                    //Turn off this toggle as its no longer needed
                    sprintKeycodeRef.isOn = false;
                    //Set new key back to the default key
                    newKey = KeyCode.Numlock;
                }
            }
            else if (crouchKeycodeRef.isOn == true)
            {
                waitingForInput = true;
                zoomKeycodeRef.interactable = false;
                sprintKeycodeRef.interactable = false;
                crouchKeycodeRef.interactable = true;
                grappleKeycodeRef.interactable = false;
                weaponSwapKeycodeRef.interactable = false;
                if (newKey != KeyCode.Numlock)
                {
                    //Set the key to the new key
                    optionsSingletonReference.crouchKeycode = newKey;
                    //Make sure no more inputs can be made
                    waitingForInput = false;
                    //Turn off this toggle as its no longer needed
                    crouchKeycodeRef.isOn = false;
                    //Set new key back to the default key
                    newKey = KeyCode.Numlock;
                }
            }
            else if (grappleKeycodeRef.isOn == true)
            {
                waitingForInput = true;
                zoomKeycodeRef.interactable = false;
                sprintKeycodeRef.interactable = false;
                crouchKeycodeRef.interactable = false;
                grappleKeycodeRef.interactable = true;
                weaponSwapKeycodeRef.interactable = false;
                if (newKey != KeyCode.Numlock)
                {
                    //Set the key to the new key
                    optionsSingletonReference.grappleKeycode = newKey;
                    //Make sure no more inputs can be made
                    waitingForInput = false;
                    //Turn off this toggle as its no longer needed
                    grappleKeycodeRef.isOn = false;
                    //Set new key back to the default key
                    newKey = KeyCode.Numlock;
                }
            }
            else if (weaponSwapKeycodeRef.isOn == true)
            {
                waitingForInput = true;
                zoomKeycodeRef.interactable = false;
                sprintKeycodeRef.interactable = false;
                crouchKeycodeRef.interactable = false;
                grappleKeycodeRef.interactable = false;
                weaponSwapKeycodeRef.interactable = true;
                if (newKey != KeyCode.Numlock)
                {
                    //Set the key to the new key
                    optionsSingletonReference.weaponSwapcode = newKey;
                    //Make sure no more inputs can be made
                    waitingForInput = false;
                    //Turn off this toggle as its no longer needed
                    weaponSwapKeycodeRef.isOn = false;
                    //Set new key back to the default key
                    newKey = KeyCode.Numlock;
                }
            }
            else
            {
                waitingForInput = false;
                zoomKeycodeRef.interactable = true;
                sprintKeycodeRef.interactable = true;
                crouchKeycodeRef.interactable = true;
                grappleKeycodeRef.interactable = true;
                weaponSwapKeycodeRef.interactable = true;
            }

            //Update the string of the button inputs
            zoomInputText.text = optionsSingletonReference.zoomKeycode.ToString();
            crouchInputText.text = optionsSingletonReference.crouchKeycode.ToString();
            grappleInputText.text = optionsSingletonReference.grappleKeycode.ToString();
            swapInputText.text = optionsSingletonReference.weaponSwapcode.ToString();
            sprintInputText.text = optionsSingletonReference.sprintKeycode.ToString();

            //Control the game resolution
            switch (resolutionDropdown.value)
            {
                case 0:
                    Screen.SetResolution(1920, 1080, Screen.fullScreen);
                    break;
                case 1:
                    Screen.SetResolution(1440, 900, Screen.fullScreen);
                    break;
                case 2:
                    Screen.SetResolution(1366, 768, Screen.fullScreen);
                    break;
                case 3:
                    Screen.SetResolution(1280, 720, Screen.fullScreen);
                    break;
            }

            //Control game volume
            AudioListener.volume = volume.value;
        }
    }

    private void OnGUI() //Used for detecting key changes
    {
        keyevent = Event.current;
        if (keyevent.isKey && waitingForInput == true)
        {
            newKey = keyevent.keyCode;
            waitingForInput = false;
        }
        else if (waitingForInput == true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                newKey = KeyCode.Mouse0;
                waitingForInput = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                newKey = KeyCode.Mouse1;
                waitingForInput = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                newKey = KeyCode.Mouse2;
                waitingForInput = false;
            }
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

    public void ResetButtonKeycodes()
    {
        optionsSingletonReference.sprintKeycode = KeyCode.LeftShift;
        optionsSingletonReference.zoomKeycode = KeyCode.Mouse2;
        optionsSingletonReference.grappleKeycode = KeyCode.Mouse1;
        optionsSingletonReference.crouchKeycode = KeyCode.LeftControl;
        optionsSingletonReference.weaponSwapcode = KeyCode.Q;
    }
}