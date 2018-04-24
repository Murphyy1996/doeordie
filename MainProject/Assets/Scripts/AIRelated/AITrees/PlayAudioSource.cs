using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PlayAudioSource : Action
{

    public AudioSource audioSource;

    public override TaskStatus OnUpdate()
    {
        audioSource.enabled = true;

        StartCoroutine("TurnOffClip");

        return TaskStatus.Success; 
    }

    IEnumerator TurnOffClip()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.enabled = false;
        yield return null;
    }


}
