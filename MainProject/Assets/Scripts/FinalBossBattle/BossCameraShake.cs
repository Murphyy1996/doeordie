using UnityEngine;
using System.Collections;

public class BossCameraShake : MonoBehaviour
{
    private Transform camTransform;
    public float shakeDuration = 0.5f, shakeAmount = 0.7f, decreaseFactor = 1.0f;
    private FirstPersonCamera fpsCam;

    private void OnEnable() //Get the required defaults and components
    {
        camTransform = Camera.main.transform;
        fpsCam = camTransform.GetComponent<FirstPersonCamera>();
    }

    private void Update()
    {
        if (camTransform != null && Time.deltaTime != 0 && fpsCam != null)
        {
            if (shakeDuration > 0)
            {
                camTransform.localPosition = fpsCam.defaultLocalPos + Random.insideUnitSphere * shakeAmount;
                fpsCam.isShaking = true;
                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                camTransform.localPosition = fpsCam.defaultLocalPos;
                fpsCam.isShaking = false;
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
