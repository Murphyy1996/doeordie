using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideIfParentImageOff : MonoBehaviour
{
    [SerializeField]
    private Image thisImage, parentImage; 

    private void Start()
    {
        thisImage = GetComponent<Image>();
        parentImage = transform.parent.GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        if (parentImage != null)
        {
            if (parentImage.enabled == true)
            {
                thisImage.enabled = true;
            }
            else
            {
                thisImage.enabled = false;
            }
        }
    }
}
