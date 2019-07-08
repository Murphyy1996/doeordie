using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Declare this singleton
    public static InputManager singleton;
    //Contain the various keycodes
    [Header("Movement Keymaps")]
    public string forwardBackwardAxis = "ForwardBackward";
    public string leftRightAxis = "LeftRight";
    public KeyCode crouchKey = KeyCode.C, jumpKey = KeyCode.Space;
    [Header("Camera Keymaps")]
    public string lookUpDown = "LookUpDown";
    public string lookLeftRight = "LookLeftRight";
    [Header("Action Keymaps")]
    public KeyCode interactionKey = KeyCode.Q;
    public KeyCode grappleKey = KeyCode.E, teleportKey = KeyCode.F;

    private void Awake() //Initialise this singleton
    {
        singleton = this;
        this.gameObject.name = "Input Manager";
        DontDestroyOnLoad(this.gameObject);
    }
}
