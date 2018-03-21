using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Check if this this instance of the AI has died")]


public class CheckIfDead : Conditional
{

    private int currentHealth;
    private NavMeshAgent navMeshAgent;

    public override void OnStart()
    {
        currentHealth = this.gameObject.GetComponent<ReusableHealth>().healthValue;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        currentHealth = this.gameObject.GetComponent<ReusableHealth>().healthValue;
        if (currentHealth <= 0)
        {   
            
            if (this.gameObject.tag == "enemy")
            {
                navMeshAgent.enabled = false;
                return TaskStatus.Success;
            }

        }
         
        return TaskStatus.Failure;
    }


}
