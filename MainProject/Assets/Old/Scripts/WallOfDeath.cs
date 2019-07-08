using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfDeath : MonoBehaviour
{

    [SerializeField]
    float speed = 1f;
    [SerializeField]
    Transform[] checkpoints;

    private int currentCheckpoint = 0;
    private bool allowedToMove = true;
	
    // Update is called once per frame
    void FixedUpdate()
    {
        if (allowedToMove == true)
        {
            float step = speed * Time.deltaTime;
            //Moves the object
            transform.position = Vector3.MoveTowards(transform.position, checkpoints[currentCheckpoint].position, step);

            //If you are within 0.5 metres
            if (Vector3.Distance(transform.position, checkpoints[currentCheckpoint].transform.position) <= 0.5f)
            {
                //Set to next checkpopint
                currentCheckpoint++;

                //Stop movement if the last checkpoint has been reached
                if (currentCheckpoint > checkpoints.Length)
                {
                    allowedToMove = false;
                }
            }

        }
//        transform.position = Vector3.Lerp(transform.position, checkpoints.position, lerpSpeed);
//        transform.rotation = Quaternion.Slerp(transform.rotation, playerPosition.rotation, lerpSpeed);
    }
}
