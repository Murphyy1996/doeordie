using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject scenePlatform;
    [SerializeField] private GameObject platformDestination;
    [SerializeField] float platformSpeed = 1f;

    Vector3 platformPos;

    void Start()
    {
        platformPos = transform.position;
    }
	
    void Update()
    {
        if (transform.position.x >= platformDestination.transform.position.x)
        {
            transform.position = platformPos;
        }

        if (gameObject != null)
        {
            MovePlatform();
        }
                 
    }

    void MovePlatform()
    {
        transform.Translate(Vector3.right * Time.deltaTime * platformSpeed);
    }
        

}
