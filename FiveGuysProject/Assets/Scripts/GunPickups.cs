using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickups : MonoBehaviour
{
    [SerializeField] gunStats gun;
    // Start is called before the first frame update
    void Start()
    {
        gun.ammoCur = gun.ammoMax;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            GameManager.instance.playerScript.setGunStats(gun);
            Destroy(gameObject);
        }

    }
}
