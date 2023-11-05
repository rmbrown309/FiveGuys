using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CEOBoss : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Collider damageCol;

    [Header("----- Enemy Stats -----")]
    [SerializeField] float HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] float despawnTime;
    [SerializeField] int pushBackResolve;

    [Header("----- Melee Stats -----")]
    [SerializeField] float hitRate;
    [SerializeField] int hitAngle;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange; //advised to keep the stoping and melee range short
    [SerializeField] Collider meleeCol;

    bool isMeleeing;
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
            angleToPlayer = Vector3.Angle(playerDir, transform.forward);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            if (angleToPlayer <= hitAngle && !isMeleeing && playerInRange && damageCol.enabled)
            {
                StartCoroutine(melee());
            }

            agent.Move((pushBack * 2) * Time.deltaTime);
        }
    }
    IEnumerator melee()
    {
        isMeleeing = true;
        anim.SetTrigger("Melee");
        yield return new WaitForSeconds(hitRate);
        isMeleeing = false;
    }
    public void hitColOn()
    {
        meleeCol.enabled = true;
    }
    public void hitColOff()
    {
        meleeCol.enabled = false;
    }
    public void takeDamage(float amount)
    {
        HP -= amount;
        if (meleeCol != null)
        {
            hitColOff();
        }
        StartCoroutine(flashDamage());
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            GameManager.instance.FinalBossDead = true;
            GameManager.instance.UpdateWinCondition(-1);
            anim.SetBool("Dead", true);
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