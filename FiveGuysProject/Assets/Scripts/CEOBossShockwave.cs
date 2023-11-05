using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CEOBossShockwave : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] int pointsCount;
    [SerializeField] float force;
    [SerializeField] float maxRadius;
    [SerializeField] float speed;
    [SerializeField] float startWidth;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointsCount + 1;
    }
    private void Update()
    {
        StartCoroutine(ShockWave());
    }
    private IEnumerator ShockWave()
    {
        float currentRadius = 0f;
        while (currentRadius < maxRadius)
        {
            //No radius draw while zero
            currentRadius += Time.deltaTime * speed;
            Draw(currentRadius);
            Damage(currentRadius);
            yield return null;
        }
    }
    private void Damage(float currentRadius)
    {
        Collider[] hittingObjects = Physics.OverlapSphere(transform.position, currentRadius);

        for (int i = 0; i < hittingObjects.Length; i++)
        {
            Rigidbody rb = hittingObjects[i].GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 direction = (hittingObjects[i].transform.position - transform.position).normalized;
                rb.AddForce(direction * force, ForceMode.Impulse);
            }
        }
    }
    private void Draw(float currentRadius)
    {
        float circlePoints = 360f / pointsCount;
        for (int i = 0; i < pointsCount; i++) 
        {
            float angle = i * circlePoints * Mathf.Deg2Rad;
            Vector3 direction = new(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * currentRadius;
            lineRenderer.SetPosition(i, position);
        }
        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage damagable = other.GetComponent<IDamage>();
        damagable?.takeDamage(damage);
        IPhysics phys = other.GetComponent<IPhysics>();
        phys?.TakePhysics((transform.position - other.transform.position).normalized * (damage));
        Destroy(gameObject);
    }
}