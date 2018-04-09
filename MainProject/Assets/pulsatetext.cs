using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pulsatetext : MonoBehaviour {

    [SerializeField]
    private Outline oLine;
    [SerializeField]
    private Text ah;
    [SerializeField]
    private float speedOfFlash;

	// Use this for initialization
	void Start ()
    {
        oLine = this.gameObject.GetComponent<Outline>();
        oLine.enabled = false;
        ah = this.gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (ah.enabled == true)
        {
            //run method for flashing
            StartCoroutine("flash");
        }
        else
        {
            return;
        }
	}

    IEnumerator flash()
    {
        oLine.enabled = true;
        yield return new WaitForSeconds(speedOfFlash);
        oLine.enabled = false;
    }


}
