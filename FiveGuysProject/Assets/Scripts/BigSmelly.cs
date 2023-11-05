using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BigSmelly : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject powerSpawn;
    [Range(0, 1)][SerializeField] float powerSpawnPercentage;
    [SerializeField] Animator anim;
    [SerializeField] Collider damageCol;
    [SerializeField] ParticleSystem noxiousGas;

    [Header("----- Enemy Stats -----")]
    [SerializeField] float HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] float despawnTime;
    [SerializeField] int pushBackResolve;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int shootAngle;

    bool isShooting;
    private Vector3 pushBack;
    Vector3 playerDir;
    bool playerInRange;
    float angleToPlayer;
    Vector3 spawnPos;

    void Start()
    {

    }
    void Update()
    {
        //checks to see if enemy is alive
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);
            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            if (angleToPlayer <= shootAngle && !isShooting && playerInRange && damageCol.enabled)
            {
                StartCoroutine(shoot());
            }

            agent.Move((pushBack) * Time.deltaTime);
        }
    }
    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void CreateBullet()
    {
        GameObject currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        currBullet.transform.forward = playerDir.normalized;
    }
    public void takeDamage(float amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            playerDir = GameManager.instance.player.transform.position - headPos.position;
            faceTarget();
        }

        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            anim.SetBool("Dead", true);
            //Create the gas when enemy dies
            GameObject gasObject = Instantiate(noxiousGas.gameObject, headPos.position, Quaternion.identity);
            Destroy(gasObject, 10.0f);
            GameManager.instance.UpdateWinCondition(-1);
            if (Random.value < powerSpawnPercentage)
            {
                spawnPos = new Vector3(transform.position.x, 1, transform.position.z);
                GameObject PowerSpawn = Instantiate(powerSpawn, spawnPos, Quaternion.identity);
            }
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
    public void SetHP(int health)
    {
        HP = health;
    }
    public float GetHp()
    {
        return HP;
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
