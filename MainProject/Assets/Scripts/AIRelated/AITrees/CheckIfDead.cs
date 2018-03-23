using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskDescription("Check if this this instance of the AI has died")]


public class CheckIfDead : Conditional
{
    
    private int currentHealth;
    private NavMeshAgent navMeshAgent;
    private BehaviorTree rangedTree;

    public override void OnStart()
    {
        rangedTree = GetComponent<BehaviorTree>();
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
                rangedTree.FindTask<CanSeeObject>().spottedPlayer = false;
                return TaskStatus.Success;
            }

        }
         
        return TaskStatus.Failure;
    }
        


}
