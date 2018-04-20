//Author: James Murphy
//Purpose: Control the options menu in the main menu
//Requirements: Options menu and an options manager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.PostProcessing;

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
    private string optionsPath;
    private bool potentialExtraKey = false;

    private void Awake() //Get any required components if neccesary
    {
        optionsSingletonReference = GameObject.Find("OptionsManager").GetComponent<OptionsConfig>();

        //Read options file
        LoadGame();
        //Mark the initial load as completed
        initialLoadCompleted = true;
    }

    private void LoadGame() //Will load options if possible
    {
        //Get the file path of the leadboard file
        optionsPath = Environment.CurrentDirectory + @"\Options.txt";
        if (File.Exists(optionsPath))
        {
            int lineCount = 0;
            //Open the leaderboard file to read from.
            foreach (string line in File.ReadAllLines(optionsPath))
            {
                lineCount++;
                if (line != "")
                {
                    if (lineCount == 1)
                    {
                        if (line.ToString() == "True")
                        {
                            invertTogRef.isOn = true;
                        }
                        else
                        {
                            invertTogRef.isOn = false;
                        }
                    }
                    else if (lineCount == 2)
                    {
                        if (line.ToString() == "True")
                        {
                            windowedTogRef.isOn = true;
                        }
                        else
                        {
                            windowedTogRef.isOn = false;
                        }
                    }
                    else if (lineCount == 3)
                    {
                        if (line.ToString() == "True")
                        {
                            sprintTogRef.isOn = true;
                        }
                        else
                        {
                            sprintTogRef.isOn = false;
                        }
                    }
                    else if (lineCount == 4)
                    {
                        if (line.ToString() == "True")
                        {
                            zoomTogRef.isOn = true;
                        }
                        else
                        {
                            zoomTogRef.isOn = false;
                        }
                    }
                    else if (lineCount == 5)
                    {
                        if (line.ToString() == "True")
                        {
                            crouchTogRef.isOn = true;
                        }
                        else
                        {
                            crouchTogRef.isOn = false;
                        }
                    }
                    else if (lineCount == 6)
                    {
                        if (line.ToString() == "True")
                        {
                            vSyncTogRef.isOn = true;
                        }
                        else
                        {
                            vSyncTogRef.isOn = false;
                        }
                    }
                    else if (lineCount == 7) //crouch
                    {

                        OptionsConfig.inst.crouchKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                    }
                    else if (lineCount == 8) //grapple
                    {
                        OptionsConfig.inst.grappleKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                    }
                    else if (lineCount == 9) //sprint
                    {
                        OptionsConfig.inst.sprintKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                    }
                    else if (lineCount == 10) //doom
                    {
                        OptionsConfig.inst.zoomKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                    }
                    else if (lineCount == 11) //volume - has some problem
                    {
                        volume.value = int.Parse(line.ToString());
                    }
                    else if (lineCount == 12)  //res
                    {
                        resolutionDropdown.value = int.Parse(line.ToString());
                    }
                    else if (lineCount == 13)  //post P
                    {
                        if (line.ToString() == "True")
                        {
                            postProcessingTogRef.isOn = true;
                        }
                        else
                        {
                            postProcessingTogRef.isOn = false;
                        }
                    }
                    else if (lineCount == 14)
                    {
                        //float amount = int.Parse(line.ToString());
                        //OptionsConfig.inst.mouseX.value = amount;
                    }
                    else if (lineCount == 15)
                    {
                        //float amount = int.Parse(line.ToString());
                        //OptionsConfig.inst.mouseY.value = amount;
                    }
                    else if (lineCount == 16)
                    {
                        OptionsConfig.inst.weaponSwapcode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                    }
                }
            }
        }
    }
    public void SaveGame() //Save the current options into the options file
    {
        //Get the file path of the leadboard file
        optionsPath = Environment.CurrentDirectory + @"\Options.txt";


        //Check if the file path exists and if not create it
        // Create a file to write to.
        using (StreamWriter currentLine = File.CreateText(optionsPath))
        {
            //SAVES ALL THE TOGGLE VALUES
            currentLine.WriteLine(invertTogRef.isOn); //save the state of the camera invert 1
            currentLine.WriteLine(windowedTogRef.isOn);//save the state of the fullscreen 2 
            currentLine.WriteLine(sprintTogRef.isOn); //save the sprint toggle value 3
            currentLine.WriteLine(zoomTogRef.isOn); //save zoom toggle value 4 
            currentLine.WriteLine(crouchTogRef.isOn); //crouch toggle value 5
            currentLine.WriteLine(vSyncTogRef.isOn); // vsync toggle  value 6
            //SAVE THE KEYBIND VALUES
            currentLine.WriteLine(optionsSingletonReference.crouchKeycode.ToString()); // crouch keycode value 7
            currentLine.WriteLine(optionsSingletonReference.grappleKeycode.ToString()); // vsync toggle  value   8
            currentLine.WriteLine(optionsSingletonReference.sprintKeycode.ToString()); // vsync toggle  value   9
            currentLine.WriteLine(optionsSingletonReference.zoomKeycode.ToString()); // vsync toggle  value   10
            //SAVE VOLUME
            currentLine.WriteLine(volume.value.ToString()); // vsync toggle  value   11
                                                            //SAVE RES VALUE
            currentLine.WriteLine(resolutionDropdown.value.ToString()); // vsync toggle  value   12
            currentLine.WriteLine(postProcessingTogRef.isOn); //save whether the post proccessing is on or not 13
            currentLine.WriteLine(0); //saves the value of the mouse sensitibity onther x axis    14
            currentLine.WriteLine(0); //saves the value of the mouse sensitibity onther y axis    15
            currentLine.WriteLine(optionsSingletonReference.weaponSwapcode.ToString()); //116

        }
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

        if (potentialExtraKey == true && waitingForInput == true)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                potentialExtraKey = false;
                newKey = KeyCode.LeftShift;
            }
            else if(Input.GetKeyDown(KeyCode.RightShift))
            {
                potentialExtraKey = false;
                newKey = KeyCode.RightShift;
            }
        }
    }

    private void OnGUI() //Used for detecting key changes
    {
        keyevent = Event.current;
        if (keyevent.isKey && waitingForInput == true)
        {
            newKey = keyevent.keyCode;
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                newKey = KeyCode.LeftShift;
                waitingForInput = false;
                potentialExtraKey = false;
            }
            waitingForInput = false;
        }
        else if (waitingForInput == true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                newKey = KeyCode.Mouse0;
                waitingForInput = false;
                potentialExtraKey = false;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                newKey = KeyCode.Mouse1;
                waitingForInput = false;
                potentialExtraKey = false;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                newKey = KeyCode.Mouse2;
                waitingForInput = false;
                potentialExtraKey = false;
            }
            else
            {
                potentialExtraKey = true;
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