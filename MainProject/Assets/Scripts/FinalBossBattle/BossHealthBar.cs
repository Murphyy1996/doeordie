using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100, currentHealth = 100;
    [SerializeField]
    private Image bossCurrentHealthImage;
    [SerializeField]
    private ReusableHealth healthScriptToMimic;

    private void Awake()
    {
        bossCurrentHealthImage = GameObject.Find("CurrentHealthImage").GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        if (healthScriptToMimic != null)
        {
            currentHealth = healthScriptToMimic.healthValue;
            float floatCurrentHealth = currentHealth;
            float floatMaxHealth = maxHealth;
            bossCurrentHealthImage.fillAmount = floatCurrentHealth / floatMaxHealth;
            if (currentHealth <= 0)
            {
                bossCurrentHealthImage.enabled = false;
            }
        }
    }

    public void ApplyBossDamage(int damageToApply) //Apply damage to this script
    {
        currentHealth = currentHealth - damageToApply;
        //Update the health text
        float floatCurrentHealth = currentHealth;
        float floatMaxHealth = maxHealth;
        bossCurrentHealthImage.fillAmount = floatCurrentHealth / floatMaxHealth;
        if (currentHealth <= 0)
        {
            bossCurrentHealthImage.enabled = false;
        }
    }
}
