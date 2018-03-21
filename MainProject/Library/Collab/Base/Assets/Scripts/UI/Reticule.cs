using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticule : MonoBehaviour
{

    private Transform player, cam;
    [SerializeField]
    private LayerMask layer;
    [SerializeField]
    private Image green, red;
    private GameObject empty;


    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main.transform;
        green.enabled = true;
        red.enabled = false;
        empty = new GameObject();
        empty.name = "ForReticule";


    }
	
    // Update is called once per frame
    void Update()
    {
        empty.transform.position = player.transform.position;
        empty.transform.rotation = cam.transform.rotation;

        RaycastHit hit;
        Debug.DrawRay(empty.transform.position, empty.transform.forward, Color.black);
        if (Physics.Raycast(empty.transform.position, empty.transform.forward, out hit, 1000f, layer))
        {
            //turn reticule red.
            if (hit.collider.tag == "enemy")
            {
                green.enabled = false;
                red.enabled = true;
            }
            else
            {
                green.enabled = true;
                red.enabled = false;
            }
           
        }




    }
}
