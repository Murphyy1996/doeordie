using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine;

//This isnt in use Author: Kate Georgiou
public static class SavingLoading
{
    public static List<GameStuff> saves = new List<GameStuff>();

    public static void SaveGame()
    {
        saves.Add(GameStuff.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Environment.CurrentDirectory + "/savedGames.ml");
        bf.Serialize(file, SavingLoading.saves);
        file.Close();
        Debug.Log("saving");
    }

    public static void LoadGame()
    {
        if (File.Exists(Environment.CurrentDirectory + "/savedGames.ml"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Environment.CurrentDirectory + "/savedGames.ml", FileMode.Open);
            SavingLoading.saves = (List<GameStuff>)bf.Deserialize(file);
            file.Close();
            Debug.Log("loading");
        }

    }



}