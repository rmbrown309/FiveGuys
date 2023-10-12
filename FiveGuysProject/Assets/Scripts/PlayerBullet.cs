using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet Stats -----")]
    public int damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] ParticleSystem hitEffect;


    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    //When a bullet collides with something
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.takeDamage(damage);

        }
        if(hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void setDestroyTime(int time)
    {
        destroyTime = time;
    }
    public void sethitEffect(ParticleSystem gunHitEffect)
    {
        hitEffect = gunHitEffect;
    }
}
