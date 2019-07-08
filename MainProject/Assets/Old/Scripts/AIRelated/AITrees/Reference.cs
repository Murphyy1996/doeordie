using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using RootMotion.FinalIK;

public class Reference : MonoBehaviour
{

    private BehaviorTree meleeTree;
    [SerializeField] [Tooltip("Drag in an empty gameobject")] private GameObject lookObj;
    public GameObject playerTarget;
    public GameObject body;
    public List<GameObject> waypoint = new List<GameObject>();
    public bool isAttackingPlayer = false;
    private ReusableHealth thisReusableHealth;
    private bool runOnceDeathCode = false;

    CanSeeObject canSeeObject;

    void Start()
    {
        meleeTree = GetComponent<BehaviorTree>();
        SetLocationVars(meleeTree);
        SpawnLookObject();

        try
        {
            canSeeObject = GetComponent<CanSeeObject>();
        }
        catch
        {
            print("Error getting the can see object");
        }
        if (GetComponent<ReusableHealth>() != null)
        {
            thisReusableHealth = GetComponent<ReusableHealth>();
        }
    }

    void Update()
    {
        //control the value of is attacking player
        if (canSeeObject != null)
        {
            isAttackingPlayer = canSeeObject.spottedPlayer.Value;
        }
        else
        {
            isAttackingPlayer = false;
        }
        //This will be run when this object dies
        if (thisReusableHealth != null && runOnceDeathCode == false)
        {
            if (thisReusableHealth.behaviourHealth.Value <= 0)
            {
                //So this dont run again
                runOnceDeathCode = true;

                //Mark this enemy as not attacking the player any more
                isAttackingPlayer = false;

                //Track the amount of enemies attacking the player
                int enemiesAttackingPlayer = 0;

                //Get all enemy objects
                GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("enemy");

                //Check what drones are attacking the player
                foreach (GameObject enemy in foundEnemies)
                {
                    //If the enemy is a drone
                    if (enemy.layer == 19)
                    {
                        if (enemy.GetComponent<Drone>().isAttackingPlayer == true)
                        {
                            enemiesAttackingPlayer++;
                        }
                    }
                    else //Get the reference script
                    {
                        if (enemy.GetComponent<Reference>() != null)
                        {
                            if (enemy.GetComponent<Reference>().isAttackingPlayer == true)
                            {
                                enemiesAttackingPlayer++;
                            }
                        }
                    }
                }

                //If there are no enemies attacking the player, stop music
                if (enemiesAttackingPlayer == 0)
                {
                    try
                    {
                        if (AudioManage.inst.combatMusic.isPlaying == true)
                        {
                            AudioManage.inst.combatMusic.Stop();
                        }
                    }
                    catch
                    {
                        print("death combat music error");
                    }
                }
                //Destroy all scripts that could fuck with the sound
                Destroy(this);
                if (GetComponent<LookAtIK>() != null)
                {
                    Destroy(GetComponent<LookAtIK>());
                }
            }
        }
        //Plau music based off is attacking player variable
        if (isAttackingPlayer == true)
        {
            if (AudioManage.inst.combatMusic.isPlaying == false)
            {
                AudioManage.inst.combatMusic.Play();
            }
        }
        SetLocationVars(meleeTree);
        /* if (canSeeObject.spottedPlayer.Value)
         {
             AudioManage.inst.combatMusic.Play();
             Debug.Log("turning combat music on");
         }
         else
         {
             AudioManage.inst.combatMusic.Stop();
             Debug.Log("turning combat music off");
         }*/
    }

    public void SetLocationVars(BehaviorTree behaviorTree)
    {
        behaviorTree.SetVariableValue("target", playerTarget);
        behaviorTree.SetVariableValue("playerBody", body);

    }

    void SpawnLookObject()
    {
        GameObject lookClone = Instantiate(lookObj, transform.position, Quaternion.identity) as GameObject;
        lookClone.transform.SetParent(gameObject.transform, false);
        lookClone.transform.localPosition = Vector3.zero;
        lookClone.name = "lookClone" + this.gameObject.name;
        lookClone.tag = "Look";
        lookObj = lookClone;
    }

}
