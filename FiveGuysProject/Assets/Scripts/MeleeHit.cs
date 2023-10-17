using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHit : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        //Checks to see if it collided with a player
        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null )
        {
            damageable.takeDamage(damage);
        }
    }
}
