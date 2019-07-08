using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Run hit animation when AI is shot")]

public class HitAnim : Action
{
    public AIShot aiShot;
    Animator rangedAnim;

    public override void OnStart()
    {
        //base.OnStart();

        rangedAnim = GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {

        if (aiShot.hasBeenHit == true)
        {
            StartCoroutine("PlayHitAnim");
        }

        return TaskStatus.Inactive;
    }

    public IEnumerator PlayHitAnim()
    {
        rangedAnim.SetTrigger("Damaged");

        yield return new WaitForSeconds(0.5f);
     
        yield return TaskStatus.Success;
    }
        
}