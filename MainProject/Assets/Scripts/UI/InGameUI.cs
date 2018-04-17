using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameUI : MonoBehaviour
{
    public static InGameUI inst;
    [SerializeField]
    private int secondsToWait;
    [SerializeField]
    private Sprite pistol, machGun, walk, run, crouch, slide, grapple, jump, wallclimb;
    [SerializeField]
    private Image state, weaponImage, beingHitMarker, gameOverScreen;
    [SerializeField]
    private Image Health, hitFront, hitBack, hitRight, hitLeft, blackBullet, redBullet;

    private float timer = 0, target = 1;
    private bool timerAllowed = false;
    private Vector3 defaultHealthScale;


  //  [SerializeField]
    public Text ammoText, pickedAmmo;
    GameObject player;
    Shooting shot;
    AmmoManager ammoScript;
    ReusableHealth hel;
    Crouch cro;
    CharacterControllerMovement move;
    Grapple grapes;
    WallClimbV2 climb;
    Teleporting tele;
    // Use this for initialization

    private void Awake()
    {
        inst = this;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ammoScript = player.GetComponent<AmmoManager>();
        shot = player.GetComponent<Shooting>();
        hel = player.GetComponent<ReusableHealth>();
        cro = player.GetComponent<Crouch>();
        move = player.GetComponent<CharacterControllerMovement>();
        grapes = player.GetComponent<Grapple>();
        climb = player.GetComponent<WallClimbV2>();
        tele = player.GetComponent<Teleporting>();
        hitFront.enabled = false;
        hitBack.enabled = false;
        hitRight.enabled = false;
        hitLeft.enabled = false;
        defaultHealthScale = Health.transform.localScale;

        //Control the default states of grapple UI
        if (UIElements.singleton != null)
        {
            if (grapes.IsGrappleAllowed() == false)
            {
                UIElements.singleton.cooldownGrapple.enabled = false;
            }
            if (tele.ReturnIfTeleportEnabled() == false)
            {
                UIElements.singleton.cooldownTele.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (timerAllowed == true)
        {
            timer = timer + Time.fixedDeltaTime;
        }
        if (timer >= target)
        {
            //method for making small
            makeBarSmall(1.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shot.currentWeaponScript == null)
        {
            weaponImage.enabled = false;
            ammoText.text = "Ammo: -";
        }
        // get shooting script - in start - get current weapon info script - compare types of ammo with an if??? -
        if (shot.currentWeaponScript != null)
        {
            WeaponInfo currentWeaponScript = shot.currentWeaponScript;
            weaponImage.enabled = true;
            if (currentWeaponScript.weaponModel == WeaponInfo.weapon.pistol)
            {
                weaponImage.sprite = pistol;
				ammoText.text = "Infinite";
             //   blackBullet.enabled = false;
              //  redBullet.enabled = false;

            }
            if (currentWeaponScript.weaponModel == WeaponInfo.weapon.machineGun)
            {
                weaponImage.sprite = machGun;
                ammoText.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
              //  blackBullet.enabled = true;
              //  redBullet.enabled = true;
            }
            if (currentWeaponScript.weaponModel == WeaponInfo.weapon.shotgun)
            {
                weaponImage.sprite = machGun;
                ammoText.text = "" + ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.shotgun);
                //  blackBullet.enabled = true;
                //  redBullet.enabled = true;
            }
        }
        float currentHealth = hel.healthValue;
        float maxHealth = hel.maxHealth;
        Health.fillAmount = currentHealth / maxHealth;
       // blackBullet.fillAmount = ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun) / 1f;
        // blackBullet.fillAmount = ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun) / ammoScript.ReturnMaxAmmoForMachineGun();
        //blackBullet.fillAmount = ammoScript.ReturnMaxAmmoForMachineGun() / ammoScript.ReturnAmountOfAmmoForWeapon(AmmoManager.ammoType.machineGun);
        ChangeImage();

    }

    private void ChangeImage()
    {
        if (grapes.IsCurrentlyGrappling() == true)
        {
            //Stop the current clip
            AudioManage.inst.player.Stop();
            //Load the sprite for the position indicator
            state.sprite = grapple;
        }
        else if (grapes.ReturnIfGrappleSliding() == true)
        {
            //Stop the current clip
            AudioManage.inst.player.Stop();
            //Load the sprite for the position indicator
            state.sprite = slide;
        }
        else if (climb != null && climb.ReturnIfWallClimbing() == true)
        {
            //Stop the current clip
            AudioManage.inst.player.Stop();
            //Load the sprite for the position indicator
            state.sprite = wallclimb;
        }
        else if (move.AccurateIsControllerGrounded() == false && cro.IsPlayerSliding() == false && grapes.isMomentumSliding() == false)
        {
            //Stop the current clip
            AudioManage.inst.player.Stop();
            //Load the sprite for the position indicator
            state.sprite = jump;
        }
        else if (cro.IsPlayerSliding() == true)
        {
            //Stop the current clip
            AudioManage.inst.player.Stop();
            //Load the sprite for the position indicator
            state.sprite = slide;
        }
        else if (cro.IsPlayerCrouching() == true && move.AccurateIsControllerGrounded() == true)
        {
            //Stop the current clip
            AudioManage.inst.player.Stop();
            //Load the sprite for the position indicator
            state.sprite = crouch;
        }
        else if (move.ReturnIfSprinting() == true)
        {
            //Check if the correct clip is loaded...
            if (AudioManage.inst.player.clip != AudioManage.inst.running)
            {
                //Stop the current clip
                AudioManage.inst.player.Stop();
                //Reload the audio clip
                AudioManage.inst.player.clip = AudioManage.inst.running;
            }
            //Check if the audio source is playing the audio clip and the player is moving
            if (AudioManage.inst.player.isPlaying == false && move.GetCurrentSpeedandDirection().magnitude > 0.1f)
            {
                //If not player the audio clip
                AudioManage.inst.player.Play();
            }
            else if (move.GetCurrentSpeedandDirection().magnitude < 0.1f) //Stop sound if not moving
            {
                //Stop the audio source from playing if not moving
                AudioManage.inst.player.Stop();
            }
            //Load the sprite for the position indicator
            state.sprite = run;
        }
        else
        {
            //Check if the correct clip is loaded...
            if (AudioManage.inst.player.clip != AudioManage.inst.walking)
            {
                //Stop the current clip
                AudioManage.inst.player.Stop();
                //Reload the audio clip
                AudioManage.inst.player.clip = AudioManage.inst.walking;
            }
            //Check if the audio source is playing the audio clip
            if (AudioManage.inst.player.isPlaying == false && move.GetCurrentSpeedandDirection().magnitude > 0.1f)
            {
                //If not player the audio clip
                AudioManage.inst.player.Play();
            }
            else if (move.GetCurrentSpeedandDirection().magnitude < 0.1f) //Stop sound if not moving
            {
                //Stop the audio source from playing if not moving
                AudioManage.inst.player.Stop();
            }
            //Load the sprite for the position indicator
            state.sprite = walk;
        }
    }

    public void moveHitmarker(float providedAngle)
    {
        if (providedAngle >= 330 && providedAngle <= 360 || providedAngle <= 30 && providedAngle >= 0)
        {
            providedAngle = 0;
        }
        if (providedAngle >= 220 && providedAngle <= 240 || providedAngle >= 140 && providedAngle <= 180)
        {
            providedAngle = 180;
        }
        hitFront.transform.eulerAngles = new Vector3(0, 0, providedAngle);
        if (hitFront.enabled == true)
        {
            hitFront.enabled = false;
        }
        StartCoroutine(DisplayHitMarker());
    }

    IEnumerator DisplayHitMarker()
    {
        hitFront.enabled = true;
        yield return new WaitForSeconds(secondsToWait);
        hitFront.enabled = false;
    }


    public void FadeInGameOver(float fadeTime)
    {
        //Fade in the game over screen
        StartCoroutine(FadeGameOverScreen(fadeTime, true));
    }

    public void FadeOutGameOver(float fadeTime)
    {
        //Pause the game
        QuestManager.inst.UnPauseGame();
        //Fade out the game over screen
        StartCoroutine(FadeGameOverScreen(fadeTime, false));
    }
    private IEnumerator FadeGameOverScreen(float fadeTimeValue, bool fadeIn) //This is where the fade code is done
    {
        //If the game over screen reference is null, cancel the coroutine
        if (gameOverScreen == null)
        {
            print("game over reference is null");
            yield return null;
        }
        //Get the colour values
        Color currentColour = gameOverScreen.color;
        Color endColour = gameOverScreen.color;
        //This will cause the screen to fade in
        if (fadeIn == true)
        {
            endColour.a = 1;
        }
        else //This will cause this coroutine to fade out
        {
            endColour.a = 0;
        }
        //Track the time elapsed
        float elapsedTime = 0;
        //Increase the alpha over the required time
        while (elapsedTime < fadeTimeValue)
        {
            //Lerp the colour over time
            currentColour.a = Mathf.Lerp(currentColour.a, endColour.a, (elapsedTime / fadeTimeValue));
            //Set the current colour
            gameOverScreen.color = currentColour;
            //Mark time as elapsed
            elapsedTime += Time.deltaTime;
            //Do this at the end of every frame
            yield return new WaitForEndOfFrame();
        }
        gameOverScreen.color = endColour;
    }

    public void makeBarBigger(float sizeToGo)
    {
        timer = 0;
        timerAllowed = true;
        //big boy
        if (Health.transform.localScale == defaultHealthScale)
        {
            Health.transform.localScale = Health.transform.localScale * sizeToGo;
        }
      //  else if (Health.transform.localScale != defaultHealthScale)
      //  {
            // makeBarSmall(2);
        //    return;
       // }
    }

    private void makeBarSmall(float sizeToGo)
    {
        timer = 0;
        timerAllowed = false;
        Health.transform.localScale = Health.transform.localScale = defaultHealthScale;

        //smallboy
    }

}


