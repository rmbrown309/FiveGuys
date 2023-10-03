using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] float destroyTimer;
    [SerializeField] int typePower;
    [SerializeField] int newJumpMax;
    [SerializeField] int newSpeedMax;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTimer);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IPower powerUp = other.GetComponent<IPower>();
        if (powerUp != null)
        {
            switch (typePower)
            {
                case 1:
                    powerUp.JumpPower(newJumpMax);
                    break;
                case 2:
                    powerUp.SpeedBoost(newSpeedMax);
                    break;
            }
        }
        Destroy(gameObject);
    }
}
