using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour 
{

    public GameObject fanToDisable;


	private bool on = false;


    void OnTriggerEnter(Collider other) 
    {
        if (other.GetComponent<Collider>().tag == "Player" /*&& !on*/)
        {

                print(on);
                print("switch");

                fanToDisable.GetComponent<ObjectRotation>().enabled = false;
                //on = true;
          
          
        }

	}


//    void OnTriggerExit(Collider other) 
//    {
//        if (other.GetComponent<Collider>().tag == "Player" /*&& !on*/)
//        {
//
//            print(on);
//            print("switch");
//
//            fanToDisable.GetComponent<ObjectRotation>().enabled = true;
//            //on = true;
//
//
//        }
//
//    }

}
