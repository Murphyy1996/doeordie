using UnityEngine;
using System.Collections;

public class BossCameraShake : MonoBehaviour
{
    private Transform camTransform;
    public float shakeDuration = 0.5f, shakeAmount = 0.7f, decreaseFactor = 1.0f;
    Vector3 originalLocalPos;

    private void OnEnable() //Get the required defaults and components
    {
        camTransform = Camera.main.transform;
        originalLocalPos = camTransform.localPosition;
    }

    private void Update()
    {
        if (camTransform != null && Time.deltaTime != 0)
        {
            if (shakeDuration > 0)
            {
                camTransform.localPosition = originalLocalPos + Random.insideUnitSphere * shakeAmount;

                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                camTransform.localPosition = originalLocalPos;
                //Once returned to position, turn this script off
                Destroy(this);
            }
        }
    }

    public void ShakeitShakeit(float shaketime, float shakeAmountToDo)
    {
        shakeDuration = shaketime;
        shakeAmount = shakeAmountToDo;
    }
}
