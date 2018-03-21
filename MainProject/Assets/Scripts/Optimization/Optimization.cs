//Author: James Murphy
//Purpose: To clean up the scene with listed functions
//Requirements: Place it on the player

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimization : MonoBehaviour
{
    public static Optimization singleton;
    private List<GameObject> emptyRotationZones = new List<GameObject>();

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        Invoke("RemoveAudioListeners", 0.01f);
    }

    public void AddRotationZoneToList(GameObject rotationZone)
    {
        emptyRotationZones.Add(rotationZone);
    }

    public void RemoveAllRotationZones() //Remove all rotation zones in the scene
    {
        if (emptyRotationZones.Count > 0)
        {
            foreach (GameObject rZone in emptyRotationZones)
            {
                Destroy(rZone);
            }
            emptyRotationZones.Clear();
        }
    }

    private void RemoveAudioListeners()
    {
        Camera mainCamera = Camera.main;
        AudioListener[] foundListeners = FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
        foreach(AudioListener listeners in foundListeners)
        {
            if (listeners.transform.gameObject != mainCamera.gameObject)
            {
                Destroy(listeners);
            }
        }
    }
}
