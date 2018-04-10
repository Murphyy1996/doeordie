using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceObjToPosition : MonoBehaviour
{
    [SerializeField]
    private Transform objToForce;
    [SerializeField]
    private Transform transformToForceTo;
    [SerializeField]
    private bool localPosition = false;
    public bool allowedToForce = true;
	
	// Update is called once per frame
	private void FixedUpdate()
    {
        if (objToForce != null)
        {
            //Force to the location position
            if (localPosition == true)
            {
                objToForce.localPosition = transformToForceTo.localPosition;
            }
            else
            {
                objToForce.position = transformToForceTo.position;
            }
        }      
	}
}
