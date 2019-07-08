using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideTrail : MonoBehaviour 
{
    //Author Kate Georgiou Desc: Handles the forming of the trail while the player slides.
    Crouch crouch;

    private GameObject player;
    
    private ParticleSystem slideParticle;
    [SerializeField] private GameObject slideObj;


	void Start () 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        crouch = player.GetComponent<Crouch>();
        slideParticle = gameObject.GetComponentInChildren<ParticleSystem>();
        //slideObj = GetComponentInChildren<GameObject>();
        slideObj.SetActive(false);
        //slideParticle.enableEmission = false;
        //slideParticle.emissionRate = 0;
	}
	

	void Update () 
    {

        if (crouch.IsPlayerSliding() == true)
        {
            slideObj.SetActive(true);
            //slideParticle.enableEmission = true;
            //slideParticle.emissionRate = 1;
        }
        else
        {
            slideObj.SetActive(false);
            //slideParticle.enableEmission = false;
            //slideParticle.emissionRate = 0;
        }

	}
}
