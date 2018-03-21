using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealth : MonoBehaviour {

    private GameObject player;
    [SerializeField]
    private int amountToIncrease;
	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<ReusableHealth>().healthValue += amountToIncrease;
            Destroy(this.gameObject);
        }

    }
}
