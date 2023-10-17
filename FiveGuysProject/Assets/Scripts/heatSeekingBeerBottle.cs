using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSeekingBeerBottle : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet Stats -----")]
    public int damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    //Bullet Tracker; Update is hopefully called once per frame
    private void Update()
    {
        rb.velocity = speed * Time.deltaTime * (GameManager.instance.player.transform.position - transform.position);
    }

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

        Destroy(gameObject);
    }

    public void setDestroyTime(int time)
    {
        destroyTime = time;
    }
}