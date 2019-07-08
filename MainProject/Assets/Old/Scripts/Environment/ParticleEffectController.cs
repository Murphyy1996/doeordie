using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{

    //Authors : Kate Georgiou and Ross Perry

    public static ParticleEffectController inst;
    [SerializeField]
    private ParticleSystem teleport, enemydamage;
    private GameObject player;

    private void Awake()
    {
        inst = this;
    }
    
    // Use this for initialization
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        teleport = player.GetComponent<ParticleSystem>();
    }

    public void TeleParticle()
    {
        StopCoroutine(tele());
        teleport.Stop();
        StartCoroutine(tele());
    }

    public void EnemyParticle(ParticleSystem enemypart)
    {
        enemydamage = enemypart;
        if (enemydamage != null)
        {
            enemydamage.Stop();
            StartCoroutine(damage());
        }      
    }

    private IEnumerator tele()
    {
        teleport.Play();
        yield return new WaitForSeconds(2);
        teleport.Stop();
    }

    private IEnumerator damage()
    {
        if (enemydamage != null)
        {
            if (enemydamage != null)
            {
                enemydamage.Play();
            }
            yield return new WaitForSeconds(2);
            if (enemydamage != null)
            {
                enemydamage.Stop();
            }
        }
        yield return null;
    }
}
