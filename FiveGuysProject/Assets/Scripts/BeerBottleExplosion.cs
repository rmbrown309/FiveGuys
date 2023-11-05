using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeerBottleExplosion : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] ParticleSystem explosionEffect;

    void Start()
    {
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage canTakeDamage = other.GetComponent<IDamage>();
        canTakeDamage?.takeDamage(damage);
        IPhysics physicsable = other.GetComponent<IPhysics>();
        physicsable?.TakePhysics((other.transform.position - transform.position).normalized * (damage * 3));
        Destroy(gameObject);
    }
}