using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickups : MonoBehaviour
{
    [SerializeField] gunStats gun;

    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audCash;
    [Range(0, 1)][SerializeField] float audCashVol;

    bool playerInTrigger;
    bool pickedUp;

    // Start is called before the first frame update
    void Start()
    {
        gun.ammoCur = gun.ammoMax;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && playerInTrigger && !pickedUp && GameManager.instance.score >= gun.cost)
        {
            if (GameManager.instance.pickupLabel != null)
                GameManager.instance.pickupLabel.SetActive(false);
            //pickedUp = true;
            GameManager.instance.IncreasePlayerScore(-gun.cost);
            GameManager.instance.playerScript.setGunStats(gun);
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
}
