using System.Collections;
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
                AudioManage.inst.healthPickup.Play();
                Debug.Log("playing health audio pls");
                healthScript.healthValue += amountToIncrease;
                //Don't let it go passed the max value
                if (healthScript.healthValue > healthScript.maxHealth)
                {
                    healthScript.healthValue = healthScript.maxHealth;
                }
                //Turn off this object
                this.gameObject.SetActive(false);
            }
        }

    }
}
