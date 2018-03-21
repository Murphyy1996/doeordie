using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCamera : MonoBehaviour
{
    private Camera thisCamera;
    [SerializeField]
    private RenderTexture textureToRenderTo;

    private void Start()
    {
        if (GetComponent<Camera>() != null)
        {
            thisCamera = GetComponent<Camera>();
        }
        if (textureToRenderTo != null)
        {
            thisCamera.targetTexture = textureToRenderTo;
        }
    }
}
