using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: James Murphy
//Purpose: Follow another obj

public class FollowObjPos : MonoBehaviour
{
    [SerializeField]
    private Transform objToFollow;

    private void FixedUpdate()
    {
        transform.position = objToFollow.position;
    }
}
