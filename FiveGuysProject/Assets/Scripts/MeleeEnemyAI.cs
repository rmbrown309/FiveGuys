using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject powerSpawn;
    [Range(0, 1)][SerializeField] float powerSpawnPercentage;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;

    [Header("----- Melee Stats -----")]
    [SerializeField] float hitRate;
    [SerializeField] int hitAngle;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange; //advised to keep the stoping and melee range short

    bool isMeleeing;
    Vector3 playerDir;
    bool playerInRange;
    float angelToPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        //check if player is in range, to get the direction and tell it to move, and deal damage
        if (playerInRange && canSeePlayer())
        {
            Debug.DrawRay(shootPos.transform.position, shootPos.forward * meleeRange, Color.red);
        }
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    bool canSeePlayer()
    {

        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angelToPlayer = Vector3.Angle(playerDir, transform.forward);
        RaycastHit see;
        //checks to see if enemy can see player
        if (Physics.Raycast(headPos.position, playerDir, out see))
        {
            if (see.collider.CompareTag("Player") && angelToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (angelToPlayer <= hitAngle && !isMeleeing)
                {
                    StartCoroutine(melee());
                }
                return true;
            }
        }
        return false;
    }
    //stab time
    IEnumerator melee()
    {
        isMeleeing = true;
        if (Physics.Raycast(shootPos.transform.position, playerDir, out RaycastHit hit, meleeRange))
        {
            IDamage damagable = hit.collider.GetComponent<IDamage>();
            //checks to see if it is not hitting an enemy
            if (!hit.collider.CompareTag("Enemy") && damagable != null)
            {
                StartCoroutine(dealDamage());
                damagable.takeDamage(meleeDamage);
            }
        }
        yield return new WaitForSeconds(hitRate);
        isMeleeing = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashDamage());
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            //is dead
            GameManager.instance.UpdateWinCondition(-1);
            if (Random.value < powerSpawnPercentage)
            {
                GameObject PowerSpawn = Instantiate(powerSpawn, shootPos.position, Quaternion.identity);
            }
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
    //flash to communicate that it is dealing damage
    IEnumerator dealDamage()
    {
        model.material.color = Color.blue;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }    

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
