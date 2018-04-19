using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfDeath : MonoBehaviour
{
    [SerializeField]
    float lerpSpeed = 1f;

    [SerializeField]
    Transform playerPosition; 

    // Use this for initialization
    void Start()
    {
		
    }
	
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, playerPosition.position, lerpSpeed);
//        transform.rotation = Quaternion.Slerp(transform.rotation, playerPosition.rotation, lerpSpeed);
    }
}
