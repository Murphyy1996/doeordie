//Author: James Murphy
//Purpose: Control when the sneak theme players
//Requirements: Sneak audio source

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSneakTheme : MonoBehaviour
{
    private float defaultVolume;
    [SerializeField]
    private float fadeSpeed = 27;
    private bool fadeIn = false, fadeOut = false, initialSetUp = false;

    private void Start()
    {
        if (AudioManage.inst.sneakMusic != null)
        {
            defaultVolume = AudioManage.inst.sneakMusic.volume;
            AudioManage.inst.sneakMusic.volume = 0;
            initialSetUp = true;
        }
    }
    private void FixedUpdate() //Theme will be controlled here
    {
        if (AudioManage.inst != null && initialSetUp == true)
        {
            //Will tell the fades in to run
            if (AudioManage.inst.combatMusic.isPlaying == false)
            {
                if (AudioManage.inst.sneakMusic.isPlaying == false || AudioManage.inst.sneakMusic.volume == 0)
                {
                    fadeOut = false;
                    fadeIn = true;
                }
            }
            else //Will tell the fade out to run
            {
                if (AudioManage.inst.sneakMusic.isPlaying == true)
                {
                    fadeOut = true;
                    fadeIn = false;
                }
            }

            //Will control fading in
            if (fadeIn == true)
            {
                fadeOut = false;
                //Will start the initial play
                if (AudioManage.inst.sneakMusic.isPlaying == false)
                {
                    AudioManage.inst.sneakMusic.Play();
                }
                //As long as the volume is below the default value, then increase the volume by the desired rate
                if (AudioManage.inst.sneakMusic.volume < defaultVolume)
                {
                    AudioManage.inst.sneakMusic.volume = AudioManage.inst.sneakMusic.volume + ((Time.fixedDeltaTime / 100) * fadeSpeed);
                }
                else //Once the desired volume has been reached stop increasing the volume
                {
                    fadeIn = false;
                    AudioManage.inst.sneakMusic.volume = defaultVolume;
                }
            }
            else if (fadeOut == true) //Will control fading out
            {
                fadeIn = false;

                //For as long as the desired volume is bigger than zero
                if (AudioManage.inst.sneakMusic.volume > 0)
                {
                    //Decrease the volume
                    AudioManage.inst.sneakMusic.volume = AudioManage.inst.sneakMusic.volume - ((Time.fixedDeltaTime / 100) * fadeSpeed);
                }
                else //If the volume is equal or smaller than zero stop the audio source
                {
                    fadeIn = false;
                    AudioManage.inst.sneakMusic.volume = 0;
                    //AudioManage.inst.sneakMusic.Stop();
                    fadeOut = false;
                }
            }

        }
    }
}
