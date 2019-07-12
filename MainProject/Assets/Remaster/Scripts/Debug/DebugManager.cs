using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    //Declare the debug manager
    public static DebugManager singleton;
    private InputManager inputManager;
    private MouseManager mouseManager;
    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    //Configuration settings for this manager
    [Header("Configuration")] [SerializeField] private bool debugAllowed = false;
    //Non inspector variables for checking what mode the player is in
    private bool freeMouseEnabled = false;

    private void Awake()
    {
        //Initialise this singleton
        singleton = this;
        //Get the require managers etc
        inputManager = InputManager.singleton;
        mouseManager = MouseManager.singleton;
        //Find the player and get relevant scripts
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        playerMovement = foundPlayer.GetComponent<PlayerMovement>();
        playerCamera = foundPlayer.GetComponent<PlayerCamera>();

    }

    private void Update() //Options for controlling different settings
    {
        //Only allow any of this is debug options are enabled
        if (debugAllowed)
        {
            //Option for toggling mouse free look
            if (Input.GetKeyUp(inputManager.mouseFreeLook))
            {
                //Track if free mouse is enabled
                freeMouseEnabled = !freeMouseEnabled;
                //Turn free mouse off 
                mouseManager.ControlMouse(freeMouseEnabled, true, freeMouseEnabled);

            }
            //Option for toggling camera focus
            if (Input.GetKeyUp(inputManager.camDebugFocusToggle))
            {
                playerCamera.cameraEnabled = !playerCamera.cameraEnabled;
            }
        }
    }



}
