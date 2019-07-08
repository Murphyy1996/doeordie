using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTaggedObjects : MonoBehaviour
{
    [SerializeField]
    private string tagToDestroy = "Untagged";

    private void Start() //Set up this object
    {
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().enabled = false;
        }
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().isTrigger = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) //Destroy tagged objects on collision
    {
        if (other.tag == tagToDestroy)
        {
            Destroy(other.gameObject);
        }
    }
}
