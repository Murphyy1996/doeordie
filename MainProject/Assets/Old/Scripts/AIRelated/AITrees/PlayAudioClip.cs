using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PlayAudioClip : Action
{

    public AudioSource audioSource;
    //public AudioClip clipToPlay;

    public override TaskStatus OnUpdate()
    {
        audioSource.enabled = true;
        //audioSource.clip = clipToPlay;
        //StartCoroutine("TurnOffClip");

        return TaskStatus.Success; 
    }

    IEnumerator TurnOffClip()
    {
        yield return new WaitForSeconds(0.3f);
        audioSource.enabled = false;
        yield return null;
    }


}
