using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TextUpdated : MonoBehaviour
{

    [SerializeField]
    private TextMesh crouchInstructions, zoom, grapple, sprint, slide;
    private Crouch crouchScript;
    private CharacterControllerMovement thisccMovement;


    // Use this for initialization
    void Start()
    {
        crouchInstructions = GameObject.Find("CrouchInstructions").GetComponent<TextMesh>();
        grapple = GameObject.Find("Grapple Instruct").GetComponent<TextMesh>();
        zoom = GameObject.Find("PistolInstruct").GetComponent<TextMesh>();
        slide = GameObject.Find("Toggle Instruct").GetComponent<TextMesh>();
        sprint = GameObject.Find("Sprint Instruct").GetComponent<TextMesh>();
        crouchScript = GameObject.Find("Player").GetComponent<Crouch>();
        thisccMovement = GameObject.Find("Player").GetComponent<CharacterControllerMovement>();

    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            if (OptionsConfig.inst != null)
            {
                if (crouchInstructions != null && OptionsConfig.inst != null)
                {
                    string tempKey;
                    if (OptionsConfig.inst != null)
                    {
                        tempKey = OptionsConfig.inst.crouchKeyText.text;
                    }
                    else
                    {
                        tempKey = "C";
                    }
                    if (OptionsConfig.inst.crouchKeyText.text == "Mouse1")
                    {
                        tempKey = "RIGHT CLICK";
                    }
                    else if (OptionsConfig.inst.crouchKeyText.text == "Mouse0")
                    {
                        tempKey = "LEFT CLICK";
                    }
                    else if (OptionsConfig.inst.crouchKeyText.text == "Mouse2")
                    {
                        tempKey = "MIDDLE MOUSE";
                    }

                    if (crouchScript.ReturnIfInToggleMode() == true)
                    {
                        crouchInstructions.text = "Press " + tempKey + " to crouch.";
                    }
                    else
                    {
                        crouchInstructions.text = "Hold " + tempKey + " to crouch.";
                    }
                }
                else
                {
                    GameObject.Find("CrouchInstructions").GetComponent<TextMesh>();
                }

                if (grapple != null)
                {
                    string tempKey = OptionsConfig.inst.grappleKeyText.text;
                    if (OptionsConfig.inst.grappleKeyText.text == "Mouse1")
                    {
                        tempKey = "RIGHT CLICK";
                    }
                    else if (OptionsConfig.inst.grappleKeyText.text == "Mouse0")
                    {
                        tempKey = "LEFT CLICK";
                    }
                    else if (OptionsConfig.inst.grappleKeyText.text == "Mouse2")
                    {
                        tempKey = "MIDDLE MOUSE";
                    }
                    grapple.text = "Walk over the GRAPPLE to pick it up." + Environment.NewLine + Environment.NewLine + tempKey + " to GRAPPLE onto surfaces and quickly" + Environment.NewLine + "travel long distances in a short time." + Environment.NewLine + Environment.NewLine + "Press SPACE to get out of GRAPPLE.";
                }
                else
                {
                    GameObject.Find("Grapple Instruct").GetComponent<TextMesh>();
                }

                if (zoom != null)
                {
                    string tempKey = OptionsConfig.inst.zoomKeyText.text;
                    if (OptionsConfig.inst.zoomKeyText.text == "Mouse1")
                    {
                        tempKey = "RIGHT CLICK";
                    }
                    else if (OptionsConfig.inst.zoomKeyText.text == "Mouse0")
                    {
                        tempKey = "LEFT CLICK";
                    }
                    else if (OptionsConfig.inst.zoomKeyText.text == "Mouse2")
                    {
                        tempKey = "MIDDLE MOUSE";
                    }
                    zoom.text = "Walk over the Pistol to pick it up" + Environment.NewLine + "LEFT CLICK to SHOOT," + Environment.NewLine + tempKey + " to AIM.";
                }
                else
                {
                    GameObject.Find("PistolInstruct").GetComponent<TextMesh>();
                }

                if (slide != null)
                {
                    string tempKey = OptionsConfig.inst.crouchKeyText.text;
                    if (OptionsConfig.inst.crouchKeyText.text == "Mouse1")
                    {
                        tempKey = "Right Mouse";
                    }
                    else if (OptionsConfig.inst.crouchKeyText.text == "Mouse0")
                    {
                        tempKey = "Left Mouse";
                    }
                    else if (OptionsConfig.inst.crouchKeyText.text == "Mouse2")
                    {
                        tempKey = "Middle Mouse";
                    }

                    slide.text = "Press " + OptionsConfig.inst.crouchKeyText.text + " whilst " + Environment.NewLine + "running to perform a SLIDE.";
                }
                else
                {
                    GameObject.Find("Toggle Instruct").GetComponent<TextMesh>();
                }

                if (sprint != null)
                {
                    string tempKey = OptionsConfig.inst.sprintKeyText.text;
                    if (OptionsConfig.inst.sprintKeyText.text == "Mouse1")
                    {
                        tempKey = "Right Mouse";
                    }
                    else if (OptionsConfig.inst.sprintKeyText.text == "Mouse0")
                    {
                        tempKey = "Left Mouse";
                    }
                    else if (OptionsConfig.inst.sprintKeyText.text == "Mouse2")
                    {
                        tempKey = "Middle Mouse";
                    }

                    bool sprintToggle = thisccMovement.ReturnIfSprintToggle();

                    if (sprintToggle == true)
                    {
                        sprint.text = "To SPRINT toggle " + tempKey;
                    }
                    else
                    {
                        sprint.text = "To SPRINT hold " + tempKey;
                    }

                }
                else
                {
                    GameObject.Find("Sprint Instruct").GetComponent<TextMesh>();
                }
            }
        }
        catch
        {
            //An error counter will run because it's less intensive thana debug
            int counter;
            counter = 1;
        }
    }
}
