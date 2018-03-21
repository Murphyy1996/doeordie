using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Check if AI has been shot")]

public class AIShot : Conditional
{
    ReusableHealth reusableHealth;
    Animator rangedAnim;
    int currentHealth;
    public bool hasBeenHit = false;

    public override void OnStart()
    {
        base.OnStart();

        reusableHealth = GetComponent<ReusableHealth>();
        rangedAnim = GetComponent<Animator>();

        currentHealth = reusableHealth.healthValue;
    }

    public override TaskStatus OnUpdate()
    {

        currentHealth = reusableHealth.healthValue;

        if (currentHealth < reusableHealth.maxHealth)
        {

            if (this.gameObject.tag == "enemy")
            {
                StartCoroutine("Reset");
            }
            return TaskStatus.Success;
        }
   
       return TaskStatus.Failure;
    }

    IEnumerator Reset()
    {
        hasBeenHit = true;
        Debug.Log("reset");
        yield return new WaitForSeconds(0.5f);

        hasBeenHit = false;

        yield return TaskStatus.Success;
    }
        

}
