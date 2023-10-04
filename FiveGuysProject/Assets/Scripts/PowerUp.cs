using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    [SerializeField] float destroyTimer;
    [SerializeField] int typePower;
    [SerializeField] int newJumpMax;
    [SerializeField] int newSpeedMax;
    [SerializeField] int newDamage;
    [SerializeField] float newShootRate;
    [SerializeField] float newRegen;
    [SerializeField] bool randType;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTimer);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.transform != this.transform)
        {
            return;
        }
        IPower powerUp = other.GetComponent<IPower>();
        if (powerUp != null)
        {
            if(randType)
            {
                typePower = Random.Range(1, 6);
                Debug.Log(typePower);
            }
            switch (typePower)
            {
                case 1:
                    Debug.Log("jump");
                    powerUp.JumpPower(newJumpMax);
                    break;
                case 2:
                    Debug.Log("speed");

                    powerUp.SpeedBoost(newSpeedMax);
                    break;
                case 3:
                    Debug.Log("health");

                    powerUp.HealthRegen(newRegen);
                    break;
                case 4:
                    Debug.Log("shoot");

                    powerUp.ShootRate(newShootRate);
                    break;
                case 5:
                    Debug.Log("dmg");

                    powerUp.DamageUp(newDamage);
                    break;
                case 6:
                    Debug.Log("dmg");

                    powerUp.DamageUp(newDamage);
                    break;
            }
        }
        Destroy(gameObject);
    }
}
