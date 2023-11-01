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
                GameManager.instance.pickupLabel.SetActive(false);
            //pickedUp = true;
            WeaponOffAnimation(GameManager.instance.playerScript.GetGunID());
            Debug.Log(GameManager.instance.playerScript.GetGunID());
            GameManager.instance.IncreasePlayerScore(-gun.cost);
            GameManager.instance.playerScript.setGunStats(gun);
            WeaponOnAnimation(GameManager.instance.playerScript.GetGunID());
            Debug.Log(GameManager.instance.playerScript.GetGunID());
            aud.PlayOneShot(audCash, audCashVol);
            //Destroy(gameObject);
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
                GameManager.instance.pickupLabel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (GameManager.instance.pickupLabel != null)
                GameManager.instance.pickupLabel.SetActive(false);
        }
    }
    IEnumerator Wait()
    {

        yield return new WaitForSeconds(3);
    }
    private void WeaponOffAnimation(int weaponID)
    {
        HudAnimate animation = weaponAnimation.GetComponent<HudAnimate>();
        if(animation != null)
        {
            switch (weaponID)
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
            }
        }
    }
    private void WeaponOnAnimation(int weaponID)
    {
        //GameObject animate = weaponAnimation.transform.GetChild(2).gameObject;
        HudAnimate animation = weaponAnimation.GetComponent<HudAnimate>();
        if(animation != null)
        {
            switch (weaponID)
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
            }
        }
    }
}
