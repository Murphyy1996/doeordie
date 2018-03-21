using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedActivationOfObj : MonoBehaviour
{
    [SerializeField]
    private float delayAmount = 2;
    [SerializeField]
    private GameObject[] objectsToActivate;
	
    // Use this for initialization
	private void Start ()
    {
        Invoke("EnableObjects", delayAmount);
	}

    private void EnableObjects() //Enable each object in the list
    {
        if (objectsToActivate.Length > 0)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
        Destroy(this);
    }
}
