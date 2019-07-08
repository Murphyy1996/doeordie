using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoors : MonoBehaviour {
    //Author: Kate Georgiou (this is already attached to the locked door prefabs - the models jsut need cahnging out)
   
    public bool walkableThrough = false, keyobtained = false;
    private Collider stopmove;

    private void Start()
    {
        stopmove = this.gameObject.GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.tag == "Player" && keyobtained == true)
        {
            stopmove.enabled = false;
            Destroy(this.gameObject);
        }
    }




}
