using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    //Declare this singleton
    public static MouseManager singleton;
    private bool mouseVisible = true, mouseLocked = false;

    private void Awake() //Initialise this singleton
    {
        singleton = this;
        this.gameObject.name = "Mouse Manager";
        ControlMouse(false, true, false);
    }

    public void ControlMouse(bool displayMouse, bool lockMouse, bool trueFreeMouse)
    {
        //Track the new status of the mouse
        mouseVisible = displayMouse;
        mouseLocked = lockMouse;
        //Control the visibility of the cursor
        Cursor.visible = displayMouse;
        //Control whether the cursor is locked or not
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        //Overwrite prior settings if true free mouse is turned on
        if (trueFreeMouse)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public bool ReturnIfMouseVisible()
    {
        return mouseVisible;
    }

    public bool ReturnIfMouseLocked()
    {
        return mouseLocked;
    }


}
