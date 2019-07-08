using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDoor : MonoBehaviour
{

	public Animator _animator;

	void OnTriggerEnter(Collider other)
	{
        if (other.GetComponent<Collider>().tag == "Player")
        {
            _animator.SetBool("isClosing", true);
        }	
		
	}
        

}
