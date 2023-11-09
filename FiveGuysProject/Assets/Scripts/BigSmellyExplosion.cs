using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSmellyExplosion : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] ParticleSystem noxiousGas;
    public float damageCooldown;
    public float _lastTick;

    void Start()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (Time.time - _lastTick < damageCooldown) 
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            //Currently set to be removed after greatly damaging the player once.
            IDamage canTakeDamage = other.GetComponent<IDamage>();
            canTakeDamage?.takeDamage(damage);
            _lastTick = Time.time;
        }
    }
    IEnumerator DamageOverTime()
    {
        yield return new WaitForSeconds(1f);
    }
}