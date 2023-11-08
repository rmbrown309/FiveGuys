using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    //[SerializeField] Animator anim;
    [SerializeField] Collider damageCol;

    [Header("----- Enemy Stats -----")]
    [SerializeField] float HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int despawnTime;

    [Header("----- Melee Stats -----")]
    [SerializeField] float hitRate;
    [SerializeField] int hitAngle;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange; //advised to keep the stoping and melee range short
    [SerializeField] Collider meleeCol;

    [Header("----- Jump Stats -----")]
    [SerializeField] float jumpMinDistance = 2f;
    [SerializeField] float jumpMaxDistance = 15f;
    [SerializeField] float jumpSpeed = 1f;
    [SerializeField] float jumpCooldown = 4f;
    [SerializeField] int jumpDamage = 1;
    public AnimationCurve jumpCurve;
    public bool isJumping;

    float jumpTime;

    bool isMeleeing;
    private Vector3 pushBack;
    Vector3 playerDir;
    bool playerInRange;
    float angelToPlayer;

    void Start()
    {

    }

    void Update()
    {
        //Checks to see if enemy is alive
        if (agent.isActiveAndEnabled)
        {
            //anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angelToPlayer = Vector3.Angle(playerDir, transform.forward);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
                faceTarget();

            if (!isJumping && (jumpTime + jumpCooldown < Time.time) && (agent.remainingDistance >= jumpMinDistance) && (agent.remainingDistance <= jumpMaxDistance))
                StartCoroutine(Jump(GameManager.instance.player.transform.position));

            if (angelToPlayer <= hitAngle && !isMeleeing && playerInRange && damageCol.enabled)
                StartCoroutine(melee());
        }
    }

    private IEnumerator Jump(Vector3 targetPosition)
    {
        isJumping = true;
        agent.enabled = false;

        //anim.SetTrigger("Jump");

        Vector3 startingPosition = transform.position;

        targetPosition -= new Vector3(1, 1, 1);

        for (float time = 0; time < 1; time += Time.deltaTime * jumpSpeed)
        {
            transform.position = Vector3.Lerp(startingPosition, (targetPosition), time) + Vector3.up * jumpCurve.Evaluate(time);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), time);

            yield return null;
        }
        //anim.SetTrigger("Landed");

        jumpTime = Time.time;

        agent.enabled = true;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, agent.areaMask))
        {
            agent.Warp(hit.position);
        }

        isJumping = false;
    }

    //stab time
    IEnumerator melee()
    {
        isMeleeing = true;
        //plays animation in which the "hitColOn/Off" functions are called
        //anim.SetTrigger("Melee");
        yield return new WaitForSeconds(hitRate);
        isMeleeing = false;
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
            //anim.SetBool("Dead", true);
            
            //Destroy(gameObject);
            agent.enabled = false;
            damageCol.enabled = false;
            StopAllCoroutines();
            StartCoroutine(Despawn());
            GameManager.instance.IncreasePlayerScore(1);
        }
        else
        {
            //anim.SetTrigger("Damage");
        }
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
