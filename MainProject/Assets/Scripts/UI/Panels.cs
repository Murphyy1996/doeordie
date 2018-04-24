using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Panels : MonoBehaviour
{

    [SerializeField]
    private GameObject visual, controls, misc;
    [SerializeField]
    private Button vis, con, mi;


    // Use this for initialization
    void Start()
    {
        Invoke("LoadComponents", 1f);
        // visual.enabled = false;
        //  controls.enabled = false;
        // misc.enabled = false;
        StopAllCoroutines();
        StartCoroutine(GetRefs()); //not getting the refs in game options menu

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadComponents()
    {
        //GameObject.Find("DisplayOptionsVis").GetComponent<Image>();
        if (visual == null) //loading the panel refs
        {

            GameObject.Find("DisplayOptionsVis").GetComponent<Image>();
            print("AMI  RUNNING1");
        }
        if (controls == null)
        {

            GameObject.Find("DisplayControls").GetComponent<Image>();
            print("AMI  RUNNING2");
        }
        if (misc == null)
        {

            GameObject.Find("DisplayMisc").GetComponent<Image>();
            print("AMI  RUNNING3");
        }
        if (vis == null) //loading the buttons refs
        {

            GameObject.Find("Tab1Vis").GetComponent<Button>();
            print("AMI  RUNNING4");
        }
        if (con == null)
        {

            GameObject.Find("Tab2Controls").GetComponent<Button>();
            print("AMI  RUNNING5");
        }
        if (mi == null)
        {

            GameObject.Find("Tab3Misc").GetComponent<Button>();
            print("AMI  RUNNING6");
        }
    }


    public void LoadVisualOptions()
    {
        visual.SetActive(true);

        if (controls.activeInHierarchy == true && controls != null)
        {
            controls.SetActive(false);
        }
        if (misc.activeInHierarchy == true && misc != null)
        {
            misc.SetActive(false); ;
        }
    }
    public void LoadControlOptions()
    {
        controls.SetActive(true);
        if (visual.activeInHierarchy == true && visual != null)
        {
            visual.SetActive(false);
        }

        if (misc.activeInHierarchy == true && misc != null)
        {
            misc.SetActive(false);
        }
    }
    public void LoadMiscOptions()
    {
        misc.SetActive(true);
        if (visual.activeInHierarchy == true && visual != null)
        {
            visual.SetActive(false);
        }
        if (controls.activeInHierarchy == true && controls != null)
        {
            controls.SetActive(false);
        }
        Debug.Log("loading misc");

    }
    public void close()
    {
        if (visual.activeInHierarchy == true && visual != null)
        {
            visual.SetActive(false);
        }
        if (controls.activeInHierarchy == true && controls != null)
        {
            controls.SetActive(false);
        }
        if (misc.activeInHierarchy == true && misc != null)
        {
            misc.SetActive(false);
        }
    }

    private IEnumerator GetRefs()
    {
        visual.SetActive(true);
        controls.SetActive(true);
        misc.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        visual.SetActive(false);
        controls.SetActive(false);
        misc.SetActive(false);
    }

}
