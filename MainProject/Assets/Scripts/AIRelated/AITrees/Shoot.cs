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

        bulletOrigin = transform.Find("Arm_out:Character1_Reference/Arm_out:Character1_Hips/Arm_out:Character1_Spine/Arm_out:Character1_Spine1/Arm_out:Character1_Spine2/Arm_out:Character1_RightShoulder/Arm_out:Character1_RightArm/Arm_out:Character1_RightForeArm/Arm_out:Character1_RightHand/Gun/BulletOrigin").transform;

        muzzleFlash = transform.Find("Arm_out:Character1_Reference/Arm_out:Character1_Hips/Arm_out:Character1_Spine/Arm_out:Character1_Spine1/Arm_out:Character1_Spine2/Arm_out:Character1_RightShoulder/Arm_out:Character1_RightArm/Arm_out:Character1_RightForeArm/Arm_out:Character1_RightHand/Gun/MuzzleObj").gameObject;
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
        bullet.GetComponent<Bullet>().recoil = 0.5f;
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
