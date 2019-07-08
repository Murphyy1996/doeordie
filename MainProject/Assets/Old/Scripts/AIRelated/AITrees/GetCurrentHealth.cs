using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Get the current health of the AI")]

public class GetCurrentHealth : Conditional
{
    ReusableHealth reusableHealth;
    public SharedInt currentAIHealth;

    public override void OnStart()
    {
        base.OnStart();

        currentAIHealth = (SharedInt)GlobalVariables.Instance.GetVariable("currentHealth");
        reusableHealth = GetComponent<ReusableHealth>();
    }

    public override TaskStatus OnUpdate()
    {
        if (gameObject.tag == "enemy")
        {
            currentAIHealth.Value = reusableHealth.healthValue;
        }

        return TaskStatus.Running;
    }

}
