using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManage : MonoBehaviour
{

    /// <summary>
    /// Author : Kate Georgiou Purpoise : holds all the audio sources and allows themt o be refrenced from other scripts
    /// </summary>


    public static AudioManage inst;

    public AudioSource background, pistolShot, machShot, teleport, death, slide, grapple, player, combatMusic, bulletEnvironment, falling, pickUp;
    public AudioClip walking, running;
    CharacterControllerMovement thisCC;

    private void Awake()
    {
        inst = this;
    }
}
