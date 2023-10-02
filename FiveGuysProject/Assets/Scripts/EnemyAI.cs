using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int targetFaceSpeed;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    bool isShooting;
    Vector3 playerDir;
    bool playerInRange;



    void Start()
    {
        //alerts game manager that an enemy exists so it can tick the counter.
        GameManager.instance.UpdateWinCondition(1);
    }

    void Update()
    {
        //if the player is in range then we get the direction and tell the nav agent to set its destination towards the player and start shooting
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                faceTarget();
            }
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (!isShooting)
                StartCoroutine(shoot());
        }
    }

    //sheeet we shootin. Shoots the bullet.
    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

    //triggers the flash for when the enemy takes damage and flashs to let the player know. 
    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            //it's dead; Needs trigger from game manager to indicate to gameManager that it died and flash win screen.
            GameManager.instance.UpdateWinCondition(-1);
            Destroy(gameObject);
            GameManager.instance.IncreasePlayerScore(1);
        }
    }

    IEnumerator flashDamage()
    {

        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;

    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
