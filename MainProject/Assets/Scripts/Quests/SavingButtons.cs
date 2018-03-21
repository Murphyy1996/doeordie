using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingButtons : MonoBehaviour
{


    public void Load()
    {
        SavingLoading.LoadGame();
    }

    public void Save()
    {
        SavingLoading.SaveGame();
    }



}
