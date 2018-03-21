using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTemplate : MonoBehaviour
{
    //Author: Kate Georgiou 17/10/17
    public string name, description, reward;

    public void SetQuest() //Will pick up this quest if it hasn't been completed or picked up
    {
        if (QuestManager.inst.CheckIfQuestPickedUp(this) == false && QuestManager.inst.CheckIfQuestCompleted(this) == false)
        {
            QuestManager.inst.pickedUpQuests.Add(this);
            if (QuestManager.inst.currentQuest == null)
            {             
                QuestManager.inst.currentQuest = this;
                QuestManager.inst.UpdateQuestText();
            }
            else //Complete the old current quest and add this as the new current
            {
                QuestManager.inst.complete.Add(QuestManager.inst.currentQuest);
                QuestManager.inst.pickedUpQuests.Remove(QuestManager.inst.currentQuest);
                QuestManager.inst.currentQuest = this;
                QuestManager.inst.UpdateQuestText();
            }
        }
       
    }
}
