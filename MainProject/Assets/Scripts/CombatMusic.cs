using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class CombatMusic : MonoBehaviour {

    // [SerializeField]
    //  private BehaviorTree tree;  //its not getting the reference to the tree
    public static CombatMusic inst;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
     }

    // Use this for initialization
    void Start ()
    {
       // tree = GetComponent<BehaviorTree>(); //is the behaviour tree attached to the player? 
	}

    // Update is called once per frame
    void Update()
    {
      /*  Debug.Log("update runnnnns");
        if (tree != null)
        {
            Debug.Log("runrunrunrun");
            SharedBool playerSpottedBool = (SharedBool)tree.GetVariable("spottedPlayer");
            if (playerSpottedBool.Value == true && playerSpottedBool != null)
            {
                Debug.Log("I CAN SEE CLEARLY NOW" + playerSpottedBool);
                //make it play the audio track while this is happening and then turn it off in an else
               // AudioManage.inst.combatMusic.Play();
            }
            else
            {
                Debug.Log("cannae see clear" + tree.name);
                // AudioManage.inst.combatMusic.Stop();
            }
            Debug.Log(playerSpottedBool);
        }
        else
        {
            tree = GetComponent<BehaviorTree>();
            Debug.Log("FINDING  TREE");
        }*/
        
       
    }

    public void PlayCombat()
    {
        // AudioManage.inst.combatMusic.Play();
        Debug.Log("combat music play");
    }
}
