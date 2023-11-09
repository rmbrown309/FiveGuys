using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BeerBottleGrenade : MonoBehaviour
{
    //[Header("----- Raycast Position -----")]
    //public LayerMask groundLayer;

    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet Stats -----")]
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject beerPuddle;
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
        if (beerPuddle != null) 
        {
            //PuddleOnGround(targetDir);
            Instantiate(beerPuddle, (transform.position * 0.97f), beerPuddle.transform.rotation);
        }
        Destroy(gameObject, destroyTime);
    }
    //void PuddleOnGround(Vector3 position)
    //{
    //    if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
    //    {
    //        Vector3 groundSpot = transform.position - hit.point + Vector3.up * .005f;
    //        Instantiate(beerPuddle, groundSpot, beerPuddle.transform.rotation);
    //    }
    //}
}