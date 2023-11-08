using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] GameObject phaseTwoWeapon;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Collider damageCol;

    [Header("----- Enemy Stats -----")]
    [SerializeField] float HP;
    [SerializeField] float speed;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int despawnTime;

    [Header("----- Melee Stats -----")]
    [SerializeField] float hitRate;
    [SerializeField] int hitAngle;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange; //advised to keep the stoping and melee range short
    [SerializeField] Collider meleeCol;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int shootAngle;

    [Header("----- Jump Stats -----")]
    [SerializeField] float jumpMinDistance = 2f;
    [SerializeField] float jumpMaxDistance = 15f;
    [SerializeField] float jumpSpeed = 1f;
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] float jumpCooldown = 4f;
    [SerializeField] float jumpQuakeRadius = 4f;
    [SerializeField] int jumpDamage = 1;
    public AnimationCurve jumpCurve;
    public bool isJumping;

    [Header("----- Guard Stats -----")]
    [SerializeField] float minRunawayDistance = 10f;
    [SerializeField] float maxRunawayDistance = 30f;
    [SerializeField] float runawaySpeed = 9f;
    [SerializeField] float guardCooldown = 20f;
    [SerializeField] float guardHealRate = 1f;
    [SerializeField] float guardHealAmount = 1f;
    [SerializeField] float guardQuakeRadius = 15f;
    [SerializeField] int guardQuakeDamage = 1;
    [SerializeField] float stunnedTime = 3f;
    public bool isGuarding;
    public bool isStunned;

    float jumpTime;
    float guardTime;

    bool phaseOne = true;
    bool phaseTwo = false;

    bool isMeleeing;
    bool isShooting;
    float OrigHP;
    Vector3 playerDir;
    bool playerInRange;
    float angleToPlayer;

    void Start()
    {
        agent.speed = speed;
        OrigHP = HP;
    }

    void Update()
    {
        //Checks to see if enemy is alive
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

            if (isStunned)
                StartCoroutine(Stunned());

            if (!isGuarding && !isStunned)
                agent.SetDestination(GameManager.instance.player.transform.position);

            if (!isGuarding && !isStunned && !isMeleeing && !isShooting && !isJumping && (guardTime + guardCooldown < Time.time) && HP < OrigHP)
                StartCoroutine(Guard());

            if (!isGuarding && !isStunned && agent.remainingDistance <= agent.stoppingDistance)
                faceTarget();

            if (!isGuarding && !isStunned && !isMeleeing && !isShooting && !isJumping && (jumpTime + jumpCooldown < Time.time) 
                && (agent.remainingDistance >= jumpMinDistance) && (agent.remainingDistance <= jumpMaxDistance))
                StartCoroutine(Jump(GameManager.instance.player.transform.position));

            if (phaseOne && angleToPlayer <= hitAngle && !isMeleeing && !isJumping && !isGuarding && !isStunned && playerInRange && damageCol.enabled)
                StartCoroutine(melee());

            if (phaseTwo && angleToPlayer <= shootAngle && !isShooting && !isGuarding && !isStunned && playerInRange && damageCol.enabled)
                StartCoroutine(shoot());

            if (phaseOne && HP <= 0 && !isJumping)
                StartCoroutine(PhaseChange());
            
        }
    }

    IEnumerator Jump(Vector3 targetPosition)
    {
        isJumping = true;
        agent.enabled = false;

        Vector3 startingPosition = transform.position;

        targetPosition -= new Vector3(1, 1, 1);

        for (float time = 0; time < 1; time += Time.deltaTime * jumpSpeed)
        {
            transform.position = Vector3.Lerp(startingPosition, (targetPosition), time) + (Vector3.up * jumpHeight) * jumpCurve.Evaluate(time);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), time);

            yield return null;
        }

        jumpTime = Time.time;

        agent.enabled = true;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, jumpQuakeRadius);
        foreach (Collider c in hits)
        {
            PlayerController playerHit = c.GetComponent<PlayerController>();
            if (playerHit != null && playerHit.jumpedTimes == 0)
                playerHit.takeDamage(jumpDamage);
        }

        isJumping = false;

    }

    IEnumerator Guard()
    {
        isGuarding = true;
        model.material.color = Color.green;

        Vector3 randomPoint = transform.position + Random.insideUnitSphere * maxRunawayDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, minRunawayDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            agent.speed = runawaySpeed;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.enabled = false;
        }

        anim.SetBool("Guard", true);

        while (!isStunned && HP < OrigHP)
        {
            yield return new WaitForSeconds(guardHealRate);
            HP += guardHealAmount;
            Collider[] hits = Physics.OverlapSphere(transform.position, guardQuakeRadius);
            foreach (Collider c in hits)
            {
                PlayerController playerHit = c.GetComponent<PlayerController>();
                if (playerHit != null && playerHit.jumpedTimes == 0)
                    playerHit.takeDamage(guardQuakeDamage);
            }
        }

        guardTime = Time.time;
        isGuarding = false;

        if(!isStunned)
        {
            anim.SetBool("Guard", false);
            model.material.color = Color.white;
            agent.enabled = true;
            agent.speed = speed;
        }
    }

    IEnumerator Stunned()
    {
        anim.SetBool("Guard", false);
        anim.SetBool("Stunned", true);
        model.material.color = Color.black;
        agent.enabled = false;
        GameManager.instance.playerScript.ShootRate(2);

        yield return new WaitForSeconds(stunnedTime);

        model.material.color = Color.white;
        anim.SetBool("Stunned", false);
        agent.enabled = true;
        agent.speed = speed;
        isStunned = false;
    }

    //stab time
    IEnumerator melee()
    {
        isMeleeing = true;
        //plays animation in which the "hitColOn/Off" functions are called
        anim.SetTrigger("Kick");
        yield return new WaitForSeconds(hitRate);
        isMeleeing = false;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        //anim.SetTrigger("Shoot");

        GameObject currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        currBullet.transform.forward = playerDir.normalized;
        yield return new WaitForSeconds(0.08f);
        currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        currBullet.transform.forward = playerDir.normalized;
        yield return new WaitForSeconds(0.08f);
        currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        currBullet.transform.forward = playerDir.normalized;

        yield return new WaitForSeconds(shootRate);
        isShooting = false;

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
        if (!isGuarding)
        {
            HP -= amount;
            //To fix bug of not turning the hit collider off when taking damage
            if (meleeCol != null)
            {
                hitColOff();
            }
            StartCoroutine(flashDamage());
            if(!isJumping && !isStunned)
                agent.SetDestination(GameManager.instance.player.transform.position);
            if (phaseTwo && HP <= 0)
            {
                //is dead
                GameManager.instance.FinalBossDead = true;
                GameManager.instance.UpdateWinCondition(-GameManager.instance.enemiesRemain);
                //anim.SetBool("Dead", true);

                //Destroy(gameObject);
                agent.enabled = false;
                damageCol.enabled = false;
                StopAllCoroutines();
                StartCoroutine(Despawn());
                GameManager.instance.IncreasePlayerScore(100);
            }
            else
            {
                //anim.SetTrigger("Damage");
            }
        }
    }

    public void TakePhysics(Vector3 amount)
    {
        if(isGuarding)
            isStunned = true;
    }

    IEnumerator PhaseChange()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        anim.SetBool("Guard", true);
        anim.SetBool("Stunned", false);
        phaseOne = false;
        agent.enabled = false;
        damageCol.enabled = false;
        yield return new WaitForSeconds(3);

        anim.SetBool("Guard", false);
        yield return new WaitForSeconds(0.5f);

        anim.SetBool("GlockOut", true);
        OrigHP *= 2;
        HP = OrigHP;
        phaseTwoWeapon.SetActive(true);
        agent.enabled = true;
        damageCol.enabled = true;
        gameObject.GetComponent<SphereCollider>().radius = 6;
        phaseTwo = true;
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
