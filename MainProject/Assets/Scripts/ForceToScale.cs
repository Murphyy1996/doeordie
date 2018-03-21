//Author: James Murphy
//Purpose: Force object to a certain scale ALL the time
//Requirements: Nothing really

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceToScale : MonoBehaviour
{
    public float scaleToForce = 1;
    private void Update()
    {
        transform.localScale = new Vector3(scaleToForce, scaleToForce, scaleToForce);
    }
}
