using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoamingMeleeEnemy : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject powerSpawn;
    [Range(0, 1)] [SerializeField] float powerSpawnPercentage;
    [SerializeField] Animator anim;
    [SerializeField] Collider damageCol;

    [Header("----- Enemy Stats -----")]
    [SerializeField] float HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int despawnTime;
    [SerializeField] int pushBackResolve;

    [Header("----- Melee Stats -----")]
    [SerializeField] float hitRate;
    [SerializeField] int hitAngle;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange; //advised to keep the stoping and melee range short
    [SerializeField] int shootAngle;
    [SerializeField] Collider meleeCol;

    bool isMeleeing;
    private Vector3 pushBack;
    Vector3 playerDir;
    bool playerInRange;
    float angleToPlayer;
    bool destinationChosen;
    Vector3 startingPos;
    float stoppingDistOrig;

    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

    }

    void Update()
    {
        //Checks to see if enemy is alive
        if (agent.isActiveAndEnabled)
        {
            if (playerInRange && !canSeePlayer())
            {
                StartCoroutine(roam());
            }
            else if (!playerInRange)
            {
                StartCoroutine(roam());
            }

            //anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            //pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);

            //playerDir = GameManager.instance.player.transform.position - headPos.position;
            //angleToPlayer = Vector3.Angle(playerDir, transform.forward);

            //agent.SetDestination(GameManager.instance.player.transform.position);

            //if (agent.remainingDistance <= agent.stoppingDistance)
            //    faceTarget();

            //if (angleToPlayer <= hitAngle && !isMeleeing && playerInRange && damageCol.enabled)
            //    StartCoroutine(melee());

            //agent.Move((pushBack * 2) * Time.deltaTime);
        }
    }
    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

        pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);

        Debug.DrawRay(headPos.position, playerDir);
        Debug.Log(angleToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                    faceTarget();

                if (angleToPlayer <= hitAngle && !isMeleeing && playerInRange && damageCol.enabled)
                    StartCoroutine(melee());

                agent.Move((pushBack * 2) * Time.deltaTime);

                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    //stab time
    IEnumerator melee()
    {
        isMeleeing = true;
        //plays animation in which the "hitColOn/Off" functions are called
        anim.SetTrigger("Melee");
        yield return new WaitForSeconds(hitRate);
        isMeleeing = false;
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.05f && !destinationChosen)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);
            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
            destinationChosen = false;
        }
    }

    //Turns hit collider on and off
    public void hitColOn()
    {
        meleeCol.enabled = true;

    }
    public void hitColOff()
    {
        meleeCol.enabled = false;
    }

    public float GetHp()
    {
        return HP;
    }
    public void SetHP(float hp)
    {
        HP = hp;
    }
    public void takeDamage(float amount)
    {
        HP -= amount;
        //To fix bug of not turning the hit collider off when taking damage
        if (meleeCol != null)
        {
            hitColOff();
        }
        StartCoroutine(flashDamage());
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            //is dead
            GameManager.instance.UpdateWinCondition(-1);
            anim.SetBool("Dead", true);
            if (Random.value < powerSpawnPercentage)
            {
                GameObject PowerSpawn = Instantiate(powerSpawn, shootPos.position, Quaternion.identity);
            }
            //Destroy(gameObject);
            agent.enabled = false;
            damageCol.enabled = false;
            StopAllCoroutines();
            StartCoroutine(Despawn());
            GameManager.instance.IncreasePlayerScore(1);
        }
        else
        {
            anim.SetTrigger("Damage");
        }
    }

    public void TakePhysics(Vector3 dir)
    {
        pushBack += dir;
    }

    //Destorys the enemy after a specified amount of time
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
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
