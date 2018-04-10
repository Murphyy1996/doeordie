using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PlayAudioClip : Action
{

    AudioSource audioSource;

    public override void OnStart()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override TaskStatus OnUpdate()
    {
        audioSource.enabled = true;

        return TaskStatus.Success; 
    }


}
