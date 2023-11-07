using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RatPatrolAI : MonoBehaviour, ISpray
{
    [Header("----- Components -----")]
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] Transform patrolCenterPoint;
    [SerializeField] float patrolRange;

    [SerializeField] int damage;

    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [Range(0, 1)] [SerializeField] float idleChatterVol;
    [SerializeField] AudioClip[] idleChatter;
    [Range(0, 1)] [SerializeField] float idleChatterPlayPercentage;

    void Start()
    {
        
    }

    void Update()
    {
        // sets a new destination whenever stopped
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if(RandomPoint(patrolCenterPoint.position, out point))
            {
                //Debug.DrawRay(point, Vector3.up, Color.red, 1.0f); // just to see where its next destination is
                agent.SetDestination(point);
            }
        }
    }

    // damages others on contact
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();
        PlayerController playerScript = other.GetComponent<PlayerController>();
        if (damageable != null && playerScript != null)
        {
            damageable.takeDamage(damage);
            Destroy(gameObject);
        }

    }

    // finds a random point in the patrolRange to set as the next destination
    bool RandomPoint(Vector3 center, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * patrolRange;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) 
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void ISpray.kill()
    {
        Destroy(gameObject);
        GameManager.instance.IncreasePlayerScore(1);
    }
}
