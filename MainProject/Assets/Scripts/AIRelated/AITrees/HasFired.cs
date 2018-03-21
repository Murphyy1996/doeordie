using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Check if the player has fired nearby an AI")]

public class HasFired : Conditional
{

    private Transform playerTransform;
    private float maxShotDetection = 15f, playerShotNearby;

    public override void OnStart()
    {
        base.OnStart();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override TaskStatus OnUpdate()
    {

        playerShotNearby = Vector3.Distance(playerTransform.position, transform.position);

        if (playerTransform.GetComponent<Shooting>().IsPlayerShooting())
        {

            if (playerShotNearby <= maxShotDetection)
            {
                return TaskStatus.Success; 
            }
           
        }

        return TaskStatus.Failure;
    }
        

}