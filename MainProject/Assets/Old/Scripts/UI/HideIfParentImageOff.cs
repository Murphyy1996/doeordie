using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideIfParentImageOff : MonoBehaviour
{
    [SerializeField]
    private Image thisImage, parentImage;
    [SerializeField]
    private bool automaticallyGetParent = true;

    private void Start()
    {
        thisImage = GetComponent<Image>();
        if (automaticallyGetParent == true)
        {
            parentImage = transform.parent.GetComponent<Image>();
        }
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
