using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixChildRotation : MonoBehaviour
{

    Quaternion rot;
    Vector3 pos;

    void Awake()
    {
        rot = transform.rotation;
        pos = transform.position;
    }

    void LateUpdate()
    {
        transform.rotation = rot;
        transform.position = pos;
    }

   
}
