using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class Reference : MonoBehaviour
{
    
    private BehaviorTree meleeTree;
    [SerializeField] [Tooltip("Drag in an empty gameobject")] private GameObject lookObj;
    public GameObject playerTarget;
    public GameObject body;
    public List<GameObject> waypoint = new List<GameObject>();


    void Start()
    {
        meleeTree = GetComponent<BehaviorTree>();
        SetLocationVars(meleeTree);
        SpawnLookObject();
    }

    void Update()
    {
        SetLocationVars(meleeTree);
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
