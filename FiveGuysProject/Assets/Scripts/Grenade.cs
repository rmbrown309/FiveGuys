using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject explosion;
    [Header("----- Grenade Stats -----")]
    [SerializeField] float speed;
    [SerializeField] float destroyTime;
    [SerializeField] float velUp;

    bool isShoving;

    public bool isRocket;
    public Vector3 targetDir;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(timer());
    }

    IEnumerator timer()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point; // aims at specific point on ray at the distance of the hit
        else
            targetPoint = ray.GetPoint(50); // some distant point on ray if not aiming at anything

        // Calculate shooting direction
        Vector3 jankPos = GameManager.instance.player.transform.position;
        jankPos.z = jankPos.z + 0.5f;
        Vector3 shootDir = targetPoint - jankPos;

        rb.velocity = (Vector3.up * velUp) + (shootDir).normalized * speed;
        yield return new WaitForSeconds(destroyTime);
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, explosion.transform.rotation);
        }
      
        Destroy(gameObject);

    }
    private void OnTriggerEnter(Collider other )
    {
        //if ( isRocket)
        //{
            if (other.isTrigger)
            {
                return;
            }

            //IDamage damageable = other.GetComponent<IDamage>();

            //if (damageable != null)
            //{
            Instantiate(explosion, transform.position, explosion.transform.rotation);

            //}

            Destroy(gameObject);
        //}
       
    }

  

  
}
