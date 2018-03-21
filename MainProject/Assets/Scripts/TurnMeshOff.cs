//Author: James Murphy
//Purpose: Turn them mesh off after startup
//Requirements: A mesh renderer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMeshOff : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
