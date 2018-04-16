//Author: James Murphy
//Purpose: Change the colour of these particles depeding on if the player can teleport
//Requirements:	Different indicator references to be filled

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTeleportParticleColour : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer rendererToCopy;
    private ParticleSystem thisParticleSystem;
    private Color blueColour, redColour;

    //Get this particle system
    private void Start()
    {
        thisParticleSystem = GetComponent<ParticleSystem>();
    }

    //Change the particle colours based on the activity of the renderers
    private void FixedUpdate()
    {
        if (rendererToCopy.enabled == true)
        {
            thisParticleSystem.enableEmission = true;
        }
        else
        {
            thisParticleSystem.Clear();
            thisParticleSystem.enableEmission = false;
        }
    }
}
