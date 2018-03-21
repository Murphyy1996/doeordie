//Author: Kate Georgiou

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallShake : MonoBehaviour
{

    CharacterControllerMovement CM;
    CharacterController thisCC;
    BossCameraShake BS;
    Vector3 down;
    [SerializeField]
    [Range(-25f, -0.1f)]
    private float amountToTriggerShake = -12;
    [SerializeField]
    private float shakeAmount = 0.1f, shakeTime = 0.5f;
    [SerializeField]
    private LayerMask layer;
    private float starterCounter = 0, counterTarget = 2;

    // Use this for initialization
    void Start()
    {

        CM = GetComponent<CharacterControllerMovement>();
        thisCC = GetComponent<CharacterController>();
        down = -transform.up;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        starterCounter = starterCounter + Time.fixedDeltaTime;

        if (starterCounter > counterTarget)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, down, out hit, 3f, layer) && thisCC.velocity.y < amountToTriggerShake) 
            {
                if (GetComponent<BossCameraShake>() == null)
                {
                    BS = this.gameObject.AddComponent<BossCameraShake>();
                    BS.ShakeitShakeit(shakeAmount, shakeTime);
                }
            }
        }
    }
}
