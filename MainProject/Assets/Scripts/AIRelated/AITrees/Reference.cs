using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class Reference : MonoBehaviour
{
    
    private BehaviorTree meleeTree;
    [SerializeField] [Tooltip("Drag in an empty gameobject")] private GameObject lookObj;
    public GameObject playerTarget;
    public GameObject body;
    public List<GameObject> waypoint = new List<GameObject>();

    CanSeeObject canSeeObject;

    void Start()
    {
        meleeTree = GetComponent<BehaviorTree>();
        SetLocationVars(meleeTree);
        SpawnLookObject();

        canSeeObject = GetComponent<CanSeeObject>();
    }

    void Update()
    {
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
