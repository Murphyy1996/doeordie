using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TextUpdated : MonoBehaviour
{

    [SerializeField]
    private TextMesh crouchInstructions, zoom, grapple, sprint, slide;


    // Use this for initialization
    void Start()
    {
        crouchInstructions = GameObject.Find("CrouchInstructions").GetComponent<TextMesh>();
        grapple = GameObject.Find("Grapple Instruct").GetComponent<TextMesh>();
        zoom = GameObject.Find("PistolInstruct").GetComponent<TextMesh>();
        slide = GameObject.Find("Toggle Instruct").GetComponent<TextMesh>();
        sprint = GameObject.Find("Sprint Instruct").GetComponent<TextMesh>();



    }
    // Update is called once per frame
    void Update()
    {
        if (OptionsConfig.inst != null)
        {
            if (crouchInstructions != null)
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

                crouchInstructions.text = "Press " + tempKey + " to crouch";
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
                    tempKey = "Right Mouse";
                }
                else if (OptionsConfig.inst.grappleKeyText.text == "Mouse0")
                {
                    tempKey = "Left Mouse";
                }
                else if (OptionsConfig.inst.grappleKeyText.text == "Mouse2")
                {
                    tempKey = "Middle Mouse";
                }
                grapple.text = "Press " + tempKey + " to use the grapple";
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
                    tempKey = "Right Mouse";
                }
                else if (OptionsConfig.inst.zoomKeyText.text == "Mouse0")
                {
                    tempKey = "Left Mouse";
                }
                else if (OptionsConfig.inst.zoomKeyText.text == "Mouse2")
                {
                    tempKey = "Middle Mouse";
                }
                grapple.text = "Press " + tempKey + " to use the grapple";
                zoom.text = "Press Left mouse button to shoot and" + tempKey + Environment.NewLine + "to zoom. You can then press" + OptionsConfig.inst.weaponSwapKeyText.text + " to swap weapons";
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

                slide.text = "To slide, while running, press" + tempKey + " to crouch";
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


                sprint.text = "To sprint either hold/toggle " + Environment.NewLine + tempKey;

            }
            else
            {
                GameObject.Find("Sprint Instruct").GetComponent<TextMesh>();
            }
        }
    }
}
