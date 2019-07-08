using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Check if ai has been damaged")]

public class IsDamaged : Conditional
{

    ReusableHealth reusableHealth;
    private int currentHealth;

    public override void OnStart()
    {
        base.OnStart();

        reusableHealth = GetComponent<ReusableHealth>();

        currentHealth = reusableHealth.healthValue;
    }

    public override TaskStatus OnUpdate()
    {
        currentHealth = reusableHealth.healthValue;

        if (currentHealth <= 15)
        {
            if (this.gameObject.tag == "enemy")
            {
                return TaskStatus.Success;
            }

        }

        return TaskStatus.Failure;
    }

}
