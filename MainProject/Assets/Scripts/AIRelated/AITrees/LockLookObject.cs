using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockLookObject : MonoBehaviour 
{

    Quaternion rot;
    Vector3 pos;

    void Awake()
    {
        rot = transform.rotation;
        pos = transform.position;
    }

    void Update()
    {
        transform.rotation = rot;
        transform.position = Vector3.zero;
    }

    void LateUpdate()
    {
        transform.rotation = rot;
        transform.position = Vector3.zero;
    }

}
