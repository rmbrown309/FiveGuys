using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDrunk : MonoBehaviour, IDamage
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

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int shootAngle;

    [Header("----- Boss Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] bossbark;
    [Range(0, 1)][SerializeField] float bossBarkVol;
    [Range(0, 1)] [SerializeField] float hitMarkerVol;
    [SerializeField] AudioClip[] hitMarkerAud;

    private Rigidbody[] rigidBodies;
    private CharacterController charController;

    bool isShooting;
    Vector3 playerDir;
    bool playerInRange;
    float angleToPlayer;
    Vector3 spawnPos;

    void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        charController = GetComponent<CharacterController>();
        DisableRagDoll();
    }

    void Start()
    {

    }
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                faceTarget();
            }

            if (angleToPlayer <= shootAngle && !isShooting && playerInRange && damageCol.enabled)
            {
                StartCoroutine(shoot());
            }
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
        aud.pitch = Random.Range(0.95f, 1.05f);
        aud.PlayOneShot(hitMarkerAud[Random.Range(0, hitMarkerAud.Length)], hitMarkerVol);
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
            GameManager.instance.UpdateWinCondition(-1);
            agent.enabled = false;
            damageCol.enabled = false;
            StopAllCoroutines();
            StartCoroutine(Despawn());
            GameManager.instance.IncreasePlayerScore(2);
        }
        else
        {
            anim.SetTrigger("Damage");
        }
    }
    IEnumerator Despawn()
    {
        EnableRagdoll();
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
            aud.PlayOneShot(bossbark[Random.Range(0, bossbark.Length)], bossBarkVol);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void DisableRagDoll()
    {
        foreach (var rigidbody in rigidBodies)
        {
            rigidbody.isKinematic = true;
        }
        //anim.enabled = true;
        //charController.enabled = true;
    }

    void EnableRagdoll()
    {
        anim.enabled = false;
        if (charController != null)
        {
            charController.enabled = false;
        }
        foreach (var rigidbody in rigidBodies)
        {
            rigidbody.isKinematic = false;
        }
    }
}