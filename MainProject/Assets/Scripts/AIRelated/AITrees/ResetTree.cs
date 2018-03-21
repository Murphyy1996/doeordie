using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Resets the tree")]

public class ResetTree : Action 
{

    private BehaviorTree meleeTree;

    public override void OnStart()
    {
        meleeTree = GetComponent<BehaviorTree>();
    }

    public override TaskStatus OnUpdate()
    {
        meleeTree.DisableBehavior();
        meleeTree.EnableBehavior();
        meleeTree.SetVariableValue("updatePosition", false);
        return TaskStatus.Success;
    }

}
