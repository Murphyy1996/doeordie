using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Manages whether AI can Melee attack and sets bool to attack")]

public class CanAttack : Action
{

    private float timer, timeBetweenMelee = 3f;
    private Animator meleeAnim;

    public bool canAttack;

    public override void OnStart()
    {
        base.OnStart();
        meleeAnim = GetComponent<Animator>();
        timer = 3f;
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log(canAttack);


        timer += Time.deltaTime;

        if (timer >= timeBetweenMelee) //if they can attack and time between attack is possible
        {
            canAttack = true;
            return TaskStatus.Success;  
        }
        else
        {
            canAttack = false;
            return TaskStatus.Failure;
        }
            

    }
        
}