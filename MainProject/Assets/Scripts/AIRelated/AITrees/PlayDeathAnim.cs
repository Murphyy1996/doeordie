using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;

[TaskDescription("Play the death anim for this enemy only")]

public class PlayDeathAnim : Action
{

    private Animator meleeAnim;
    private BehaviorTree meleeTree;
    AimIK aimIK;
    LookAtIK lookAtIk;

    public override void OnStart()
    {
        meleeAnim = GetComponent<Animator>();
        meleeTree = GetComponent<BehaviorTree>();
        aimIK = GetComponent<AimIK>();
        lookAtIk = GetComponent<LookAtIK>();
    }

    public override TaskStatus OnUpdate()
    {

        if (this.gameObject.tag == "enemy")
        {
            meleeAnim.SetFloat("Speed", 1.5f, 0.07f, Time.deltaTime);
            meleeAnim.SetBool("isDead", true);

            aimIK.solver.target = null; //Prevent gun following player after death
            lookAtIk.solver.target = null;
         
            return TaskStatus.Success;
        }
        else
        {
            meleeAnim.SetBool("isDead", false);
        }
           
        return TaskStatus.Failure;
    }

}
