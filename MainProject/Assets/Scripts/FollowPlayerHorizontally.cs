//Author: James Murphy
//Purpose: Follow the player on all axis except the y axis
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerHorizontally : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = transform.parent.gameObject;
        transform.SetParent(null);
    }

    private void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        }
    }
}
