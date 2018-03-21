//Author: James Murphy

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTurret : MonoBehaviour
{
    private Shooting shooting;
    [SerializeField]
    private Transform bulletSpawn;
    [SerializeField]
    private float durationBetweenShots;

    private void Awake()
    {
        shooting = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooting>();
        InvokeRepeating("ShootBullet", 0, durationBetweenShots);
    }

 

    private void ShootBullet()
    {
        if (shooting.gameObject.activeSelf == true)
        {
            GameObject bullet = shooting.ReturnBulletObjectPool()[0];

            bullet.GetComponent<Bullet>().bulletDamage = 2;
            bullet.GetComponent<Bullet>().playerBullet = false;

            shooting.ReturnBulletObjectPool().Remove(bullet);

            bullet.transform.rotation = bulletSpawn.transform.rotation;
            bullet.transform.position = bulletSpawn.transform.position;

            bullet.SetActive(true);
        }
    }
}
