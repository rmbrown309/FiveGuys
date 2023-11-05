using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottleHazard : MonoBehaviour
{
    [SerializeField] int slowSpeed;
    [SerializeField] ParticleSystem beerPuddle;

    //void Start()
    //{
    //    StartCoroutine(DestroyPuddle());
    //}

    //IEnumerator DestroyPuddle()
    //{
    //    //if (beerPuddle != null)
    //    //{
    //    //    Instantiate(beerPuddle, transform.position, beerPuddle.transform.rotation);
    //    //}
    //    yield return new WaitForSeconds(0.1f);
    //    Destroy(gameObject);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.transform != this.transform)
        {
            return;
        }
        IPower powerUp = other.GetComponent<IPower>();
        powerUp?.SpeedBoost(slowSpeed);
        Destroy(gameObject);
    }
}