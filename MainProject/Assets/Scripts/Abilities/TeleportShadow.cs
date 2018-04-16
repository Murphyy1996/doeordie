using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShadow : MonoBehaviour
{
    [SerializeField]
    private LayerMask rayLayer;
    private LineRenderer lineRendererToUse;
    [SerializeField]
    private Transform indicatorShadow;
    [SerializeField]
    private Material materialToUse;
    private MeshRenderer thisMeshRenderer;

    private void Start()
    {
        lineRendererToUse = GetComponent<LineRenderer>();
        lineRendererToUse.enabled = true;
        lineRendererToUse.SetWidth(0.5f, 0.5f);
        lineRendererToUse.SetColors(materialToUse.color, materialToUse.color);
        indicatorShadow.transform.SetParent(null);
        indicatorShadow.transform.rotation = Quaternion.Euler(0, 0, 0);
        thisMeshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        if (indicatorShadow != null)
        {
            indicatorShadow.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (indicatorShadow != null)
        {
            indicatorShadow.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (thisMeshRenderer.enabled == true)
        {
            RaycastHit raycastHit;
            //Perform the raycast
            Physics.Raycast(transform.position, -Vector3.up, out raycastHit, rayLayer);
            //If the raycast hits something then continue to do the shadow downwards, else just do it well below
            Vector3 endPosition = Vector3.zero;
            //If there is no hit point
            if (raycastHit.collider == null)
            {
                endPosition = new Vector3(transform.position.x, -1000, transform.position.z);
            }
            else
            {
                endPosition = raycastHit.point;
            }
            //Place the shadow at the hit position
            if (indicatorShadow != null)
            {
                indicatorShadow.transform.position = endPosition;
            }
            //If the line renderer isn't null, render it
            if (lineRendererToUse != null)
            {
                lineRendererToUse.enabled = true;
                lineRendererToUse.SetPosition(0, transform.position);
                lineRendererToUse.SetPosition(1, endPosition);
            }
        }
        else
        {
            indicatorShadow.transform.position = Vector3.zero;
            lineRendererToUse.enabled = false;
        }

    }
}
