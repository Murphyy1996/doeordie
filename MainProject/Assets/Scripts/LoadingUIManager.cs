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
    [SerializeField]
    private Sprite[] artToUse;

    private void Awake()
    {
        singleton = this;
        thisLoadingScreen = GetComponent<Image>();
        if (artToUse.Length > 0)
        {
            thisLoadingScreen.sprite = artToUse[Random.Range(0, artToUse.Length)];
        }
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
