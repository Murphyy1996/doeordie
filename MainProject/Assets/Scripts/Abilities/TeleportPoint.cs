using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    [HideInInspector]
    public CharacterController playerCC;
    [SerializeField]
    private bool isInObject = false, hasPlayerLOS = false;
    private GameObject spawnedVisualIndicator;
    private Renderer allowedToTeleport, notAllowedToTeleport;
    private Teleporting teleportScript;
    private LayerMask layerMask;
    private Transform camTransform;
    private FirstPersonCamera fpsScript;
    private float rayLength;

    private void OnEnable() //Size the collider depending the size of the player collider
    {
        hasPlayerLOS = false;
        isInObject = false;
        this.gameObject.layer = 2;
        rayLength = transform.localScale.y + 0.2f;
        Invoke("DelayedEnable", 0.1f);
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.Sleep();
        }
    }

    private void DelayedEnable()
    {
        if (rb == null)
        {
            this.gameObject.name = "Teleport Point";
            capsuleCollider = this.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.isTrigger = true;
            rb = this.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = true;
            rb.useGravity = false;
            teleportScript = playerCC.GetComponent<Teleporting>();
            layerMask = teleportScript.ReturnTeleportPointLayerMask();
            camTransform = Camera.main.transform;
            fpsScript = camTransform.GetComponent<FirstPersonCamera>();
        }
        if (playerCC != null && capsuleCollider != null)
        {
            capsuleCollider.radius = playerCC.radius;
            capsuleCollider.height = playerCC.height;
        }
        if (spawnedVisualIndicator == null)
        {
            spawnedVisualIndicator = Instantiate(playerCC.GetComponent<Teleporting>().ReturnTeleportIndicator(), transform.position, transform.rotation);
            //Get the required renderers
            allowedToTeleport = spawnedVisualIndicator.GetComponent<Renderer>();
            foreach (Transform tran in allowedToTeleport.transform)
            {
                if (tran.gameObject.name == "NotAllowedToTeleport")
                {
                    notAllowedToTeleport = tran.GetComponent<Renderer>();
                    break;
                }
            }
            spawnedVisualIndicator.transform.localScale = transform.localScale;
            notAllowedToTeleport.enabled = false;
            allowedToTeleport.enabled = false;
            spawnedVisualIndicator.name = "Visual Teleport Indicator";
            spawnedVisualIndicator.transform.SetParent(transform);
        }
        rb.WakeUp();
    }

    private void RaycastAtPlayer()
    {
        //If the player reference is available
        if (playerCC != null && teleportScript != null && spawnedVisualIndicator != null)
        {
            Vector3 direction = playerCC.transform.position - transform.position;
            //Check if the surface is teleportable
            RaycastHit rayOut;
            if (Physics.Raycast(transform.position, direction, out rayOut, 50f, layerMask))
            {
                if (rayOut.collider.tag == "Player")
                {
                    hasPlayerLOS = true;
                }
                else
                {
                    hasPlayerLOS = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        RaycastAtPlayer();
        //if this object is active
        if (teleportScript != null && this.isActiveAndEnabled == true && spawnedVisualIndicator != null)
        {
            //Decide what colour to set the teleport indicator
            if (isInObject == true || hasPlayerLOS == false)
            {
                ColourTeleportIndicatorRed();
            }
            else
            {
                if (teleportScript.CanActuallyTeleport() == true)
                {
                    ColourTeleportIndicatorBlue();
                }
                else
                {
                    ColourTeleportIndicatorRed();
                }
            }
            //AutoAlignRaycast();
        }
    }
    private void AutoAlignRaycast()
    {
        bool hitTop = false, hitBottom = false;
        RaycastHit rayhit;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 2), transform.position.z);
        if (Physics.Raycast(rayStart, -transform.up, out rayhit, rayLength, layerMask))
        {
            hitBottom = true;
        }
        rayStart = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2), transform.position.z);
        if (Physics.Raycast(rayStart, transform.up, out rayhit, rayLength, layerMask))
        {
            hitTop = true;
        }

        if (isInObject == true)
        {
            if (hitTop == true && hitBottom == true || hitTop == false && hitBottom == false)
            {
                print("whos the top, whos the bottom???");
            }
            else if (hitTop == true && hitBottom == false)
            {
                print("sink me bitch!!!");
            }
            else if (hitBottom == true && hitTop == false)
            {
                print("raise me up!!!");
            }
        }
    }

    private void ColourTeleportIndicatorRed()
    {
        notAllowedToTeleport.enabled = true;
        allowedToTeleport.enabled = false;
    }

    private void ColourTeleportIndicatorBlue()
    {
        notAllowedToTeleport.enabled = false;
        allowedToTeleport.enabled = true;
    }

    private void OnTriggerStay(Collider otherObject)
    {
        if (otherObject.tag != "Player" && otherObject.tag != "enemy")
        {
            isInObject = true;
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.tag != "Player" && otherObject.tag != "enemy")
        {
            isInObject = false;
        }
    }

    public bool AmInObject()
    {
        if (hasPlayerLOS == true)
        {
            return isInObject;
        }
        return true;
    }
}
