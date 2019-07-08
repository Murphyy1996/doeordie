//Author: James Murphy
//Purpose: Follow the selected point
//Requirements: None

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalFollow : MonoBehaviour
{
    public Transform followZone;

    private void Update()
    {
        if (followZone != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, followZone.position, 100 * Time.deltaTime);
            transform.rotation = followZone.rotation;
        }
        else
        {
            //If the follow zone is null, destroy this object
            this.gameObject.SetActive(false);
        }
    }
}
