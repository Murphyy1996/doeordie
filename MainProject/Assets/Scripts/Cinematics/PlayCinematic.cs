using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayCinematic : MonoBehaviour
{
    [SerializeField]
    private MovieTexture cutsceneToPlay;
    [SerializeField]
    private float cutsceneLength = 0;
    [SerializeField]
    private string sceneNameToLoadAfterCutscene;
    private float cutsceneCounter = 0;

    // Use this for initialization
    private void Start()
    {
        if (cutsceneToPlay != null)
        {
            cutsceneToPlay.Play();
        }
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        cutsceneCounter = cutsceneCounter + Time.fixedDeltaTime;

        //Run the code to make the cutscene transition to the selected scene
        if (cutsceneCounter >= cutsceneLength)
        {
            GetComponentInChildren<AudioSource>().Stop();
            LoadingUIManager.singleton.ShowLoadingScreen(sceneNameToLoadAfterCutscene);
        }

        //Check for button input to skip the cutscene
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Space))
        {
            cutsceneCounter = 9999;
        }
    }
}
