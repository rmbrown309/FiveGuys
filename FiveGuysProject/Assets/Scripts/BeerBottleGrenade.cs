using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottleGrenade : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet Stats -----")]
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] GameObject explosion;
    [SerializeField] int VelUp;

    Vector3 targetDir;

    private void Start()
    {
        StartCoroutine(Timer());
    }
    IEnumerator Timer()
    {
        rb.velocity = (Vector3.up * 5) + (GameManager.instance.player.transform.position - transform.position).normalized * speed;
        yield return new WaitForSeconds(destroyTime);
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, explosion.transform.rotation);
        }
        Destroy(gameObject);
    }
}
