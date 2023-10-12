using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    enum PickupType
    {
        HealthUp, SpeedUp, QuickRegen, DamageUp, AmmoRefill, SprayWeapon
    }

    [SerializeField] PickupType pickupType;
    [SerializeField] int pickupCost;

    bool playerInTrigger;
    string labelText;
    bool pickedUp;

    void Start()
    {
        
    }
    
    void Update()
    {
        if(Input.GetButtonDown("Interact") && playerInTrigger && !pickedUp && GameManager.instance.score >= pickupCost)
        {
            // activate pickup effect
            switch (pickupType)
            {
                case PickupType.HealthUp:
                    GameManager.instance.playerScript.IncreasePlayerMaxHealth(6);

                    // remove label to show that pickup was bought
                    if (GameManager.instance.pickupLabel != null)
                        GameManager.instance.pickupLabel.SetActive(false);
                    pickedUp = true;

                    break;

                case PickupType.SpeedUp:
                    GameManager.instance.playerScript.IncreasePlayerMaxSpeed(1.5f);

                    // remove label to show that pickup was bought
                    if (GameManager.instance.pickupLabel != null)
                        GameManager.instance.pickupLabel.SetActive(false);
                    pickedUp = true;

                    break;

                case PickupType.QuickRegen:
                    GameManager.instance.playerScript.IncreasePlayerRegenSpeed(2);

                    // remove label to show that pickup was bought
                    if (GameManager.instance.pickupLabel != null)
                        GameManager.instance.pickupLabel.SetActive(false);
                    pickedUp = true;

                    break;

                case PickupType.DamageUp:
                    GameManager.instance.playerScript.IncreasePlayerDamage(1);

                    break;

                case PickupType.AmmoRefill:
                    // Not yet implemented
                    break;
                case PickupType.SprayWeapon:
                    GameManager.instance.playerScript.GetRatKiller();

                    // remove label to show that pickup was bought
                    if (GameManager.instance.pickupLabel != null)
                        GameManager.instance.pickupLabel.SetActive(false);
                    pickedUp = true;

                    break;
            }

            GameManager.instance.IncreasePlayerScore(-pickupCost);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (pickupType)
        {
            case PickupType.HealthUp:
                GameManager.instance.pickupText.text = "[E] Health Up: " + pickupCost + " Points";
                break;
            case PickupType.SpeedUp:
                GameManager.instance.pickupText.text = "[E] Speed Up: " + pickupCost + " Points";
                break;
            case PickupType.QuickRegen:
                GameManager.instance.pickupText.text = "[E] Quick Regen: " + pickupCost + " Points";
                break;
            case PickupType.DamageUp:
                GameManager.instance.pickupText.text = "[E] Damage Up: " + pickupCost + " Points";
                break;
            case PickupType.AmmoRefill:
                GameManager.instance.pickupText.text = "[E] Refill Ammo: " + pickupCost + " Points";
                break;
            case PickupType.SprayWeapon:
                GameManager.instance.pickupText.text = "[E] Acquire Rat Killer: " + pickupCost + " Points";
                break;
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
            playerInTrigger = true;
            if (GameManager.instance.pickupLabel != null)
                GameManager.instance.pickupLabel.SetActive(false);
        }
    }

}
