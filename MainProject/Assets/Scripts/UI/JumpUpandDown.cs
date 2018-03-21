using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class JumpUpandDown : MonoBehaviour
{


    private GameObject player;
    private Transform gunZone;
    [SerializeField]
    [Header("Obj References")]
    private RectTransform uiToMove;
    [Header("General Options")]
    [SerializeField]
    private float lerpSpeed = 15f;
    [SerializeField]
    private float landingBounceDuration = 0.08f;
    [SerializeField]
    private float jumpRise = 20f, landShrink = 15f;
    [SerializeField]
    [Header("Gun Specific Options")]
    private float divisionModifierForGunRise = 120;
    [SerializeField]
    private float divisionModifierForGunSink = 30;
    private Vector3 defaultUiPosition, raisedUiPosition, uiLerpTarget, lowerUIPos, gunLerpTarget, defaultGunPosition;
    CharacterControllerMovement cc;
    private bool runOnceLanding = false, canResetToDefault = false, allowedToLerp = false;
    private CharacterController thisCC;
    private Transform gunRaiseEmpty, gunLowerEmpty;

    // Use this for initialization
    private void Start()
    {
        Invoke("DelayedStart", 0.3f);
    }

    private void DelayedStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        thisCC = player.GetComponent<CharacterController>();
        gunZone = GameObject.Find("GunZone").transform;
        cc = player.GetComponent<CharacterControllerMovement>();
        defaultUiPosition = uiToMove.position;
        raisedUiPosition = new Vector3(defaultUiPosition.x, defaultUiPosition.y + jumpRise, defaultUiPosition.z);
        lowerUIPos = new Vector3(defaultUiPosition.x, defaultUiPosition.y - landShrink, defaultUiPosition.z);
        uiLerpTarget = defaultUiPosition;
        defaultGunPosition = gunZone.localPosition;
        //Raised gun tsrget
        GameObject gunRaise = new GameObject();
        gunRaise.name = "Gun Raised Position";
        gunRaise.transform.SetParent(Camera.main.transform);
        gunRaise.transform.localPosition = new Vector3(gunZone.localPosition.x, gunZone.localPosition.y + jumpRise / divisionModifierForGunRise, gunZone.localPosition.z);
        gunRaiseEmpty = gunRaise.transform;
        //Lower gun target
        GameObject gunLower = new GameObject();
        gunLower.name = "Gun Lowered Position";
        gunLower.transform.SetParent(Camera.main.transform);
        gunLower.transform.localPosition = new Vector3(gunZone.localPosition.x, gunZone.localPosition.y - jumpRise / 30, gunZone.localPosition.z);
        gunLowerEmpty = gunLower.transform;
        //Set the default gun target
        gunLerpTarget = defaultGunPosition;
        allowedToLerp = true;
    }



    private void FixedUpdate()
    {
        if (allowedToLerp == true)
        {
            if (cc.ReturnObjectStandingOnJump() == null)
            {
                if (thisCC.velocity.y >= 8f || thisCC.velocity.y <= -8f)
                {
                    RaiseUI();
                }
            }
            else
            {
                if (runOnceLanding == false)
                {
                    runOnceLanding = true;
                    LowerUI();
                }
                else
                {
                    ReturnToDefault();
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (player != null && player.activeSelf == true && Time.timeScale == 1 && allowedToLerp == true)
        {
            uiToMove.position = Vector3.Lerp(uiToMove.position, uiLerpTarget, lerpSpeed * Time.deltaTime);
            gunZone.localPosition = Vector3.Lerp(gunZone.localPosition, gunLerpTarget, lerpSpeed * Time.deltaTime);
        }

    }

    private void RaiseUI()
    {
        StopAllCoroutines();
        runOnceLanding = false;
        canResetToDefault = false;
        uiLerpTarget = raisedUiPosition;
        gunLerpTarget = gunRaiseEmpty.localPosition;

    }

    private void LowerUI()
    {
        uiLerpTarget = lowerUIPos;
        gunLerpTarget = gunLowerEmpty.localPosition;
        StartCoroutine(StayLowered());
    }

    private IEnumerator StayLowered()
    {
        yield return new WaitForSeconds(landingBounceDuration);
        canResetToDefault = true;
    }

    private void ReturnToDefault()
    {
        if (canResetToDefault == true)
        {
            uiLerpTarget = defaultUiPosition;
            gunLerpTarget = defaultGunPosition;
        }
    }
}
