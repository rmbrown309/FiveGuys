using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickups : MonoBehaviour
{
    [SerializeField] gunStats gun;
    [SerializeField] GameObject weaponAnimation;
    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audCash;
    [Range(0, 1)][SerializeField] float audCashVol;
    GameObject animate;
    bool playerInTrigger;
    bool pickedUp;
    bool isPowerWeapon;
    int lastWeaponID;
    // Start is called before the first frame update
    void Start()
    {
        weaponAnimation = GameObject.FindWithTag("HUD");
      //  animate = weaponAnimation.transform.GetChild(2).gameObject;
        gun.ammoCur = gun.ammoMax;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && playerInTrigger && !pickedUp && GameManager.instance.score >= gun.cost)
        {
            if (GameManager.instance.pickupLabel != null)
            {
                GameManager.instance.pickupLabelContainer.SetActive(false);
                GameManager.instance.pickupLabel.SetActive(false);
            }
            //pickedUp = true;
            WeaponOffAnimation(GameManager.instance.playerScript.ammoID);
            
            //Debug.Log(GameManager.instance.playerScript.GetGunID());
            GameManager.instance.IncreasePlayerScore(-gun.cost);
            GameManager.instance.playerScript.setGunStats(gun);
            lastWeaponID = GameManager.instance.playerScript.ammoID;
            WeaponOnAnimation(GameManager.instance.playerScript.ammoID);
            //Debug.Log(GameManager.instance.playerScript.GetGunID());
            aud.PlayOneShot(audCash, audCashVol);
            //Destroy(gameObject);
        }
        if(GameManager.instance.playerScript.ammoID == 4)
        {
            //Debug.Log("isPowerWeapon");
            WeaponOffAnimation(lastWeaponID);
            WeaponOnAnimation(GameManager.instance.playerScript.ammoID);
            isPowerWeapon = true;
        }else if(GameManager.instance.playerScript.ammoID < 4 && isPowerWeapon == true)
        {
            WeaponOffAnimation(4);
            WeaponOnAnimation(GameManager.instance.playerScript.ammoID);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            GameManager.instance.pickupText.text = "[E] Buy "+ gun.gunName + ": " + gun.cost + " Points";
        }
        if (other.CompareTag("Player") && !pickedUp)
        {
            playerInTrigger = true;
            if (GameManager.instance.pickupLabel != null)
            {
                GameManager.instance.pickupLabelContainer.SetActive(true);
                GameManager.instance.pickupLabel.SetActive(true);
            }

                
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (GameManager.instance.pickupLabel != null)
            {
                GameManager.instance.pickupLabelContainer.SetActive(false);
                GameManager.instance.pickupLabel.SetActive(false);
            }

        }
    }
    IEnumerator Wait()
    {

        yield return new WaitForSeconds(3);
    }
    private void WeaponOffAnimation(int ammoID)
    {
        HudAnimate animation = weaponAnimation.GetComponent<HudAnimate>();
        if(animation != null)
        {
            switch (ammoID)
            {
                case 0:
                    animation.TurnOffMainWeapon();
                    break;
                case 1:
                    animation.TurnOffSubWeapon();
                    break;
                case 2:
                    animation.TurnOffSnipeWeapon();
                    break;
                case 3:
                    animation.TurnOffShotGunWeapon();
                    break;
                case 4:
                    animation.TurnOffPowerWeapons();
                    break;
            }
        }
    }
    private void WeaponOnAnimation(int ammoID)
    {
        //GameObject animate = weaponAnimation.transform.GetChild(2).gameObject;
        HudAnimate animation = weaponAnimation.GetComponent<HudAnimate>();
        if(animation != null)
        {
            switch (ammoID)
            {
                case 0:
                    animation.TurnOnMainWeapon();
                    break;
                case 1:
                    animation.TurnOnSubWeapon();
                    break;
                case 2:
                    animation.TurnOnSnipeWeapon();
                    break;
                case 3:
                    animation.TurnOnShotGunWeapon();
                    break;
                case 4:
                    animation.TurnOnPowerWeapons();
                    break;
            }
        }
    }
    public void SetAudio(float newVolume)
    {
        audCashVol = newVolume;
        gun.audShotVol = newVolume;
    }

}
