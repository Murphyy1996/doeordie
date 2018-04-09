using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.PostProcessing;

//Author: Kate Georgiou Date: 10/10/17 Purpose: holds all the different options functionality and also contains the code for inputting new keycodes.


public class OptionsConfig : MonoBehaviour
{
    public Transform menu; // a refrence to the transform of the canvas for looping purposes
    public static OptionsConfig inst; //setting this script up as a singleton
    [SerializeField]
    [Header("Saved Options")]
    private bool CamInvert = false;
    [SerializeField]
    private bool sprintTog, CrouchTog, buttonPress = false, zoomToggleBool, vysncbool, postPEnabled = true; //by default the camera is not invert.
    [Header("Component References")]
    [SerializeField]
    private Toggle invertCamToggle;
    [SerializeField]
    private Toggle crouchToggle, sprintToggle, fullscreen, vSync, zoomTog, crouchInputKey, sprintInputKey, zoomInputKey, grappleInputKey, postPToggle, weaponSwapInputkey; //a refrence to the toggles
    public Slider vol, mouseX, mouseY;
    public Text sprintKeyText, grappleKeyText, zoomKeyText, crouchKeyText, weaponSwapKeyText; //refrence to the buttons text so that it can be changed when a new input is chosen
    private Camera mainCamera;
    private FirstPersonCamera fpsScript;
    private Crouch crouchScript;
    private CharacterControllerMovement movement;
    private ZoomWeapon zoomScript;
    private Grapple grappleScript;
    private Shooting shoot;
    private int lastKnownScene = 0;
    public Dropdown resolution;
    //Variables for setting keycodes
    private KeyCode crouchKeycode = KeyCode.LeftControl, sprintKeycode = KeyCode.LeftShift, zoomKeycode = KeyCode.Mouse2, grappleKeycode = KeyCode.Mouse1, weaponSwapcode = KeyCode.Q;
    private KeyCode defaultCrouchKeycode = KeyCode.LeftControl, defaultSprintKeycode = KeyCode.LeftShift, defaultZoomKeycode = KeyCode.Mouse2, defaultGrappleKeycode = KeyCode.Mouse1, defaultWeaponCode = KeyCode.Q;
    private bool runOnceKeyInput = false, warningActivation = false;
    private KeyCode newKey; //the refrence for the new keycode input that is added.
    bool waitforKey = false; //whether or not we are waiting for a new key input (set to false by default)
    private Event keyevent; //the event that occurs when a new key is being chosen.
    private GameObject currentObjectUsingEventSystem;
    private string optionsPath;
    private PostProcessingBehaviour postPScript;
    [SerializeField]
    private float mouseXCurrent, mouseYCurrent, mouseXTarget, mouseYTarget;
    [SerializeField]
    private Image visual, inputWarning;
    [SerializeField]
    private List<KeyCode> currentKeycodes = new List<KeyCode>();
    [SerializeField]
    private Text warningText;



    private void Awake()
    {
        if (inst)
        {
            DestroyImmediate(gameObject);
        }
        else //Make this object the singleton
        {
            DontDestroyOnLoad(gameObject);
            inst = this;
        }
        //will need to go in to the movement script and change the input to these things i.e if (Input.Get(OptionsConfig.inst.crouch)) etc etc
        if (resolution != null)
        {
            // DetectCurrentResolution();
        }
    }

    // Use this for initialization
    private void Start()
    {
        //Don't destroy this object on load
        DontDestroyOnLoad(this);
        //Get required components
        LoadComponents();
        //Get the player if possible
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        vol.value = 1;
        Invoke("LoadOptions", 0.3f);
        Invoke("DelayedStart", 1f);
        inputWarning.enabled = false;
        warningText.enabled = false;
    }

    private void LoadComponents() //This method will get the components
    {
        if (invertCamToggle == null) //finds
        {
            invertCamToggle = GameObject.Find("InvertCam").GetComponent<Toggle>();
        }
        if (sprintToggle == null)  //finds
        {
            sprintToggle = GameObject.Find("SprintHold").GetComponent<Toggle>();
        }
        if (zoomTog == null)  //finds
        {
            zoomTog = GameObject.Find("ZoomTog").GetComponent<Toggle>();
        }
        if (fullscreen == null) //finds
        {
            fullscreen = GameObject.Find("Fullscreen").GetComponent<Toggle>();
        }
        if (vSync == null)
        {
            vSync = GameObject.Find("VSync").GetComponent<Toggle>();
        }
        if (vol == null)
        {
            vol = GameObject.Find("Volume").GetComponent<Slider>();
        }
        if (resolution == null)
        {
            resolution = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        }
        if (crouchToggle == null)
        {
            crouchToggle = GameObject.Find("CrouchHold").GetComponent<Toggle>();
        }
        if (postPToggle == null)
        {
            postPToggle = GameObject.Find("PostP").GetComponent<Toggle>();
        }
        //Get the toggles for the input objects here
        if (crouchInputKey == null)
        {

            crouchInputKey = GameObject.Find("CrouchInput").GetComponent<Toggle>();
            crouchKeyText = crouchInputKey.gameObject.GetComponentInChildren<Text>();
            crouchKeyText.text = crouchKeycode.ToString();

        }
        if (sprintKeyText == null)
        {
            sprintInputKey = GameObject.Find("SprintInput").GetComponent<Toggle>();
            sprintKeyText = sprintInputKey.gameObject.GetComponentInChildren<Text>();
            sprintKeyText.text = sprintKeycode.ToString();
        }
        if (zoomKeyText == null)
        {
            zoomInputKey = GameObject.Find("ZoomInput").GetComponent<Toggle>();
            zoomKeyText = zoomInputKey.gameObject.GetComponentInChildren<Text>();
            zoomKeyText.text = zoomKeycode.ToString();
        
        }
        if (grappleKeyText == null)
        {
            grappleInputKey = GameObject.Find("GrappleInput").GetComponent<Toggle>();
            grappleKeyText = grappleInputKey.gameObject.GetComponentInChildren<Text>();
            grappleKeyText.text = grappleKeycode.ToString();
        }
        if (weaponSwapKeyText == null)
        {
            weaponSwapInputkey = GameObject.Find("WeaponInput").GetComponent<Toggle>();
            weaponSwapKeyText = weaponSwapInputkey.gameObject.GetComponentInChildren<Text>();
            weaponSwapKeyText.text = weaponSwapcode.ToString();
        }
       
        if (menu == null)
        {
            menu = GameObject.Find("Options").GetComponent<Transform>();
        }
        if (fpsScript == null && SceneManager.GetActiveScene().buildIndex != 0) //when I add this build index bit in - breaks options - HERE MURP
        {
            fpsScript = Camera.main.GetComponent<FirstPersonCamera>();
        }
        if (fpsScript != null)
        {
            mouseXCurrent = fpsScript.GetCurrentXSensitivity();
            mouseYCurrent = fpsScript.GetCurrentYSensitivity();
        }
        if (mouseX == null)
        {
            GameObject.Find("MouseXSensSlide");
        }
        if (mouseY == null)
        {
            GameObject.Find("MouseYSensSlider");
        }
        if (inputWarning == null)
        {
            GameObject.Find("InputWarning");
        }
        //if (currentKeycodes == null)
       // {
           

       // }
        if (warningText == null)
        {
            GameObject.Find("WarningText");
        }
    }

    private void DelayedStart()
    {
        currentKeycodes.Add(crouchKeycode);
        currentKeycodes.Add(sprintKeycode);
        currentKeycodes.Add(zoomKeycode);
        currentKeycodes.Add(grappleKeycode);
        currentKeycodes.Add(weaponSwapcode);
    }
    // Update is called once per frame
    private void Update()
    {
        //Force this singleton to get references on the main menu as it seems to have an issue
        if (SceneManager.GetActiveScene().buildIndex == 0 && menu == null)
        {
            LoadComponents();
        }
        //The below code controls the rest of the options
        if (mainCamera == null) //Check if the camera is in the scene
        {
            mainCamera = Camera.main;
        }
        else //Control the relevent toggles and references
        {
            //Have the option controls above the if in order to allow them to set defaults
            ControlCameraInvertToggle();
            ControlCrouchToggle();
            ControlSprintToggle();
            ControlZoomToggle();
            ControlVsyncToggle();
            ControlVolumeSlider();
            //The control the input keycodes specifically
            ControlCrouchKeycode();
            ControlSprintKeycode();
            ControlZoomKeycode();
            ControlGrappleKeycode();
          
            PostProcessingToggle();
            ControlMouseSens();
            ControlWeaponSwapKeycode();
            /*if (currentKeycodes == null)
            {
                currentKeycodes.Add(crouchKeycode);
                currentKeycodes.Add(sprintKeycode);
                currentKeycodes.Add(zoomKeycode);
                currentKeycodes.Add(grappleKeycode);
                currentKeycodes.Add(weaponSwapcode);
                Debug.Log("adding to the list2");
            }*/
            if (warningActivation == true)
            {
                StopAllCoroutines();
                StartCoroutine("LoadWarning");
                warningActivation = false;

            }
            //Update the last known scene when all components references have been got
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene && resolution != null)
            {
                lastKnownScene = SceneManager.GetActiveScene().buildIndex;
            }
        }
    }

    private void OnLevelWasLoaded() //Run code when the scene loads
    {
        //If you are on any scene other than the main menu
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            //This will make sure the options are saved if the scene has simply been restarted
            if (SceneManager.GetActiveScene().buildIndex == lastKnownScene)
            {
                lastKnownScene = 9999;
            }
        }
        //get all the components on the options menu
        Invoke("LoadComponents", 0.2f);
    }

    public void ChangeFullscreenSetting()
    {
        //Change the fullscreen value to the opposite of the current value
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void DetectCurrentResolution() //Detect the current resolution and display it on the resolution dropdown
    {
        string currentRes = Screen.width + "," + Screen.height;

        //If the game recognises the current resolution, set the dropdown to match the current res to avoid user confusion
        switch (currentRes)
        {
            case "1920,1080":
                resolution.value = 0;
                break;
            case "1440,900":
                resolution.value = 1;
                break;
            case "1366, 766":
                resolution.value = 2;
                break;
            case "1280, 720":
                resolution.value = 3;
                break;
        }
        //Now get the already selected settings
        DetectCurrentSettings();
    }

    private void DetectCurrentSettings() //When ran this will detect the current settings and check the values match the current values
    {
        //Get the vsync toggle box
        vSync = GameObject.Find("VSync").GetComponent<Toggle>();
        //Set the vsync toggle the current vsync option
        if (QualitySettings.vSyncCount == 0 || QualitySettings.vSyncCount == 1)
        {
            vSync.isOn = false;
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            vSync.isOn = true;
            QualitySettings.vSyncCount = 2;
        }
        //Make sure the dropdown selects the current anti aliasing value

    }

    public void ChangeResolutionDropdown()
    {
        switch (resolution.value)
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
    }

    public void InputReturnToDefaultButton() //Returns all input buttons to defaults
    {
        //Return the keycodes
        crouchKeycode = defaultCrouchKeycode;
        grappleKeycode = defaultGrappleKeycode;
        sprintKeycode = defaultSprintKeycode;
        zoomKeycode = defaultZoomKeycode;
        weaponSwapcode = defaultWeaponCode;
        //Update the texts
        crouchKeyText.text = crouchKeycode.ToString();
        grappleKeyText.text = grappleKeycode.ToString();
        sprintKeyText.text = sprintKeycode.ToString();
        zoomKeyText.text = zoomKeycode.ToString();
        weaponSwapKeyText.text = weaponSwapcode.ToString();

        //re-update the list so that it can do the warning
        currentKeycodes.Clear();
        DelayedStart();
        warningActivation = false;
        inputWarning.enabled = false;
        warningText.enabled = false;
    }

    //The Control for options that save accross scenes etc

    private void ControlCameraInvertToggle() //This will control the invert toggle
    {
        //If the toggle exists
        if (invertCamToggle != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                invertCamToggle.isOn = CamInvert;
            }
            else //Allow for updating and changing on runtime
            {
                //Get the fps script reference if needed / possible
                if (mainCamera != null & fpsScript == null)
                {
                    fpsScript = mainCamera.GetComponent<FirstPersonCamera>();
                }
                //Set the bool accordingly
                if (invertCamToggle.isOn == true)
                {
                    CamInvert = true;
                }
                else
                {
                    CamInvert = false;
                }
                //Apply the bool
                if (fpsScript != null)
                {
                    fpsScript.SetInvertedCamera(CamInvert);
                }
            }
        }
    }

    private void ControlCrouchToggle() //This will control the crouch toggle
    {
        //If the toggle exists
        if (crouchToggle != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                crouchToggle.isOn = CrouchTog;
                Debug.Log("pressing button");


            }
            else //Allow for updating and changing on runtime
            {
                //Get the crouch script reference if needed / possible
                if (crouchScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        crouchScript = player.GetComponent<Crouch>();
                    }
                }
                //Set the bool accordingly
                if (crouchToggle.isOn == true)
                {
                    CrouchTog = true;
                }
                else
                {
                    CrouchTog = false;
                }
                //Apply the bool
                if (crouchScript != null)
                {
                    crouchScript.SetCrouchToggleMode(CrouchTog);
                }
            }
        }
    }
    private void ControlSprintToggle() //This will control the sprint toggle
    {
        //If the toggle exists
        if (sprintToggle != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                sprintToggle.isOn = sprintTog;
            }
            else //Allow for updating and changing on runtime
            {
                //Get the crouch script reference if needed / possible
                if (movement == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        movement = player.GetComponent<CharacterControllerMovement>();
                    }
                }
                //Set the bool accordingly
                if (sprintToggle.isOn == true)
                {
                    sprintTog = true;
                }
                else
                {
                    sprintTog = false;
                }
                //Apply the bool
                if (movement != null)
                {
                    movement.SetSprintToggleBool(sprintTog);
                }
            }
        }
    }
    private void ControlZoomToggle() //This will control the zoom toggle
    {
        //If the toggle exists
        if (zoomTog != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                zoomTog.isOn = zoomToggleBool;
            }
            else //Allow for updating and changing on runtime
            {
                //Get the crouch script reference if needed / possible
                if (zoomScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        zoomScript = player.GetComponent<ZoomWeapon>();
                    }
                }
                //Set the bool accordingly
                if (zoomTog.isOn == true)
                {
                    zoomToggleBool = true;
                }
                else
                {
                    zoomToggleBool = false;
                }
                //Apply the bool
                if (zoomScript != null)
                {
                    zoomScript.SetZoomToggle(zoomToggleBool);
                }
            }
        }
    }
    private void ControlVsyncToggle() //This will control the vsync toggle
    {
        //If the toggle exists
        if (vSync != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                vSync.isOn = vysncbool;
            }
            else //Allow for updating and changing on runtime
            {


                //Set the bool accordingly
                if (vSync.isOn == true)
                {
                    vysncbool = true;
                }
                else
                {
                    vysncbool = false;
                }

                //apply the setting
                if (vysncbool == true)
                {
                    QualitySettings.vSyncCount = 1;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                }

            }
        }
    }
    private void ControlVolumeSlider() //This will control the volume slide
    {
        if (vol != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                vol.value = AudioListener.volume;
            }
            else //Allow for updating and changing on runtime
            {
                //Apply the bool
                if (vol != null)
                {
                    AudioListener.volume = vol.value;
                }
            }
        }
    }
    private void ControlFullScreenToggle() //This will control the crouch toggle
    {
        //If the toggle exists
        if (vSync != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                vSync.isOn = vysncbool;
            }
            else //Allow for updating and changing on runtime
            {
                //Set the bool accordingly
                if (vSync.isOn == true)
                {
                    vysncbool = true;
                }
                else
                {
                    vysncbool = false;
                }

                //apply the setting
                if (vysncbool == true)
                {
                    QualitySettings.vSyncCount = 2;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                }
            }
        }
    }
    private void OnGUI() //Used for detecting key changes
    {
        keyevent = Event.current;
       // Debug.LogError(" RUNNING GUI");
        if (keyevent.isKey && waitforKey == true)
        {
           // Debug.LogError(" RUNNING key events");
            newKey = keyevent.keyCode;
            
            foreach (KeyCode key in currentKeycodes)
            {
                
                if (newKey == key)
                {
                    warningActivation = true;
                    //needs to update and add the new keys to the list but when i do it normal way it no work
                    
                }
             
            }
            waitforKey = false;
            currentKeycodes.Clear();
            DelayedStart();
        }
        else if (waitforKey == true) //Mouse inputs and other keys not supported by the keyevent manager
        {
            //Debug.LogError("not RUNNING key events");
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                newKey = KeyCode.Mouse0;
                waitforKey = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                newKey = KeyCode.Mouse1;
                waitforKey = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                newKey = KeyCode.Mouse2;
                waitforKey = false;
            }
        }


    }
    private void ControlCrouchKeycode() //This will allow you to set the crouch keycode
    {
        //If the toggle exists
        if (crouchInputKey != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                crouchInputKey.isOn = false;
                crouchKeyText.text = crouchKeycode.ToString();
            }
            else //Allow for updating and changing on runtime
            {
                //Get the crouch script reference if needed / possible
                if (crouchScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        crouchScript = player.GetComponent<Crouch>();
                    }
                }
                //Set the bool accordingly
                if (crouchKeyText != null && crouchInputKey.isOn == true)
                {
                    if (currentObjectUsingEventSystem == null || currentObjectUsingEventSystem == crouchInputKey.gameObject)
                    {
                        //Also set the colour
                        //Run once to set up the check for key
                        if (runOnceKeyInput == false)
                        {
                            currentObjectUsingEventSystem = crouchInputKey.gameObject;
                            runOnceKeyInput = true;
                            waitforKey = true;
                        }
                        else if (waitforKey == false)
                        {
                            Debug.Log(newKey);
                            crouchInputKey.isOn = false;
                            runOnceKeyInput = false;
                            crouchKeycode = newKey;
                            crouchKeyText.text = newKey.ToString();

                            //Let the game know the event system is free
                            currentObjectUsingEventSystem = null;
                        }
                    }
                    else
                    {
                        crouchInputKey.isOn = false;
                    }
                }
                //Apply the bool
                if (crouchScript != null)
                {
                    crouchScript.SetCrouchKeycode(crouchKeycode);
                    crouchKeyText.text = crouchKeycode.ToString();
                }
            }
        }
    }
    private void ControlSprintKeycode() //This will allow you to set the sprint keycode
    {
        //If the toggle exists
        if (sprintInputKey != null)
        {
            sprintKeyText.text = sprintKeycode.ToString();
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                sprintInputKey.isOn = false;
                
            }
            else //Allow for updating and changing on runtime
            {
                //Get the crouch script reference if needed / possible
                if (movement == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        movement = player.GetComponent<CharacterControllerMovement>();
                    }
                }
                //Set the bool accordingly
                if (sprintInputKey.isOn == true)
                {
                    if (currentObjectUsingEventSystem == null || currentObjectUsingEventSystem == sprintInputKey.gameObject)
                    {
                        //Also set the colour
                        //Run once to set up the check for key
                        if (runOnceKeyInput == false)
                        {
                            currentObjectUsingEventSystem = sprintInputKey.gameObject;
                            runOnceKeyInput = true;
                            waitforKey = true;
                        }
                        else if (waitforKey == false)
                        {
                            sprintInputKey.isOn = false;
                            runOnceKeyInput = false;
                            sprintKeycode = newKey;
                            sprintKeyText.text = newKey.ToString();
                            //Let the game know the event system is free
                            currentObjectUsingEventSystem = null;
                        }
                    }
                    else
                    {
                        sprintInputKey.isOn = false;
                    }
                }
                //Apply the bool
                if (movement != null)
                {
                    movement.SetSprintKeycode(sprintKeycode);
                    sprintKeyText.text = sprintKeycode.ToString();
                }
            }
        }
    }
    private void ControlZoomKeycode() //This will allow you to set the zoom keycode
    {
        //If the toggle exists
        if (zoomInputKey != null)
        {
            zoomKeyText.text = zoomKeycode.ToString();
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                zoomInputKey.isOn = false;
            }
            else //Allow for updating and changing on runtime
            {
                //Get the zoom script reference if needed / possible
                if (zoomScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        zoomScript = player.GetComponent<ZoomWeapon>();
                    }
                }
                //Set the bool accordingly
                if (zoomInputKey.isOn == true)
                {
                    if (currentObjectUsingEventSystem == null || currentObjectUsingEventSystem == zoomInputKey.gameObject)
                    {
                        //Also set the colour
                        //Run once to set up the check for key
                        if (runOnceKeyInput == false)
                        {
                            currentObjectUsingEventSystem = zoomInputKey.gameObject;
                            runOnceKeyInput = true;
                            waitforKey = true;
                        }
                        else if (waitforKey == false)
                        {
                            zoomInputKey.isOn = false;
                            runOnceKeyInput = false;
                            zoomKeycode = newKey;
                            zoomKeyText.text = newKey.ToString();
                            //Let the game know the event system is free
                            currentObjectUsingEventSystem = null;
                        }
                    }
                    else
                    {
                        zoomInputKey.isOn = false;
                    }
                }
                //Apply the bool
                if (zoomScript != null)
                {
                    zoomScript.SetZoomButton(zoomKeycode);
                    zoomKeyText.text = zoomKeycode.ToString();
                }
            }
        }
    }
    private void ControlGrappleKeycode() //This will allow you to set the grapple keycode
    {
        //If the toggle exists
        if (grappleInputKey != null)
        {
            grappleKeyText.text = grappleKeycode.ToString();
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                grappleInputKey.isOn = false;
              
            }
            else //Allow for updating and changing on runtime
            {
                //Get the zoom script reference if needed / possible
                if (grappleScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        grappleScript = player.GetComponent<Grapple>();
                    }
                }
                //Set the bool accordingly
                if (grappleInputKey.isOn == true)
                {
                    if (currentObjectUsingEventSystem == null || currentObjectUsingEventSystem == grappleInputKey.gameObject)
                    {
                        //Also set the colour
                        //Run once to set up the check for key
                        if (runOnceKeyInput == false)
                        {
                            currentObjectUsingEventSystem = grappleInputKey.gameObject;
                            runOnceKeyInput = true;
                            waitforKey = true;
                        }
                        else if (waitforKey == false)
                        {
                            grappleInputKey.isOn = false;
                            runOnceKeyInput = false;
                            grappleKeycode = newKey;
                            grappleKeyText.text = newKey.ToString();
                            //Let the game know the event system is free
                            currentObjectUsingEventSystem = null;
                        }
                    }
                    else
                    {
                        grappleInputKey.isOn = false;
                    }
                }
                //Apply the bool
                if (grappleScript != null)
                {
                    grappleScript.SetGrappleKeycode(grappleKeycode);
                    grappleKeyText.text = grappleKeycode.ToString();
                }
            }
        }
    }
    private void ControlWeaponSwapKeycode()
    {
        //If the toggle exists
        if (weaponSwapInputkey != null)
        {
            weaponSwapKeyText.text = weaponSwapcode.ToString();
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                weaponSwapInputkey.isOn = false;
                
            }
            else //Allow for updating and changing on runtime
            {
                //Get the zoom script reference if needed / possible
                if (shoot == null)
                {
                    GameObject player = GameObject.Find("Player");
                    if (player != null)
                    {
                        shoot = player.GetComponent<Shooting>();
                    }
                }
                //Set the bool accordingly
                if (weaponSwapInputkey.isOn == true)
                {
                    if (currentObjectUsingEventSystem == null || currentObjectUsingEventSystem == weaponSwapInputkey.gameObject)
                    {
                        //Also set the colour
                        //Run once to set up the check for key
                        if (runOnceKeyInput == false)
                        {
                            currentObjectUsingEventSystem = weaponSwapInputkey.gameObject;
                            runOnceKeyInput = true;
                            waitforKey = true;
                        }
                        else if (waitforKey == false)
                        {
                            weaponSwapInputkey.isOn = false;
                            runOnceKeyInput = false;
                            weaponSwapcode = newKey;
                            weaponSwapKeyText.text = newKey.ToString();
                            //Let the game know the event system is free
                            currentObjectUsingEventSystem = null;
                        }
                    }
                    else
                    {
                        weaponSwapInputkey.isOn = false;
                    }
                }
                //Apply the bool
                if (shoot != null)
                {
                    shoot.SetWeaponKeycode(weaponSwapcode);
                    weaponSwapKeyText.text = weaponSwapcode.ToString();
                }
            }
        }
    }
    public void SaveOptions() //Save the current options into the options file
    {
        //Get the file path of the leadboard file
        optionsPath = Environment.CurrentDirectory + @"\Options.txt";


        //Check if the file path exists and if not create it
        // Create a file to write to.
        using (StreamWriter currentLine = File.CreateText(optionsPath))
        {
            //SAVES ALL THE TOGGLE VALUES
            currentLine.WriteLine(OptionsConfig.inst.CamInvert); //save the state of the camera invert 1
            currentLine.WriteLine(OptionsConfig.inst.fullscreen);//save the state of the fullscreen 2 
            currentLine.WriteLine(OptionsConfig.inst.sprintTog); //save the sprint toggle value 3
            currentLine.WriteLine(OptionsConfig.inst.zoomToggleBool); //save zoom toggle value 4 
            currentLine.WriteLine(OptionsConfig.inst.CrouchTog); //crouch toggle value 5
            currentLine.WriteLine(OptionsConfig.inst.vSync); // vsync toggle  value 6
            //SAVE THE KEYBIND VALUES
            currentLine.WriteLine(OptionsConfig.inst.crouchKeycode.ToString()); // crouch keycode value 7
            currentLine.WriteLine(OptionsConfig.inst.grappleKeycode.ToString()); // vsync toggle  value   8
            currentLine.WriteLine(OptionsConfig.inst.sprintKeycode.ToString()); // vsync toggle  value   9
            currentLine.WriteLine(OptionsConfig.inst.zoomKeycode.ToString()); // vsync toggle  value   10
            //SAVE VOLUME
            currentLine.WriteLine(OptionsConfig.inst.vol.value.ToString()); // vsync toggle  value   11
         //   Debug.Log("Saved as" + vol.value);
            //SAVE RES VALUE
            currentLine.WriteLine(OptionsConfig.inst.resolution.value.ToString()); // vsync toggle  value   12

            currentLine.WriteLine(OptionsConfig.inst.postPEnabled); //save whether the post proccessing is on or not 13

           

            currentLine.WriteLine(OptionsConfig.inst.mouseX.value.ToString()); //saves the value of the mouse sensitibity onther x axis    14
            currentLine.WriteLine(OptionsConfig.inst.mouseY.value.ToString()); //saves the value of the mouse sensitibity onther y axis    15
            Debug.Log("mouse X saved as" + mouseX.value);

            currentLine.WriteLine(OptionsConfig.inst.weaponSwapcode.ToString()); //116

        }
        Invoke("DelayedStart", 1f);
    }

    private void LoadOptions() //Will load leaderboard if possible
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
                            OptionsConfig.inst.invertCamToggle.isOn = true;
                        }
                        else
                        {
                            OptionsConfig.inst.invertCamToggle.isOn = false;
                        }
                    }
                    else if (lineCount == 2)
                    {
                        if (line.ToString() == "True")
                        {
                            OptionsConfig.inst.fullscreen.isOn = true;
                        }
                        else
                        {
                            OptionsConfig.inst.fullscreen.isOn = false;
                        }
                    }
                    else if (lineCount == 3)
                    {
                        if (line.ToString() == "True")
                        {
                            OptionsConfig.inst.sprintToggle.isOn = true;
                        }
                        else
                        {
                            OptionsConfig.inst.sprintToggle.isOn = false;
                        }
                    }
                    else if (lineCount == 4)
                    {
                        if (line.ToString() == "True")
                        {
                            OptionsConfig.inst.zoomTog.isOn = true;
                        }
                        else
                        {
                            OptionsConfig.inst.zoomTog.isOn = false;
                        }
                    }
                    else if (lineCount == 5)
                    {
                        if (line.ToString() == "True")
                        {
                            OptionsConfig.inst.crouchToggle.isOn = true;
                        }
                        else
                        {
                            OptionsConfig.inst.crouchToggle.isOn = false;
                        }
                    }
                    else if (lineCount == 6)
                    {
                        if (line.ToString() == "True")
                        {
                            OptionsConfig.inst.vSync.isOn = true;
                        }
                        else
                        {
                            OptionsConfig.inst.vSync.isOn = false;
                        }
                    }
                    else if (lineCount == 7) //crouch
                    {

                        OptionsConfig.inst.crouchKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                        OptionsConfig.inst.crouchKeyText.text = OptionsConfig.inst.crouchKeycode.ToString();
                    }
                    else if (lineCount == 8) //grapple
                    {
                        OptionsConfig.inst.grappleKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                        OptionsConfig.inst.grappleKeyText.text = OptionsConfig.inst.grappleKeyText.ToString();
                    }
                    else if (lineCount == 9) //sprint
                    {
                        OptionsConfig.inst.sprintKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                        OptionsConfig.inst.sprintKeyText.text = OptionsConfig.inst.sprintKeyText.ToString();
                    }
                    else if (lineCount == 10) //doom
                    {
                        OptionsConfig.inst.zoomKeycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                        OptionsConfig.inst.zoomKeyText.text = OptionsConfig.inst.zoomKeyText.ToString();
                    }

                    else if (lineCount == 11) //volume - has some problem
                    {
                      //  float counterNumber = int.Parse(line.ToString());
                      //  OptionsConfig.inst.vol.value = counterNumber;
                        OptionsConfig.inst.vol.value = int.Parse(line.ToString());
                    }
                    else if (lineCount == 12)  //res
                    {
                        OptionsConfig.inst.resolution.value = int.Parse(line.ToString());
                    }
                    else if (lineCount == 13)  //post P
                    {
                        if (line.ToString() == "True")
                        {
                            OptionsConfig.inst.postPToggle.isOn = true;
                        }
                        else
                        {
                            OptionsConfig.inst.postPToggle.isOn = false;
                        }
                    }
                   
                    else if (lineCount == 14)
                    {
                        float amount = int.Parse(line.ToString());
                        OptionsConfig.inst.mouseX.value = amount;
                    }
                    else if (lineCount == 15)
                    {
                        float amount = int.Parse(line.ToString());
                        OptionsConfig.inst.mouseY.value = amount;
                    }
                    else if (lineCount == 16)
                    {
                        OptionsConfig.inst.weaponSwapcode = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.ToString());
                        OptionsConfig.inst.weaponSwapKeyText.text = OptionsConfig.inst.weaponSwapKeyText.ToString();


                    }


                }
            }
        }
    }

    public void PostProcessingToggle()
    {
        //If the toggle exists
        if (postPToggle != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                //Make sure it equals the default
                postPToggle.isOn = postPEnabled;
            }
            else //Allow for updating and changing on runtime
            {
                //Get the post processing script reference if needed / possible
                if (postPScript == null && SceneManager.GetActiveScene().buildIndex != 0)
                {
                    if (Camera.main != null)
                    {
                        postPScript = Camera.main.GetComponent<PostProcessingBehaviour>();
                    }
                }

                //Set the bool accordingly
                if (postPToggle.isOn == true)
                {
                    postPEnabled = true;
                }
                else
                {
                    postPEnabled = false;
                }

                if (postPScript != null)
                {
                    //apply the setting
                    if (postPEnabled == true)
                    {
                        postPScript.enabled = true;
                    }
                    else
                    {
                        postPScript.enabled = false;
                    }
                }
            }
        }
    }

   

  private void ControlMouseSens()
    {
        // StopAllCoroutines();
        // StartCoroutine(ApplyMouseSens());
   
        
        if (mouseY != null && mouseX != null)
        {
            //If on a new scene set the value of the  ui so it can transfer accross scene
            if (SceneManager.GetActiveScene().buildIndex != lastKnownScene)
            {
                
                //Make sure it equals the default
                mouseY.value = mouseYCurrent;
                mouseX.value = mouseXCurrent;
                //Save the defaults
                mouseYCurrent = mouseY.value;
                mouseXCurrent = mouseX.value;
            }
            else //Allow for updating and changing on runtime
            {
                if (fpsScript != null)
                {
                    
                    fpsScript.ChangeXSensitivity(mouseX.value);
                    fpsScript.ChangeYSensitivity(mouseY.value);
                 //   Debug.Log("changing sensitivty");
                }
            }
        }
        else //Get the components
        {
           // try
           // {
                mouseX = GameObject.Find("MouseXSensSlide").GetComponent<Slider>();
                mouseY = GameObject.Find("MouseYSensSlider").GetComponent<Slider>();
           // }
          //  catch
          //  {
                print("retrieving component");
           // }
        }
    }

    private IEnumerator LoadWarning()
    {
        inputWarning.enabled = true;
        warningText.enabled = true;
        yield return new WaitForSeconds(2f);
        inputWarning.enabled = false;
        warningText.enabled = false;
      
     //   Debug.Log("Removing warning");
    }

}
