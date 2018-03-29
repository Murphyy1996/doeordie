﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealth : MonoBehaviour {

    private GameObject player;
    [SerializeField]
    private int amountToIncrease;
    private ReusableHealth healthScript;
	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        healthScript = player.GetComponent<ReusableHealth>();
	}

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (healthScript.healthValue != healthScript.maxHealth)
            {
                healthScript.healthValue += amountToIncrease;
                this.gameObject.SetActive(false);
            }
        }

    }
}
