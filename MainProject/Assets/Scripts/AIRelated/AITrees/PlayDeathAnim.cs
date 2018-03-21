using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Play the death anim for this enemy only")]

public class PlayDeathAnim : Action
{

    private Animator meleeAnim;
    private BehaviorTree meleeTree;

    public override void OnStart()
    {
        meleeAnim = GetComponent<Animator>();
        meleeTree = GetComponent<BehaviorTree>();
    }

    public override TaskStatus OnUpdate()
    {

        if (this.gameObject.tag == "enemy")
        {

            meleeAnim.SetBool("isDead", true);
         
            return TaskStatus.Success;
        }
        else
        {
            meleeAnim.SetBool("isDead", false);
        }
           
        return TaskStatus.Failure;
    }

}
