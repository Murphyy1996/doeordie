using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicBook : MonoBehaviour
{

    [SerializeField]
    GameObject[] panels;

    int arrayPos;

    // Use this for initialization
    void Start()
    {
        arrayPos = 0;

        for (int i = 0; arrayPos < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

    }
	
    public void NextPanel()
    {
        panels[arrayPos].SetActive(true);

        arrayPos++;
    }

}
