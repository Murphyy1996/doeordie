using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantMovement : MonoBehaviour
{

	public Transform outputPosition;

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			//get doomclone script
            other.gameObject.transform.position = outputPosition.position;
		}
	}
}
