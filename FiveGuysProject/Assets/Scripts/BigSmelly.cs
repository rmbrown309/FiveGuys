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

    //[Header("----- Squish Stats -----")]
    //[SerializeField] float squishOnY;
    //[SerializeField] float timeToReturnY;
    //[SerializeField] float afterHitTime;
    //[SerializeField] AnimationCurve curve;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int shootAngle;

    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [Range(0, 1)][SerializeField] float idleChatterVol;
    [SerializeField] AudioClip[] idleChatter;
    [Range(0, 1)][SerializeField] float idleChatterPlayPercentage;
    [SerializeField] float idleCoolDown;
    [Range(0, 1)] [SerializeField] float hitMarkerVol;
    [SerializeField] AudioClip[] hitMarkerAud;

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

            if (Random.value < idleChatterPlayPercentage)
            {
                StartCoroutine(RandomIdleChat());
            }

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
        //if (isShooting = true && HP > 0)
        //{
        //    StartCoroutine(Squish());
        //}
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
            GameManager.instance.IncreasePlayerScore(2);
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
    //IEnumerator Squish()
    //{
    //    Vector3 origScale = new(gameObject.transform.localScale.x, 0.5f, gameObject.transform.localScale.z);
    //    Vector3 toSquish = new(gameObject.transform.localScale.x, gameObject.transform.localScale.y - squishOnY, gameObject.transform.localScale.z);
    //    float timeElapsed = 0;
    //    while (timeElapsed < timeToReturnY)
    //    {
    //        float t = timeElapsed / timeToReturnY;
    //        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, toSquish, curve.Evaluate(t));
    //        timeElapsed += Time.deltaTime;
    //    }
    //    yield return new WaitForSeconds(0.1f);
    //    timeElapsed = 0;
    //    while (timeElapsed < timeToReturnY)
    //    {
    //        float t = timeElapsed / timeToReturnY;
    //        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, origScale, curve.Evaluate(t));
    //        timeElapsed += Time.deltaTime;
    //    }
    //    yield return new WaitForSeconds(0.1f);
    //}
    IEnumerator RandomIdleChat()
    {
        if (Random.value < idleChatterPlayPercentage)
        {
            float randPitch = Random.Range(0.95f, 1.05f);
            aud.pitch = randPitch;
            aud.PlayOneShot(idleChatter[Random.Range(0, idleChatter.Length)], idleChatterVol);
        }
        yield return new WaitForSeconds(idleCoolDown);
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
