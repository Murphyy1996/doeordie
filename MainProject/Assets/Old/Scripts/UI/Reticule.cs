using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticule : MonoBehaviour
{
    //Authior: Kate Georgiou
    private Transform player, cam;
    [SerializeField]
    private LayerMask layer;
    [SerializeField]
    private Image green, red, hit;
    private GameObject empty;
    private Vector3 defaultReticuleSize;
    public static Reticule inst;

    private void Awake()
    {
        inst = this;
    }

    // Use this for initialization
    private void Start()
    {


        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main.transform;
        green.enabled = true;
        red.enabled = false;
        empty = new GameObject();
        empty.name = "ForReticule";
        defaultReticuleSize = green.transform.localScale;
        hit.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        empty.transform.position = player.transform.position;
        empty.transform.rotation = cam.transform.rotation;
        RaycastHit hit;
        Debug.DrawRay(empty.transform.position, empty.transform.forward, Color.black);
        if (Physics.Raycast(empty.transform.position, empty.transform.forward, out hit, 500f, layer))
        {
            //turn reticule red.
            if (hit.collider.tag == "enemy" || hit.collider.tag == "Head Shot")
            {
                green.enabled = false;
                red.enabled = true;
                //  Debug.Log("DETECTING ENEMY");
            }
            else
            {

                green.enabled = true;
                red.enabled = false;
                // Debug.Log("No detect, only cry");
            }

        }
    }

    public void IncreaseReticuleSize(float sizeToIncreaseBy)
    {
        green.transform.localScale = green.transform.localScale * sizeToIncreaseBy;
        red.transform.localScale = red.transform.localScale * sizeToIncreaseBy;
    }

    public void ResetReticuleSize()
    {
        green.transform.localScale = defaultReticuleSize;
        red.transform.localScale = defaultReticuleSize;
    }

    public IEnumerator hitState(float timeToShow)
    {
        hit.enabled = true;
        yield return new WaitForSeconds(timeToShow);
        hit.enabled = false;

    }

    public void turnOffHitMarker()
    {
        hit.enabled = false;
    }
}
