using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    [SerializeField]
    private GameObject shadowPrefab;
    private GameObject shadowObj;
    [SerializeField]
    private LayerMask rayLayerMask;

    private void Start() //Create the shadow obj
    {
        shadowObj = Instantiate(shadowPrefab, transform.position, Quaternion.identity) as GameObject;
        shadowObj.name = "ShadowObj";
        shadowObj.transform.SetParent(transform);
    }

    private void FixedUpdate()
    {
        if (shadowObj != null)
        {
            bool hitObj = false;
            RaycastHit rayhit;
            //Check if hit obj
            if (Physics.Raycast(transform.position, -Vector3.up, out rayhit, rayLayerMask))
            {
                hitObj = true;
            }
            //Make the y value match the rayhit
            if (hitObj == true)
            {
                shadowObj.transform.position = new Vector3(transform.position.x, rayhit.point.y, transform.position.z);
            }
            else
            {
                shadowObj.transform.position = new Vector3(transform.position.x, 10000, transform.position.z);
            }
        }
    }
}
