using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlocker : MonoBehaviour {

    [SerializeField]
    private GameObject door;

	// Use this for initialization
	void Start () {

        
        door.GetComponent<Collider>().enabled = false;
        door.GetComponent<MeshRenderer>().enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
           
            door.GetComponent<Collider>().enabled = true;
            door.GetComponent<MeshRenderer>().enabled = true;
            //Destroy this trigger object
            Destroy(this.gameObject);
        }
    }
}
