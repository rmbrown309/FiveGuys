using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage, IPhysics
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

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
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

            //if the player is in range then we get the direction and tell the nav agent to set its destination towards the player and start shooting
            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance < agent.stoppingDistance)
                faceTarget();

            if (angleToPlayer <= shootAngle && !isShooting && playerInRange && damageCol.enabled)
                StartCoroutine(shoot());

            agent.Move((agent.velocity + pushBack) * Time.deltaTime);
        }
            
        
    }

    //bool canSeePlayer()
    //{
    //    playerDir = GameManager.instance.player.transform.position - headPos.position;
    //    angleToPlayer = Vector3.Angle(playerDir, transform.forward);


    //    Debug.DrawRay(headPos.position, playerDir);
    //    Debug.Log(angleToPlayer);

    //    RaycastHit hit;
    //    if (Physics.Raycast(headPos.position, playerDir, out hit))
    //    {
    //        if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
    //        {
    //            agent.SetDestination(GameManager.instance.player.transform.position);

    //            if (agent.remainingDistance < agent.stoppingDistance)
    //                faceTarget();

    //            if (angleToPlayer <= shootAngle && !isShooting)
    //                StartCoroutine(shoot());
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //sheeet we shootin. Shoots the bullet.
    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        //GameObject currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        //allows enemies to shoot vertically toward player
        //currBullet.transform.forward = playerDir.normalized;

        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

    public void CreateBullet()
    {
        GameObject currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        currBullet.transform.forward = playerDir.normalized;
    }

    //triggers the flash for when the enemy takes damage and flashs to let the player know. 
    public void takeDamage(int amount)
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
            //it's dead; Needs trigger from game manager to indicate to gameManager that it died and flash win screen.
            anim.SetBool("Dead", true);
            GameManager.instance.UpdateWinCondition(-1);
            if (Random.value < powerSpawnPercentage)
            {
                spawnPos = new Vector3(transform.position.x, 1, transform.position.z);
                GameObject PowerSpawn = Instantiate(powerSpawn, spawnPos , Quaternion.identity);
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

    //Take damage and get pushed back
    public void takePhysics(Vector3 dir)
    {
        pushBack += dir;
    }

    //Destroys the enemy after a specified amount of time
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
    public int GetHp()
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
