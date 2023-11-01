using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSmellyExplosion : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] ParticleSystem noxiousGas;

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamage canTakeDamage = other.GetComponent<IDamage>();
            canTakeDamage?.takeDamage(damage);
            IPhysics physicsable = other.GetComponent<IPhysics>();
            physicsable?.TakePhysics((other.transform.position - transform.position).normalized * (damage * 3));
            Destroy(gameObject);
        }
    }
}
