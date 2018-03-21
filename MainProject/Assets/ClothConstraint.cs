using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothConstraint : MonoBehaviour
{

    Cloth cloth;
    Vector3 rotationLast;
    float velocity;

    void Start()
    {
        cloth = GetComponent<Cloth>();
    }
	
    void Update()
    {
        //print(velocity);
    }

    void FixedUpdate()
    {
        rotationLast = transform.position;
        velocity = ((transform.position - rotationLast).magnitude) / Time.deltaTime;


        if (velocity >= 3f)
        {
            cloth.ClearTransformMotion();
        }

    }
        
}
