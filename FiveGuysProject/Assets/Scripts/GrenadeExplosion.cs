using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] ParticleSystem explosionFX;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroy());
    }

    IEnumerator destroy()
    {
        if (explosionFX != null)
        {
            Instantiate(explosionFX, transform.position, explosionFX.transform.rotation);

        }
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        IDamage damagable = other.GetComponent<IDamage>();
        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
        IPhysics phys = other.GetComponent<IPhysics>();
        if (phys != null)
        {
            phys.TakePhysics((transform.position - other.transform.position).normalized * (damage * 15));
        }
        Destroy(gameObject);
    }
}
