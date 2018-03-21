using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Check if the ai has died")]

public class IsAlive : Conditional
{

    ReusableHealth reusableHealth;
    private Animator meleeAnim;
	
    public override void OnStart()
    {
        base.OnStart();

        reusableHealth = GetComponent<ReusableHealth>();
        meleeAnim = GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (reusableHealth.healthValue < 0)
        {
            if (this.gameObject.tag == "enemy")
            {
                return TaskStatus.Failure;
            }
          
        }
        return TaskStatus.Success;
       
    }
        

}
