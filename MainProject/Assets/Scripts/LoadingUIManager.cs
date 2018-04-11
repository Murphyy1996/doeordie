using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Author: James Murphy
//Purpose: A method to show the loading screen

public class LoadingUIManager : MonoBehaviour
{
    public static LoadingUIManager singleton;
    private Image thisLoadingScreen;

    private void Awake()
    {
        singleton = this;
        thisLoadingScreen = GetComponent<Image>();
    }

    public void ShowLoadingScreen(string sceneToLoad) //Enable the loading screen
    {
        thisLoadingScreen.enabled = true;
        StartCoroutine(LoadScene(sceneToLoad));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
