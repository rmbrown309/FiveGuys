using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CEOBossShockwave : MonoBehaviour
{
    [SerializeField] GameObject shockWaveVisualizer;
    [SerializeField] int damage;

    private bool isShockWaveEnabled = false;

    public void ShockWaveEnabled()
    {
        isShockWaveEnabled = true;
    }
    IEnumerator ShockwaveRoutine()
    {
        shockWaveVisualizer.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        shockWaveVisualizer.SetActive(false);
        isShockWaveEnabled = false;
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
        physicsable?.TakePhysics((other.transform.position - transform.position).normalized * (damage));
        Destroy(gameObject);
    }
}