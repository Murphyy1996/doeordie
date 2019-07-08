using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReward : MonoBehaviour
{
    //Author: Kate Georgiou 17/10/17
    public static QuestReward inst;
    private bool itemStolen = false, spokenToVip = false, VIPkilled = false, allKilled = false, inTrigger = false;
    [SerializeField]
    private GameObject targetToKill;
    private int enemiesKilled, enemiesFound;
    [SerializeField]
    private int timer, increaseGrapplRangeBy, increaseTeleStrengthBy, increaseMaxHealthBy;

    private void Awake()
    {
        inst = this;
    }

    private void OnLevelWasLoaded(int level)
    {
        GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in foundEnemies)
        {
            enemiesFound++;
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (QuestManager.inst.currentQuest != null)
        {
            switch (QuestManager.inst.currentQuest.name) //can be expanded as quests are added.
            {
               

                case "Steal the item":
                    if (itemStolen == true)
                    {

                        Grapple grapple = GetComponent<Grapple>();
                        grapple.SetGrappleRange(grapple.ReturnGrappleRange() + increaseGrapplRangeBy);
                        StolenBoolReset();
                        //quest rewards here - grapplinh hook range extend
                    }
                    break;

                case "Speak to the VIP":

                    if (spokenToVip == true)
                    {
                     
                        GetComponent<Grapple>().SetGrappleAllowedValue(true);
                        SpokenToVIPBoolFalse();
                    }

                    break;

                case "Kill the VIP":
                    if (VIPkilled == true)
                    {
                        Teleporting tele = GetComponent<Teleporting>();
                       
                        tele.SetNewStrength(tele.ReturnStregnthValue() + increaseTeleStrengthBy);
                        
                        killVIPFalse();
                    }
                    break;

                case "Kill all enemies":
                    if (allKilled == true)
                    {
                        GetComponent<ReusableHealth>().maxHealth =+increaseMaxHealthBy;
                    
                        KillBoolFalse();
                    }
                    break;

                case "Get to this point in time":

                    if (inTrigger == true)
                    {
                        GetComponent<Teleporting>().SetTeleportEnabledValue(true);
                        
                        TriggerFalse();
                    }

                    break;
            }

            if (enemiesFound == enemiesKilled)
            {
                KillBoolTrue();
            }
            timer--;
        }
    }

    public void StolenBoolTrue()
    {
        itemStolen = true;
       

    }
    public void StolenBoolReset()
    {
        itemStolen = false;
    }

    public void SpokenToVIPBoolTrue()
    {
        spokenToVip = true;
    }

    public void SpokenToVIPBoolFalse()
    {
        spokenToVip = false;
    }
    public GameObject RetriveTargetGO()
    {
        return targetToKill;
    }
    public void killVIPTrue()
    {
        VIPkilled = true;
    }
    public void killVIPFalse()
    {
        VIPkilled = false;
    }

    public void KillBoolTrue()
    {
        allKilled = true;
    }
    public void KillBoolFalse()
    {
        allKilled = false;
    }

    public void AddToCounter()
   {
        enemiesKilled++;
    }

    public void TriggerTrue()
    {
        inTrigger = true;
    }
    public void TriggerFalse()
    {
        inTrigger = false;
    }
    public int retriveTimer()
    {
        return timer;
    }
}
