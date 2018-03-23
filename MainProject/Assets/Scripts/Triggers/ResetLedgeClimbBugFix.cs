//Author: James Murphy
//Purpose: When started this will reset the ledge climb
//Requirements: To be on an object that is activated

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLedgeClimbBugFix : MonoBehaviour
{
    private void Start() //When this obj is activated reset the ledge climb script
    {
        try
        {
            GameObject player = GameObject.Find("Player");
            player.GetComponent<LedgeClimbV2>().ResetToDefault();
            Destroy(this);
        }
        catch
        {
            Destroy(this);
        }
    }
}
