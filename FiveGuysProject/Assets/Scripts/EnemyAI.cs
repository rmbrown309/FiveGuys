using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    //[SerializeField] GameObject ragdoll;

    [Header("----- Enemy Stats -----")]
    [SerializeField] float HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] float despawnTime;
    [SerializeField] int pushBackResolve;

    [Header("----- Squish Stats -----")]
    [SerializeField] float squishOnY; 
    [SerializeField] float timeToReturnY;
    [SerializeField] float afterHitTime;
    [SerializeField] AnimationCurve curve;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int shootAngle;

    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [Range(0, 1)] [SerializeField] float hitVol;
    [SerializeField] AudioClip[] hitAud;
    [Range(0, 1)] [SerializeField] float idleChatterVol;
    [SerializeField] AudioClip[] idleChatter;
    [Range(0, 1)] [SerializeField] float idleChatterPlayPercentage;
    [SerializeField] float idleCoolDown;

    //ragdoll shennanigans
    private Rigidbody[] rigidBodies;
    private CharacterController charController; 

    bool isShooting;
    private Vector3 pushBack;
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
        //checks to see if enemy is alive
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);

            //if the player is in range then we get the direction and tell the nav agent to set its destination towards the player and start shooting
            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

            agent.SetDestination(GameManager.instance.player.transform.position);
            if (Random.value < idleChatterPlayPercentage)
            {
                StartCoroutine(RandomIdleChat());
            }

            if (agent.remainingDistance < agent.stoppingDistance)
                faceTarget();

            if (angleToPlayer <= shootAngle && !isShooting && playerInRange && damageCol.enabled)
                StartCoroutine(shoot());

            agent.Move((pushBack) * Time.deltaTime);

        }

    


    }

    //sheeet we shootin. Shoots the bullet.
    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        //CreateBullet();
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
    public void takeDamage(float amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (agent.remainingDistance < agent.stoppingDistance) 
        {

            playerDir = GameManager.instance.player.transform.position - headPos.position;
            faceTarget();
        }

        if (HP > 0)
            StartCoroutine(Squish());

        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            //it's dead; Needs trigger from game manager to indicate to gameManager that it died and flash win screen.
            Vector3 origScale = new Vector3(0.5f, 0.5f, 0.5f);

            gameObject.transform.localScale = origScale;
            //anim.SetBool("Dead", true);
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
            anim.SetBool("Damage", true);


        }

    }

    //Take damage and get pushed back
    public void TakePhysics(Vector3 dir)
    {
        pushBack += dir;
    }

    //Destroys the enemy after a specified amount of time
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

    IEnumerator Squish()
    {
        Vector3 origScale = new Vector3(gameObject.transform.localScale.x, 0.5f, gameObject.transform.localScale.z);
        Vector3 toSquish = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y - squishOnY, gameObject.transform.localScale.z);

  

        float timeElapsed = 0;

        while(timeElapsed < timeToReturnY)
        {
            float t = timeElapsed / timeToReturnY;
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, toSquish, curve.Evaluate(t));
            timeElapsed += Time.deltaTime;

        }
        yield return new WaitForSeconds(0.1f);

        timeElapsed = 0;
        while (timeElapsed < timeToReturnY)
        {
            float t = timeElapsed / timeToReturnY;
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, origScale, curve.Evaluate(t));
            timeElapsed += Time.deltaTime;

        }
        yield return new WaitForSeconds(0.1f);



    }

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
    public void SetHP(float health)
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
