//Author: James Murphy
//Purpose: To stab knife
//Place me as a child of the knife

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneStab : MonoBehaviour
{
    [SerializeField]
    [Range(0, 30)]
    private int attackDamage = 5;
    [SerializeField]
    private float knifeSpeed;
    [SerializeField]
    [Range(0, 4f)]
    private float attackCooldownTime = 2f, damageCooldownTime = 2f;
    [SerializeField]
    [Range(0, 1f)]
    private float stabDistance = 0.6f;
    private GameObject startPoint, attackPoint, lerpTarget;
    private bool canLerp = true, canApplyDamage = true, allSetUp = false;
    private Drone droneMovementScript;
    private Rigidbody parentRB;
    private Teleporting playerTeleportScript;


    private void Awake() //Set up the lerp points
    {
        //Get the drone RB
        parentRB = GetComponentInParent<Rigidbody>();
        playerTeleportScript = GameObject.Find("Player").GetComponent<Teleporting>();
        //Get the drone movement script
        droneMovementScript = GetComponentInParent<Drone>();
        //Create the lerp points
        startPoint = new GameObject();
        startPoint.transform.position = transform.position;
        startPoint.transform.SetParent(transform.parent);
        startPoint.name = "Knife Start";
        attackPoint = new GameObject();
        attackPoint.transform.position = new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, startPoint.transform.position.z + stabDistance);
        attackPoint.name = "Knife End";
        attackPoint.transform.SetParent(transform.parent);
        allSetUp = true;
    }

    private void FixedUpdate() //This will lerp when needed
    {
        try
        {
            if (startPoint != null && attackPoint != null && allSetUp == true)
            {
                //Set the default lerp target
                if (lerpTarget == null)
                {
                    lerpTarget = attackPoint;
                }

                if (droneMovementScript.CheckIfAttacking() == true) //Only stab if attack
                {
                    if (canLerp == true && canApplyDamage == true)
                    {
                        //Run lerp code here!!!
                        transform.position = Vector3.Lerp(transform.position, lerpTarget.transform.position, knifeSpeed * Time.deltaTime);
                    }

                    //If the object has reached the target, go back to default
                    if (Vector3.Distance(transform.position, lerpTarget.transform.position) <= 0.05f)
                    {
                        if (lerpTarget == attackPoint)
                        {
                            lerpTarget = startPoint;
                        }
                        else
                        {
                            lerpTarget = attackPoint;
                            canLerp = false;
                            //Activate the attack cooldown
                            StartCoroutine(AttackCooldown());
                        }
                    }
                }
                else //Hold the knifes in
                {
                    StopAllCoroutines();
                    canLerp = true;
                    transform.position = Vector3.Lerp(transform.position, startPoint.transform.position, knifeSpeed * Time.deltaTime);
                }

            }
        }
        catch
        {
            print("Error in drone attack script");
        }

    }

    private void OnTriggerStay(Collider otherObject)
    {
        if (otherObject.tag == "Player" && droneMovementScript.CheckIfAttacking() == true && canApplyDamage == true && playerTeleportScript.ReturnIfTeleporting() == false)
        {
            GameObject player = otherObject.gameObject;
            if (player.GetComponent<ReusableHealth>() != null)
            {
                canApplyDamage = false;
                player.GetComponent<Grapple>().ExitGrapple();
                player.GetComponent<ReusableHealth>().CalculateHitDirection(transform.position);
                player.GetComponent<ReusableHealth>().ApplyDamage(attackDamage);
                droneMovementScript.StartCoroutine(droneMovementScript.ActivateStun(2.5f));
                this.transform.parent.GetComponent<Rigidbody>().AddRelativeForce(-Vector3.forward * 1000);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldownTime);
        canApplyDamage = true;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldownTime);
        canLerp = true;
    }
}
