using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUse : MonoBehaviour {
    //Author: Kaate Georgiou . Make sure to drag the door you want linked to this key into this script in the inspector and then change the model in the prefab.


    [SerializeField]
    private GameObject doorLinkedTo;


    private void Start()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        LockedDoors locking = doorLinkedTo.GetComponent<LockedDoors>();
        if (other.tag == "Player")
        {
            locking.keyobtained = true;
            Destroy(this.gameObject);
        }
    }

}
