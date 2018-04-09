//Author: James Murphy
//Purpose: Move the bullet forwards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public int bulletDamage = 0;
    [SerializeField]
    [Range(0, 4)]
    private int headshotDamageMultiplier = 2;
    [HideInInspector]
    public float recoil = 0;
    public bool playerBullet = true;
    private Shooting shootingScript;
    private GameObject lastHitObj, playerCamera, bulletDecalParent;
    private RaycastHit storedHit, predictedHit;
    [SerializeField]
    private LayerMask bulletLayerMask;
    private Vector3 colliderHitLocation;
    [HideInInspector]
    public Transform bulletOrigin;

    private void OnEnable()
    {
        //Only get all components when needed
        if (playerCamera == null)
        {
            playerCamera = Camera.main.gameObject;
            shootingScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooting>();
            //Get the parent for bullet decals so they dont clog up the scene
            bulletDecalParent = GameObject.Find("BulletDecalParent");
            if (bulletDecalParent == null)
            {
                bulletDecalParent = new GameObject();
                bulletDecalParent.name = "BulletDecalParent";
            }
        }

        //Set the correct layer
        if (playerBullet == true)
        {
            this.gameObject.layer = 13;
        }
        else
        {
            this.gameObject.layer = 21;
        }
        StartCoroutine(PutMeBackInThePool());
        //Check for hit detection
        BulletHitDetection();
    }

    private void BulletHitDetection() //This code handles all bullet detection
    {
        //By default use this transform as a fail safe
        Transform rayStart = transform;
        //If the bullet origin has been filled, then use that instead
        if (bulletOrigin != null)
        {
            rayStart = bulletOrigin;
        }
        //Work out recoil if needed
        if (recoil != 0)
        {
            //Work out the predicted hit point
            Physics.Raycast(rayStart.transform.position, rayStart.transform.forward, out predictedHit, 100000, bulletLayerMask);

            //if you are very close to the target, reduce recoil
            if (predictedHit.distance <= 8)
            {
                //Store the distance value
                float distance = predictedHit.distance;
                //Reduce recoil based on how close you are
                if (distance <= 1)
                {
                    recoil = recoil / 8;
                }
                else if (distance <= 2)
                {
                    recoil = recoil / 5;
                }
                else if (distance <= 3)
                {
                    recoil = recoil / 4;
                }
                else if (distance <= 5.5)
                {
                    recoil = recoil / 2;
                }
                else if (distance <= 8)
                {
                    recoil = recoil / 1.5f;
                }
            }
        }
        //Work out the recoil position
        Vector3 recoilAffectedRayStart = new Vector3(rayStart.position.x + Random.Range(-recoil, recoil), rayStart.position.y + Random.Range(-recoil, recoil), rayStart.position.z + Random.Range(-recoil, recoil));
        //This variable will make sure the raycast doesnt run more than it needs to
        if (Physics.Raycast(recoilAffectedRayStart, rayStart.transform.forward, out storedHit, 100000, bulletLayerMask))
        {
            //If the bullet has been shot by an enemy
            if (storedHit.collider.gameObject.tag == "Player" && playerBullet == false)
            {
                if (storedHit.collider.GetComponent<ReusableHealth>() != null)
                {
                    //Call the BulletHitDirection Method from the reusable health script
                    storedHit.collider.GetComponent<ReusableHealth>().CalculateHitDirection(transform.position);
                    //Damage the other object
                    storedHit.collider.GetComponent<ReusableHealth>().ApplyDamage(bulletDamage);
                }
                //Remove this bullet from the scene
                RepoolBullet();
            }
            //If the player has scored a headshot
            else if (storedHit.collider.gameObject.tag == "Head Shot" && playerBullet == true)
            {
                if (storedHit.collider.GetComponentInParent<ReusableHealth>() != null)
                {
                    float floatDamage = bulletDamage * headshotDamageMultiplier;
                    int calculatedweaponDamage = (int)floatDamage;
                    //Damage the other object
                    ReusableHealth healthScript = storedHit.collider.GetComponentInParent<ReusableHealth>();
                    if (healthScript.glowWhenDamaged == true)
                    {
                        healthScript.glowMaterialToUse = healthScript.glowMaterialCritical;
                    }
                    healthScript.ApplyDamage(calculatedweaponDamage);
                }
                Reticule.inst.StopAllCoroutines();
                Reticule.inst.turnOffHitMarker();
                Reticule.inst.StartCoroutine(Reticule.inst.hitState(0.05f));
                //Remove this bullet from the scene
                RepoolBullet();
            }
            //If shot by the player
            else if (storedHit.collider.gameObject.tag == "enemy" && playerBullet == true)
            {
                //Debug.Log(storedHit.collider.gameObject.name);
                DroneHitConditions();
                if (storedHit.collider.GetComponent<ReusableHealth>() != null)
                {
                    ReusableHealth healthScript = storedHit.collider.GetComponent<ReusableHealth>();
                    if (healthScript.glowWhenDamaged == true)
                    {
                        healthScript.glowMaterialToUse = healthScript.glowMaterialRegularHit;
                    }
                    //Damage the other object
                    storedHit.collider.GetComponent<ReusableHealth>().ApplyDamage(bulletDamage);
                }
                Reticule.inst.StopAllCoroutines();
                Reticule.inst.turnOffHitMarker();
                Reticule.inst.StartCoroutine(Reticule.inst.hitState(0.05f));
                //Remove this bullet from the scene
                RepoolBullet();
            }
            else //If the bullet hits the environment just spawn a general hit decal
            {
                if (playerBullet == true && storedHit.collider.gameObject != shootingScript.gameObject || playerBullet == false)
                {
                    if (storedHit.collider.gameObject.GetComponent<ReusableHealth>() != null && storedHit.collider.gameObject.GetComponent<ReusableHealth>().enabled == true)
                    {
                        //Call the BulletHitDirection Method from the reusable health script
                        storedHit.collider.GetComponent<ReusableHealth>().CalculateHitDirection(storedHit.point);
                        //Damage the other object
                        storedHit.collider.gameObject.GetComponent<ReusableHealth>().ApplyDamage(bulletDamage);
                    }
                    //Create a bullet decal
                    CreateBulletDecal();

                    //Check if the audio clip exists
                    if (AudioManage.inst.bulletEnvironment != null)
                    {
                        if (AudioManage.inst.bulletEnvironment.isPlaying == false)
                        {
                            AudioManage.inst.bulletEnvironment.Play();
                            Debug.Log("playing audio source");
                            Invoke("StopBulletShotEnvironment", 1f);
                        }
                    }

                    //Remove this bullet from the scene
                    RepoolBullet();

                }
            }
        }
        else //If the bullet hits nothing, repool it
        {
            RepoolBullet();
        }
    }

    private void Recoil()
    {
        //Raycast for where the bullet will hit
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out predictedHit, 1000f, bulletLayerMask) && playerBullet == true)
        {
            Vector3 recoilFactoredHitPoint = predictedHit.point;
            //Work out recoil if needed
            if (recoil != 0)
            {
                //if you are very close to the target, reduce recoil
                if (predictedHit.distance <= 8)
                {
                    //Store the distance value
                    float distance = predictedHit.distance;
                    //Reduce recoil based on how close you are
                    if (distance <= 1)
                    {
                        recoil = recoil / 6;
                    }
                    else if (distance <= 2)
                    {
                        recoil = recoil / 5;
                    }
                    else if (distance <= 3)
                    {
                        recoil = recoil / 4;
                    }
                    else if (distance <= 5.5)
                    {
                        recoil = recoil / 2;
                    }
                    else if (distance <= 8)
                    {
                        recoil = recoil / 1.5f;
                    }
                }
                //Hold the hit point for the bullet
                recoilFactoredHitPoint = new Vector3(predictedHit.point.x + Random.Range(-recoil, recoil), predictedHit.point.y + Random.Range(-recoil, recoil), predictedHit.point.z + Random.Range(-recoil, recoil));
            }
            //Alter the course of the bullet
            transform.LookAt(recoilFactoredHitPoint);
        }
    }

    //this is code for hitting drone
    private void DroneHitConditions()
    {
        if (storedHit.collider.GetComponent<Drone>() != null)
        {
            Drone droneScript = storedHit.collider.GetComponent<Drone>();
            Rigidbody droneRB = storedHit.collider.GetComponent<Rigidbody>();
            if (droneScript.ReturnIfStunned() == false)
            {
                droneScript.StopCoroutine(droneScript.ActivateStun(0.5f));
                droneScript.StartCoroutine(droneScript.ActivateStun(0.2f));
            }
            droneRB.AddForce(transform.forward * 1000);
        }
    }

    private void CreateBulletDecal() //This will create a bullet decal
    {
        //Mark the hit object with the bullet decal
        GameObject decal = shootingScript.ReturnDecalObjectPool()[0];
        if (decal != null)
        {
            //Remove any existing decal follow scripts
            if (decal.GetComponent<DecalFollow>() != null)
            {
                Destroy(decal.GetComponent<DecalFollow>());
            }
            //Unparent the decal from anything else
            decal.transform.SetParent(null);
            //Get the default size of the decal
            Vector3 defaultDecalScale = decal.transform.localScale;
            //Remove the decal from the object pool
            shootingScript.ReturnDecalObjectPool().Remove(decal);
            //Add the decal to the end of the pool
            shootingScript.ReturnDecalObjectPool().Add(decal);
            //Work out where the decal will spawn
            Vector3 decalSpawn = new Vector3(storedHit.point.x, storedHit.point.y, storedHit.point.z);
            //Position the decal on the object
            decal.transform.SetPositionAndRotation(decalSpawn, Quaternion.FromToRotation(Vector3.forward, storedHit.normal));
            //Create a parent
            GameObject followPoint = new GameObject();
            followPoint.name = "decalFollowPointObj";
            followPoint.transform.SetPositionAndRotation(decalSpawn, Quaternion.FromToRotation(Vector3.forward, storedHit.normal));
            followPoint.transform.SetParent(storedHit.collider.transform);
            //Give bullet decals a parent to tidy up the scene
            decal.transform.SetParent(bulletDecalParent.transform);
            //Set the decal parent to the object if it is not rotating
            decal.gameObject.AddComponent<DecalFollow>().followZone = followPoint.transform;
            decal.SetActive(true);
        }
    }

    private void RepoolBullet() //This will repool the bullet
    {
        //Stop the repool automatic code
        StopAllCoroutines();
        //Add the bullet to the end of the object pool
        shootingScript.ReturnBulletObjectPool().Add(this.gameObject);
        //Turn this bullet off
        this.gameObject.SetActive(false);
    }

    private IEnumerator PutMeBackInThePool() //Clean up the bullet if it has not done its thing within 3 seconds
    {
        yield return new WaitForSeconds(3);
        RepoolBullet();
    }
    public void StopBulletShotEnvironment()
    {
        if (AudioManage.inst.bulletEnvironment.isPlaying == true)
        {
            AudioManage.inst.bulletEnvironment.Stop();
        }

    }
}


