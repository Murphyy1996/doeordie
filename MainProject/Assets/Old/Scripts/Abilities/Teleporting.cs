using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class Teleporting : MonoBehaviour
{

    //Author: James Murphy
    [SerializeField]
    private bool teleportEnabled = true;
    [SerializeField]
    private KeyCode teleportKeyCode = KeyCode.F;
    [SerializeField]
    private LayerMask castTeleportLayers, LosLayersWithPlayer;
    private CharacterControllerMovement movementScript;
    private CharacterController thisCC;
    private Transform cameraTransform;
    [SerializeField]
    [Range(0, 30)]
    private float teleportStrength = 2f, teleportRangeChangeSensetivity = 10f;
    [SerializeField]
    [Range(0, 5)]
    private float teleportStrengthClamp = 5;
    private GameObject teleportEmpty;
    private TeleportPoint teleportPointScript;
    private Grapple grappleScript;
    [SerializeField]
    private bool teleportButtonPressed = false, teleportButtonHeld = false, cooldown = false, isTeleporting = false, teleportExitPending = false;
    private GameObject detectorBox;
    [SerializeField]
    [Range(0, 20)]
    private float teleportCoolDown;
    private float tempTeleportStrength;
    private BlurOptimized blurEffect;
    private bool runOncePerTeleport = false, defaultGrappleValue = false;
    private float failSafeTimer = 0;
    private WallClimbV2 wallClimbScript;
    private Vector3 defaultTeleportIndicatorLocalPosition;
    private ReusableHealth playerHealth;
    private Image gameoverScreen;

    public enum teleportDirection
    {
        forwards,
        up,
        down
    }

    ;

    [HideInInspector]
    public teleportDirection currentTeleportDirection = teleportDirection.forwards, lastTeleportDirection;

    [SerializeField]
    private GameObject teleportIndicator;
    private Transform teleportIndicatorParent;

    private void Start() //Get required components
    {
        tempTeleportStrength = teleportStrength;
        movementScript = GetComponent<CharacterControllerMovement>();
        thisCC = GetComponent<CharacterController>();
        grappleScript = GetComponent<Grapple>();
        wallClimbScript = GetComponent<WallClimbV2>();
        teleportIndicatorParent = GameObject.Find("TeleportIndicatorParent").transform;
        playerHealth = GetComponent<ReusableHealth>();
        Invoke("GetCamera", 0.02f);
    }

    //Get the camera and spawn the detect box on the camera on the only collide with self layer and a rigidbody ooo and a detection script
    private void GetCamera()
    {
        cameraTransform = Camera.main.transform;
        blurEffect = cameraTransform.GetComponent<BlurOptimized>();
        //Make the indicator follow the y axis of the camera
        teleportIndicatorParent.transform.SetParent(cameraTransform);
        //Get the default teleport indicator local position
        defaultTeleportIndicatorLocalPosition = teleportIndicatorParent.transform.localPosition;
        //Spawn the teleport empty and name it
        if (teleportEmpty == null)
        {
            teleportEmpty = new GameObject();
            teleportPointScript = teleportEmpty.AddComponent<TeleportPoint>();
            teleportPointScript.playerCC = thisCC;
            //Position the teleport empty
            teleportEmpty.transform.rotation = teleportIndicatorParent.rotation;
            teleportEmpty.transform.position = teleportIndicatorParent.position;
            //Set as a child of the teleport indicator empty
            teleportEmpty.transform.SetParent(teleportIndicatorParent);
            //Turn on the empty
            teleportEmpty.SetActive(false);
        }
        try
        {
            gameoverScreen = GameObject.Find("DeathScreen").GetComponent<Image>();
        }
        catch
        {
            print("couldn't get game over screen");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Do not recognise key inputs if the teleport is cooling down
        if (cooldown == true)
        {
            teleportButtonHeld = false;
            teleportButtonPressed = false;
        }
        //Detect key inputs
        if (Input.GetKeyUp(teleportKeyCode) && cooldown == false && teleportEnabled == true)
        {
            teleportButtonHeld = true;
        }
        if (teleportEnabled == true && teleportButtonHeld == true && cooldown == false)
        {
            ShowPotentialTeleportPosition();
        }
    }

    private void FixedUpdate()
    {
        //Handles the teleport exit code
        if (teleportExitPending == true) //need a peice of code for exit teleport pending
        {
            TeleportFailsafe();
            teleportExitPending = false;
            teleportButtonHeld = false;
            teleportButtonPressed = false;
        }
        //This handles the teleport buttons
        if (teleportButtonPressed == true && teleportEnabled == true && cooldown == false)
        {
            teleportButtonHeld = false;
            teleportButtonPressed = false;
        }
        //Handles the code for teleporting
        if (isTeleporting == true)
        {
            //Reset the momentum as you leave teleport
            movementScript.GravityVelocityReset();
            grappleScript.ExitGrapple();
            teleportEmpty.transform.SetParent(null);
            //Increase the failsafe timer
            failSafeTimer = failSafeTimer + Time.fixedDeltaTime;
            //Check if the conditions to teleport have been satisfied
            if (teleportEmpty != null && teleportEmpty.transform.position != transform.position && Vector3.Distance(transform.position, teleportEmpty.transform.position) >= 1f)
            {
                if (failSafeTimer >= 0.2f)
                {
                    teleportExitPending = true;
                }
                else
                {
                    if (runOncePerTeleport == false)
                    {
                        defaultGrappleValue = false;
                        defaultGrappleValue = grappleScript.IsGrappleAllowed();
                        blurEffect.enabled = true;
                        AudioManage.inst.teleport.Play();
                        runOncePerTeleport = true;
                        //Controls the particle effect
                        if (ParticleEffectController.inst != null)
                        {
                            ParticleEffectController.inst.TeleParticle();
                        }
                        //Turn the movement speed to nothing
                        movementScript.ChangeMovementSpeed(0);
                    }
                    grappleScript.SetGrappleAllowedValue(false);
                    cooldown = true;
                    //Lerp the player
                    float speed = Time.fixedDeltaTime * 15;
                    transform.position = Vector3.Lerp(transform.position, teleportEmpty.transform.position, speed);
                }
            }
            else
            {
                teleportExitPending = true;
            }
        }
    }

    public void SetDefaultGrappleValue(bool value)
    {
        defaultGrappleValue = value;
    }

    private void TeleportFailsafe() //Force the player to the teleport point
    {
        if (gameoverScreen.color.a == 0 && CanActuallyTeleport() == true)
        {
            if (teleportPointScript != null)
            {
                if (teleportPointScript.AmInObject() == false)
                {
                    transform.position = teleportEmpty.transform.position;
                    cooldown = true;
                }
            }
        }
        isTeleporting = false;
        failSafeTimer = 0;
        if (defaultGrappleValue == true)
        {
            grappleScript.SetGrappleAllowedValue(true);
        }
        blurEffect.enabled = false;
        runOncePerTeleport = false;
        teleportEmpty.transform.SetParent(transform);
        StopAllCoroutines();
        StartCoroutine(Cooldown());
        //Turn off the teleport point
        teleportEmpty.SetActive(false);
        //Reset the movement speed
        movementScript.ResetMovementSpeed();
    }

    private void ShowPotentialTeleportPosition() //This will show the potential position of where the player can go
    {
        //Show the teleport empty
        if (teleportButtonHeld == true)
        {
            //Spawn the teleport empty and name it
            if (teleportEmpty == null)
            {
                teleportEmpty = new GameObject();
                teleportPointScript = teleportEmpty.AddComponent<TeleportPoint>();
                teleportPointScript.playerCC = thisCC;
                //Turn on the empty
                teleportEmpty.SetActive(false);
            }

            //Allow the player to reposition the indicator with the middle mouse button
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                //Get the unclamped zoom position
                float unclampedZPosition = teleportIndicatorParent.localPosition.z + (Input.GetAxis("Mouse ScrollWheel") * teleportRangeChangeSensetivity);
                //Clamp the zoom between the desired values
                float clampedZPosition = Mathf.Clamp(unclampedZPosition, defaultTeleportIndicatorLocalPosition.z - teleportStrengthClamp, defaultTeleportIndicatorLocalPosition.z + teleportStrengthClamp);
                //Apply the clamped zoom to the position
                teleportIndicatorParent.localPosition = new Vector3(teleportIndicatorParent.localPosition.x, teleportIndicatorParent.localPosition.y, clampedZPosition);
            }

            //Reposition and create the indicator
            if (teleportEmpty.activeSelf == false)
            {
                //Reset the local position of the teleport indicator parent back to default
                teleportIndicatorParent.transform.localPosition = defaultTeleportIndicatorLocalPosition;
                //Position the teleport empty
                teleportEmpty.transform.rotation = teleportIndicatorParent.rotation;
                teleportEmpty.transform.position = teleportIndicatorParent.position;
                //Turn on the empty
                teleportEmpty.SetActive(true);
            }

            //If needed set this object as parent
            if (teleportEmpty.transform.parent != teleportIndicatorParent)
            {
                teleportEmpty.transform.SetParent(teleportIndicatorParent);
            }
            if (playerHealth.playerIsDead == false)
            {
                //Will control whether you exit the teleport or enter it
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    teleportButtonHeld = false;
                    teleportButtonPressed = false;
                    teleportEmpty.SetActive(false);
                }
                else if (Input.GetKeyDown(teleportKeyCode) && cooldown == false)
                {
                    tempTeleportStrength = teleportStrength;
                    teleportButtonPressed = true;
                    StandardTeleportCode(false);
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0) && cooldown == false)
                {
                    tempTeleportStrength = teleportStrength;
                    teleportButtonPressed = true;
                    teleportButtonHeld = true;
                    StandardTeleportCode(true);
                }
            }
            else
            {
                teleportButtonHeld = false;
                teleportButtonPressed = false;
                teleportEmpty.SetActive(false);
            }
        }
        else
        {
            if (teleportEmpty != null)
            {
                teleportEmpty.SetActive(false);
            }
        }
    }

    public bool CanActuallyTeleport() //Return whether the player can teleport
    {
        bool teleportableSurface = true;
        //Check if the surface is teleportable
        RaycastHit rayOut;
        Vector3 direction = cameraTransform.forward;
        //If possible look at the teleport point
        if (teleportEmpty != null && teleportEmpty.activeSelf == true)
        {
            direction = teleportEmpty.transform.position - cameraTransform.position;
        }
        if (Physics.Raycast(cameraTransform.position, direction, out rayOut, teleportStrength, castTeleportLayers))
        {
            //If you are teleporting onto a non teleportable surface
            if (rayOut.collider.gameObject.layer == 15)
            {
                teleportableSurface = false;
            }
        }
        return teleportableSurface;
    }

    private void StandardTeleportCode(bool usingClick) //Teleport code is here
    {
        try
        {
            //Disable player input
            movementScript.IsPlayerInputEnabled(false);

            //This variable will change depending on if the surface is teleportable or not
            bool teleportableSurface = true;
            float tempTeleportStrength = teleportStrength;

            //Raycast depending on what way the teleport is
            Vector3 rayDirection = teleportEmpty.transform.position - cameraTransform.position; ;
            //Check if the surface is teleportable
            RaycastHit rayOut;
            if (Physics.Raycast(cameraTransform.position, rayDirection, out rayOut, teleportStrength, castTeleportLayers))
            {
                //If you are teleporting onto a non teleportable surface
                if (rayOut.collider.gameObject.layer == 15)
                {
                    teleportableSurface = false;
                }
            }

            //If the player is allowed to teleport through the surface prepare for it here
            if (teleportableSurface == true && teleportButtonHeld == true && teleportButtonPressed == true)
            {
                //Enable the cooldown
                cooldown = true;
                //Mark buttons as not pressed
                teleportButtonHeld = false;
                teleportButtonPressed = false;
                //Delay the teleport to allow physics to do its thing first
                Invoke("TeleportDelay", 0.01f);
            }
        }
        catch
        {
            print("Error teleporting");
            //Cancel potential teleport
            CancelInvoke();
            cooldown = false;
            isTeleporting = false;
        }

        //If using click is true and the user didn't teleport
        if (cooldown == false)
        {
            if (teleportEmpty != null)
            {
                teleportEmpty.SetActive(false);
            }
            teleportEnabled = false;
            teleportButtonHeld = false;
            teleportEnabled = true;
            teleportButtonHeld = true;
            teleportButtonPressed = false;
            cooldown = false;
        }

        //Enable player input again
        movementScript.IsPlayerInputEnabled(true);
    }

    private void TeleportDelay()
    {
        //This check will see if the player can leave the teleport safely
        if (teleportPointScript.AmInObject() == false && Vector3.Distance(transform.position, teleportEmpty.transform.position) > 3 && teleportButtonPressed == false && teleportButtonHeld == false)
        {
            //Mark the player as teleporting
            isTeleporting = true;
            //Enable the teleport blur
            blurEffect.enabled = true;
        }
        else
        {
            CancelTeleport();
            cooldown = false;
        }
    }

    public void SetTeleportEnabledValue(bool value)
    {
        teleportEnabled = value;
    }

    public float ReturnStregnthValue()
    {
        return teleportStrength;
    }

    public void SetNewStrength(float stregnth)
    {
        teleportStrength = stregnth;
    }

    private IEnumerator Cooldown() //Cooldown for the teleport
    {
        StopCoroutine(FillUIElement());
        StartCoroutine(FillUIElement());
        yield return new WaitForSeconds(teleportCoolDown);
        cooldown = false;
    }

    private IEnumerator FillUIElement() //Fill the teleport UI element
    {
        if (UIElements.singleton.gameObject != null && UIElements.singleton.cooldownGrapple != null)
        {
            float time = teleportCoolDown;
            //While there is still a cool down
            while (time > 0)
            {
                //Reduce the time so that the while loop will not crash the game
                time -= Time.deltaTime;
                //Fill the image
                UIElements.singleton.cooldownTele.fillAmount = (teleportCoolDown - time) / teleportCoolDown;
                //  Debug.Log("RUN RUN RUN ");
                yield return null;
            }
        }
    }

    public GameObject ReturnTeleportIndicator()
    {
        return teleportIndicator;
    }

    public GameObject ReturnTeleportEmpty()
    {
        return teleportEmpty;
    }

    public bool ReturnIfTeleportEnabled()
    {
        return teleportEnabled;
    }

    public LayerMask ReturnTeleportPointLayerMask()
    {
        return LosLayersWithPlayer;
    }

    public bool ReturnIfTeleporting() //Return if the player is teleporting
    {
        return isTeleporting;
    }

    public bool ReturnIfTeleportButtonHeld()
    {
        return teleportButtonHeld;
    }

    public void CancelTeleport() //This will be called on death
    {
        CancelInvoke();
        teleportExitPending = true;
        teleportButtonHeld = false;
        if (teleportEmpty != null)
        {
            teleportEmpty.SetActive(false);
        }
    }
}
