using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeElevatorButton : MonoBehaviour {

    private ActivateAnimationOnButton AC;
  
    public Material buttonObj;
    private bool coRoRan = false;

    
	// Use this for initialization
	void Start ()
    {
        AC = this.gameObject.GetComponent<ActivateAnimationOnButton>();
        
       
     
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (AC.ReturnBool() == true)
        {
            if (coRoRan == false)
            {
                StopAllCoroutines();
                StartCoroutine(changingCols());
                coRoRan = true;
            }
          
        }
      
	}

    IEnumerator changingCols()
    {
        buttonObj.color = Color.green;
      
        yield return new WaitForSeconds(5f);
        buttonObj.color = Color.red;
    }

}
