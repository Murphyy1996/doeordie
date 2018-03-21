//Author: James Murphy
//Purpose: To set the quest template on this game object as the current quest
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetThisQuestOnStart : MonoBehaviour
{
    private void Awake()
    {
        Invoke("DelayedAwake", 0.2f);   
    }

    private void DelayedAwake() //Set this quest as soon as the scene starts
    {
        if (QuestManager.inst != null && GetComponent<QuestTemplate>() != null)
        {
            GetComponent<QuestTemplate>().SetQuest();
        }
        Destroy(this);
    }
}
