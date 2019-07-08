using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeOpacityOnTrigger : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer renderer;
    private Material glassCol;
    [SerializeField]
    [Range(0, 1)]
    private float defaultOpacityAmount = 1f, targetOpacityAmount = 0.3f;

    // Use this for initialization
    void Start ()
    {

        glassCol = renderer.material;
        Color targetColour = glassCol.color;
        targetColour.a = defaultOpacityAmount;
        glassCol.color = targetColour;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Color targetColour = glassCol.color;
            targetColour.a = targetOpacityAmount;
            glassCol.color = targetColour;
            Destroy(this.gameObject);
        }
    }
}
