using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAppear : MonoBehaviour
{
	//Author: Kate Georgiou

	public GameObject text;
	public bool pressToAppear = true;

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (Input.GetKey(KeyCode.E) && pressToAppear)
			{
				text.SetActive(true);
			}

			if (pressToAppear == false)
			{
				text.SetActive(true);
			}
		}
			
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			text.SetActive(false);
		}
	}
}
