//Author: James Murphy 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject grappleObject, player;
    private Vector3 playerStart;
    private SphereCollider collider;
    private bool needsToMove = false;

    private void Start() //Set up any components
    {
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        this.gameObject.layer = 2;
        collider = GetComponent<SphereCollider>();
        collider.isTrigger = true;
        player = GameObject.FindGameObjectWithTag("Player");
        playerStart = player.transform.position;
    }
}
