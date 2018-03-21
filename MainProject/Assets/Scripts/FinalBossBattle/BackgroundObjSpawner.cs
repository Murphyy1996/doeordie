using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjSpawner : MonoBehaviour
{
    [SerializeField]
    [Header("Objects to spawn")]
    private GameObject[] prefabSpawnArray;
    [SerializeField]
    [Header("Spawn rate settings")]
    private float minimumSpawnRate = 0;
    [SerializeField]
    private float maximumSpawnRate = 10;
    private float selectedSpawnTime = 0;
    private float timer = 0;
    [SerializeField]
    private bool unparentChild = false, useExistingTags = false;
    [SerializeField]
    private bool limitAmountOfObjects = false;
    [SerializeField]
    private int maxAmountOfObjects = 0;
    [SerializeField]
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        //Turn off any mesh renderers
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().enabled = false;
        }
        //Randomly select the spawn time
        SelectNewSpawnTime();
    }

    private void FixedUpdate()
    {
        //Only run any code if there are objects to spawn
        if (prefabSpawnArray.Length > 0)
        {
            //Increase the timer
            timer = timer + Time.fixedDeltaTime;

            //Only spawn if the timer is bigger than the selected spawn time
            if (timer >= selectedSpawnTime)
            {
                if (limitAmountOfObjects == true) //Remove any empties
                {
                    foreach (GameObject item in spawnedObjects)
                    {
                        if (item == null)
                        {
                            spawnedObjects.Remove(item);
                        }
                    }
                }
                if (limitAmountOfObjects == false || limitAmountOfObjects == true && spawnedObjects.Count <= maxAmountOfObjects)
                {
                    //Spawn the object
                    GameObject spawnedObj = Instantiate(prefabSpawnArray[Random.Range(0, prefabSpawnArray.Length)], transform.position, transform.rotation) as GameObject;
                    if (useExistingTags == false)
                    {
                        spawnedObj.name = "SpawnedObj";
                        spawnedObj.tag = "BackgroundObj";
                    }
                    if (unparentChild == false)
                    {
                        spawnedObj.transform.SetParent(transform);
                    }
                    if (limitAmountOfObjects == true)
                    {
                        spawnedObjects.Add(spawnedObj);
                    }
                }
                //Prepare for the next spawn
                SelectNewSpawnTime();
            }
        }
        else //Destroy this spawner if there is nothing to spawn
        {
            Destroy(this);
        }
    }

    private void SelectNewSpawnTime() //Randomly select a new spawn time
    {
        //Randomly select the spawn time
        selectedSpawnTime = Random.Range(minimumSpawnRate, maximumSpawnRate);
        //Set the timer back to zero
        timer = 0;
    }
}
