
/*
 * Author: Ross
 * Date: 01/11/17
 * Purpose: Adds functionality for turret AI
 * Placement: Place on an turret game object and fill out variables (hover over variables for description on what they should be)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{

    //Visible Variables
    [SerializeField] [Range(0f, 50f)] [Tooltip("How long does the turret stay looking at a point before changing")] private float timeTillTurn = 2f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How fast the turret turns when looking for the player")] private float turnSpeed = 2f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How quickly can the turret shoot")] private float timeBetweenShots = 2f;
    [SerializeField] [Range(0f, 50f)] [Tooltip("Damage per shot the turret will do")] private int turretDamage = 10;
    [SerializeField] [Range(0f, 50f)] [Tooltip("How far can the turret see")] private int maxSightDistance = 50;
    [SerializeField] [Range(0f, 50f)] [Tooltip("The maximum cone of sight")] private int maxFieldOfView = 45;
    [SerializeField] private LayerMask rayLayer;

    //General Variables
    private float timer, playerShotNearby, maxShotDetection = 40f;
    private GameObject gunObj, muzzleFlash, bullet;
    private Transform playerTransform;
    private Vector3 rayDirection;
    private Quaternion lookAtPoint;
    public Animation lookAnim;
    private Transform bulletOrigin;
    private int droneLayer;
    private RaycastHit fovHit;
    [HideInInspector]
    public bool manuallyTriggerAttack = false;
    private float forceAttackTimer = 0;

    //Script References
    ReusableHealth playerHealthScript;
    Shooting shooting;
    ReusableHealth reusableHealth;

    //FSM States
    [Header("Turret is currently:")] [SerializeField] private States currentState;

    enum States
    {
        Observing,
        Alert,
        End
    }


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealthScript = playerTransform.GetComponent<ReusableHealth>();
        droneLayer = LayerMask.NameToLayer("Drone");
        muzzleFlash = transform.Find("MuzzleObj").gameObject;
        muzzleFlash.SetActive(false);
        bulletOrigin = transform.Find("BulletOrigin").transform;
        reusableHealth = playerTransform.GetComponent<ReusableHealth>();
        shooting = playerTransform.GetComponent<Shooting>();
        lookAnim = GetComponent<Animation>();
        muzzleFlash.SetActive(false);

        currentState = States.Observing;

    }

    private void OnDisable() //Disable combat moosic
    {
        AudioManage.inst.combatMusic.Stop();
    }


    void Update()
    {
        timer += Time.deltaTime;

        FieldOfView();
        LookAtPlayer();
    }

    void FixedUpdate()
    {
        //Add or remove states here...
        if (manuallyTriggerAttack == false)
        {
            switch (currentState)
            {
                case States.Observing:
                    StartCoroutine(Looking());
                    break;
                case States.Alert:
                    Shoot();
                    break;
            }
        }
        else
        {
            forceAttackTimer = forceAttackTimer + Time.fixedDeltaTime;

            if (forceAttackTimer >= timeBetweenShots)
            {
                //Wait for the next shot
                forceAttackTimer = 0;
                //force the turret to shoot at the player
                ForceShootAtPlayer();
            }
        }

    }

    //Simulates the players sight
    public void FieldOfView()
    {

        try
        {
            //Track player location
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        catch
        {

        }


        if (playerTransform != null)
        {

            //Direction from current position to player
            rayDirection = playerTransform.position - transform.position;

            //Check angle between AI's forward vector and the direction vector between player and AI
            if (Vector3.Angle(rayDirection, transform.forward) < maxFieldOfView)
            {
                Debug.DrawLine(transform.position, playerTransform.position, Color.blue);

                if (Physics.Raycast(transform.position, rayDirection, out fovHit, maxSightDistance, rayLayer))
                {

                    //When ray hits drone
                    if (fovHit.transform.gameObject.layer == droneLayer)
                    {
                        //Check if drone script is off
                        if (fovHit.collider.gameObject.GetComponent<Drone>() == null)
                        {
                            //Mark it as seeing player (runs LookAtPlayer)
                            currentState = States.Alert;
                        }


                    }

                    //Check if any collider interrupts the ray from the enemy to the player
                    if (Physics.Linecast(transform.position, playerTransform.position, out fovHit, rayLayer))
                    {

                        if (fovHit.collider != null)
                        {

                            //The player is in the fov
                            if (fovHit.transform.tag == "Player")
                            {
                                //Follow and attack the player
                                currentState = States.Alert;
                            }
                            else if (currentState == States.Alert)
                            {
                                //revert back to normal behavior
                                StartCoroutine(ResetBehavior());
                            }


                        }


                    }
                    else
                    {
                        return;
                    }


                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }


        }


    }

    //Defualt state, play ping-pong animation
    IEnumerator Looking()
    {
        if (currentState == States.Observing && lookAnim != null)
        {
            lookAnim.Play();
        }
        yield break;
    }

    //Focus on the player
    void LookAtPlayer()
    {

        if (currentState == States.Alert && lookAnim != null)
        {
            lookAnim.Stop();
            transform.LookAt(new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z));
            if (AudioManage.inst.combatMusic != null)
            {
                if (AudioManage.inst.combatMusic.isPlaying == false)
                {
                    AudioManage.inst.combatMusic.Play();
                }
            }

        }
        else
        {
            //return;
            if (AudioManage.inst.combatMusic != null)
            {
                AudioManage.inst.combatMusic.Stop();
            }
        }

    }

    //Player has been spotted in FOV, shoot the player
    void Shoot()
    {

        if (Vector3.Distance(transform.position, playerTransform.position) < maxSightDistance && timer >= timeBetweenShots)
        {
            if (AudioManage.inst.combatMusic != null)
            {
                if (AudioManage.inst.combatMusic.isPlaying == false)
                {
                    AudioManage.inst.combatMusic.Play();
                }
            }
            bullet = shooting.ReturnBulletObjectPool()[0];
            bullet.GetComponent<Bullet>().bulletOrigin = bulletOrigin;
            bullet.GetComponent<Bullet>().recoil = 0.8f;
            bullet.GetComponent<Bullet>().bulletDamage = turretDamage;
            bullet.GetComponent<Bullet>().playerBullet = false;

            shooting.ReturnBulletObjectPool().Remove(bullet);

            bullet.transform.rotation = bulletOrigin.transform.rotation;
            bullet.transform.position = bulletOrigin.transform.position;

            bullet.SetActive(true);
            StartCoroutine(MuzzleFlash());

            timer = 0f;
        }
        else
        {
            StartCoroutine(ResetBehavior());
        }

    }

    //Player has been spotted in FOV, shoot the player
    void ForceShootAtPlayer()
    {
        playerHealthScript.CalculateHitDirection(transform.position);
        playerHealthScript.ApplyDamage(turretDamage);
        
        StartCoroutine(MuzzleFlash());
    }

    //Enable/Disable muzzle flash for the turret
    IEnumerator MuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);

        yield break;
    }

    //Show field of view in the scene
    public void OnDrawGizmos()
    {

        //        if (playerTransform == null)
        //        {
        //            return;
        //        }
        //
        //        try
        //        {
        //            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //        }
        //        catch
        //        {
        //
        //        }
        //
        //        Vector3 frontRayPoint = transform.forward * maxSightDistance;
        //
        //        //Visualise rough field of view
        //        Vector3 leftRayPoint = frontRayPoint;
        //        leftRayPoint.x += maxFieldOfView;
        //
        //        Vector3 rightRayPoint = frontRayPoint;
        //        rightRayPoint.x -= maxFieldOfView;
        //
        //        Debug.DrawLine(transform.position, frontRayPoint * maxSightDistance, Color.red);
        //        Debug.DrawLine(transform.position, leftRayPoint * maxSightDistance, Color.green);
        //        Debug.DrawLine(transform.position, rightRayPoint * maxSightDistance, Color.green);

    }

    //Delay then reset
    IEnumerator ResetBehavior()
    {
        yield return new WaitForSeconds(5f);

        currentState = States.Observing;
    }

    //Return whether damaged
    public bool CheckIfAttacking()
    {
        if (currentState == States.Alert)
        {
            return true;
        }
        return false;
    }

    //Should trigger the attack state
    public void TriggerAttackState()
    {
        currentState = States.Alert;
    }


}
