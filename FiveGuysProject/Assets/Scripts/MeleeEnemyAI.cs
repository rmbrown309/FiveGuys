using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour, IDamage, IPhysics
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
    [SerializeField] ParticleSystem spawnFx;

    [Header("----- Enemy Stats -----")]
    [SerializeField] float HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int despawnTime;
    [SerializeField] int pushBackResolve;

    [Header("----- Squish Stats -----")]
    [SerializeField] float squishOnY;
    [SerializeField] float timeToReturnY;
    [SerializeField] float afterHitTime;
    [SerializeField] AnimationCurve curve;

    [Header("----- Melee Stats -----")]
    [SerializeField] float hitRate;
    [SerializeField] int hitAngle;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange; //advised to keep the stoping and melee range short
    [SerializeField] Collider meleeCol;

    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [Range(0, 1)] [SerializeField] float idleChatterVol;
    [SerializeField] AudioClip[] idleChatter;
    [Range(0, 1)] [SerializeField] float idleChatterPlayPercentage;
    [SerializeField] float idleCoolDown;
    [Range(0, 1)] [SerializeField] float hitMarkerVol;
    [SerializeField] AudioClip[] hitMarkerAud;

    //ragdoll shennanigans
    private Rigidbody[] rigidBodies;
    private CharacterController charController;

    bool isMeleeing;
    private Vector3 pushBack;
    Vector3 playerDir;
    bool playerInRange;
    float angelToPlayer;

    void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        charController = GetComponent<CharacterController>();
        DisableRagDoll();
        spawnFx.Play();
    }

    void Start()
    {
        
    }

    void Update()
    {
        //Checks to see if enemy is alive
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);

            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angelToPlayer = Vector3.Angle(playerDir, transform.forward);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (Random.value < idleChatterPlayPercentage)
            {
                StartCoroutine(RandomIdleChat());
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
                faceTarget();

            if (angelToPlayer <= hitAngle && !isMeleeing && playerInRange && damageCol.enabled)
                StartCoroutine(melee());

            agent.Move((pushBack * 2) * Time.deltaTime);
        }
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
        aud.pitch = Random.Range(0.95f, 1.05f);
        aud.PlayOneShot(hitMarkerAud[Random.Range(0, hitMarkerAud.Length)], hitMarkerVol);
        HP -= amount;
        //To fix bug of not turning the hit collider off when taking damage
        if (meleeCol != null)
        {
            hitColOff();
        }
        if (HP > 0)
            StartCoroutine(Squish());
        StartCoroutine(flashDamage());
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            //is dead
            Vector3 origScale = new Vector3(0.5f, 0.5f, 0.5f);
            gameObject.transform.localScale = origScale;
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

        while (timeElapsed < timeToReturnY)
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
