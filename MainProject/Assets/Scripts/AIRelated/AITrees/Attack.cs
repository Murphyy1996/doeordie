using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Manages attack animations and applies damage to the player")]

public class Attack : Action
{

    public int damageToPlayer = 5;
    private Transform playerTransform;

    public override void OnStart()
    {
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //Look at player
        Vector3 tarPos = playerTransform.position - transform.position;
        Quaternion tarRot = Quaternion.LookRotation(tarPos);

        //Turn around quickly
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, Time.smoothDeltaTime * 50f); //Add variable for alert turn speed

        //Apply damage when player is hit
        playerTransform.GetComponent<ReusableHealth>().ApplyDamage(damageToPlayer);

        return TaskStatus.Success; 

    }
        

}
