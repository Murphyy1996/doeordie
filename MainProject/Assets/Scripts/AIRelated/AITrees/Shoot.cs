using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Behaviour to allow the AI to shoot")]

public class Shoot : Action 
{

    Shooting shooting;
    Animator rangedAnim;
    GameObject bullet, muzzleFlash;
    Transform playerTransform, bulletOrigin;
    int shootDamage = 10;

    public override void OnStart()
    {
        rangedAnim = GetComponent<Animator>();

        bulletOrigin = transform.Find("Character1_Ctrl_Reference/Character1_Ctrl_RightWristEffector/polySurface9/BulletOrigin").transform;

        muzzleFlash = transform.Find("Character1_Ctrl_Reference/Character1_Ctrl_RightWristEffector/polySurface9/MuzzleObj").gameObject;
        muzzleFlash.SetActive(false);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shooting = playerTransform.GetComponent<Shooting>();
    }

    public override TaskStatus OnUpdate()
    {
        StartCoroutine("DelayedShoot");
        StartCoroutine("Muzzle");

        return TaskStatus.Success; 
    }

    private IEnumerator DelayedShoot()
    {
        yield return new WaitForSeconds(1f);

        bullet = shooting.ReturnBulletObjectPool()[0];
        bullet.GetComponent<Bullet>().bulletOrigin = bulletOrigin;
        bullet.GetComponent<Bullet>().recoil = 0.8f;
        bullet.GetComponent<Bullet>().bulletDamage = shootDamage;
        bullet.GetComponent<Bullet>().playerBullet = false;

        shooting.ReturnBulletObjectPool().Remove(bullet);

        bullet.transform.rotation = bulletOrigin.transform.rotation;
        bullet.transform.position = bulletOrigin.transform.position;

        bullet.SetActive(true);

        yield break;
    }

    private IEnumerator Muzzle()
    {
        yield return new WaitForSeconds(1f);
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
   
        yield break;
    }
        
        

}
