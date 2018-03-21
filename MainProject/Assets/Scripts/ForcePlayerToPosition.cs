using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePlayerToPosition : MonoBehaviour
{
    private Transform player;
    [SerializeField]
    private Transform transformToForcePlayer;
    [SerializeField]
    private bool localPosition = false;
    public bool allowedToForce = true;
	
    // Use this for initialization
	private void Start ()
    {
        player = GameObject.Find("Player").transform;
	}
	
	// Update is called once per frame
	private void FixedUpdate()
    {
        if (player != null)
        {
            //Force to the location position
            if (localPosition == true)
            {
                player.localPosition = transformToForcePlayer.localPosition;
            }
            else
            {
                player.position = transformToForcePlayer.position;
            }
        }      
	}
}
