using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IPower
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform shootPos;

    [Header("----- Player Stats -----")]
    [Range(1, 15)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] float playerSpeed;
    [Range(1, 3)][SerializeField] float sprintMod; // amount playerSpeed is multiplied when sprinting
    [Range(1, 3)][SerializeField] int jumpMax; // number of jumps player can perform before landing
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-10, -40)][SerializeField] float gravityValue;
    [SerializeField] float healthRegainSpeed;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpedTimes;
    
    bool isShooting;

    [Header("-----PowerUp Settings-----")]
    GameObject[] enemyToFind;
    [SerializeField] float waitT;
    bool[] powerActive = new bool[5];
    int powerIndex;
    private IEnumerator type1Routine;
    private IEnumerator type2Routine;
    private IEnumerator type3Routine;
    private IEnumerator type4Routine;
    private IEnumerator type5Routine;
    private int origJump;
    private float origHealthRegen;
    private float origShootRate;
    private int origEnemyHp;
    private int HPOrig;
    private float OrigSpeed;

    //Regen detectors
    bool isDamaged;
    bool damageActive;
    //Power Up
    bool isPowered;
    int powerType;

    private void Start()
    {
        enemyToFind = GameObject.FindGameObjectsWithTag("Enemy1");
        powerIndex = -1;
        origJump = jumpMax;
        OrigSpeed = playerSpeed;
        HPOrig = HP;
        origHealthRegen = healthRegainSpeed;
        origShootRate = shootRate;
        if (GameManager.instance.playerSpawnPoint != null)
        {
            SpawnPlayer();
        }
        if (GameManager.instance.enemiesRemain == 0)
            GameManager.instance.noEnemies = true;
    }

    void Update()
    {
        Movement();

        if (Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(Shoot());

        //if player got damaged AND there isnt an active regen happening
        //then regen health
        if (isDamaged && !damageActive)
        {
            StartCoroutine(RegainHelth());
        }
        switch (powerIndex)
        {
            case 0:
                powerIndex = -1;
                if (type1Routine != null)
                {
                    StopCoroutine(type1Routine);
                    powerActive[0] = false;
                }
                type1Routine = JumpPowerCooldown();
                StartCoroutine(type1Routine);
                break;
            case 1:
                powerIndex = -1;
                if (type2Routine != null)
                {
                    StopCoroutine(type2Routine);
                    powerActive[1] = false;
                }
                type2Routine = SpeedPowerCooldown();
                StartCoroutine(type2Routine);
                break;
            case 2:
                powerIndex = -1;
                if (type3Routine != null)
                {
                    StopCoroutine(type3Routine);
                    powerActive[2] = false;
                }
                type3Routine = HealthPowerCooldown();
                StartCoroutine(type3Routine);
                break;
            case 3:
                powerIndex = -1;
                if (type4Routine != null)
                {
                    StopCoroutine(type4Routine);
                    powerActive[3] = false;
                }
                type4Routine = ShootRatePowerCooldown();
                StartCoroutine(type4Routine);
                break;
            case 4:
                powerIndex = -1;
                if (type5Routine != null)
                {
                    StopCoroutine(type5Routine);
                    powerActive[4] = false;
                }
                type5Routine = EnemyHpPowerCooldown();
                StartCoroutine(type5Routine);
                break;
        }
    }

    void Movement()
    {
        Sprint();

        // Keeps player velocity from going negative and resets ability to jump while grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        // wasd movement input
        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        controller.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }

        // makes gravity work on player
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void Sprint()
    {
        // start sprint
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
        }
        // stop sprint
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        // Find hit position with raycast
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point; // aims at specific point on ray at the distance of the hit
        else
            targetPoint = ray.GetPoint(50); // some distant point on ray if not aiming at anything

        // Calculate shooting direction
        Vector3 shootDir = targetPoint - shootPos.position;

        // Instantiates bullet object and redirects its rotation toward the shootDir
        GameObject currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        currBullet.transform.forward = shootDir.normalized;

        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }
    IEnumerator JumpPowerCooldown()
    {
        yield return new WaitForSeconds(waitT);
        jumpMax = origJump;
        powerActive[0] = false;
    }
    IEnumerator SpeedPowerCooldown()
    {
        yield return new WaitForSeconds(waitT);
        playerSpeed = OrigSpeed;
        powerActive[1] = false;
    }

    IEnumerator HealthPowerCooldown()
    {
        yield return new WaitForSeconds(waitT);
        healthRegainSpeed = origHealthRegen;
        powerActive[2] = false;
    }
    IEnumerator ShootRatePowerCooldown()
    {
        yield return new WaitForSeconds(waitT);
        shootRate = origShootRate;
        powerActive[3] = false;
    }
    IEnumerator EnemyHpPowerCooldown()
    {
        yield return new WaitForSeconds(waitT);
        for (int i = 0; i < enemyToFind.Length; i++)
        {
            if (enemyToFind[i] != null)
            {
                EnemyAI enemyScript = enemyToFind[i].GetComponent<EnemyAI>();

                enemyScript.SetHP(origEnemyHp);
            }
        }
        powerActive[4] = false;
    }
    public void JumpPower(int jumps)
    {
        jumpMax = jumps;
        powerActive[0] = true;
        powerIndex = 0;
    }
    public void SpeedBoost(float speed)
    {
        playerSpeed = speed;
        powerActive[1] = true;
        powerIndex = 1;
    }
    public void HealthRegen(float regen)
    {
        healthRegainSpeed = regen;
        powerActive[2] = true;
        powerIndex = 2;
    }
    public void ShootRate(float shoot)
    {
        shootRate = shoot;
        powerActive[3] = true;
        powerIndex = 3;
    }
    public void DamageUp(int damage)
    {
        for (int i = 0; i < enemyToFind.Length; i++)
        {
            if (enemyToFind[i] != null)
            {
                EnemyAI enemyScript = enemyToFind[i].GetComponent<EnemyAI>();
                if (origEnemyHp == 0)
                {
                    origEnemyHp = enemyScript.GetHp();
                }
                enemyScript.SetHP(damage);
            }
            
        }
        powerActive[4] = true;
        powerIndex = 4;
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        isDamaged = true;
        UpdatePlayerUI();

        // Future Implementation for when a game over menu needs to appear
        if (HP < 1)
        {
            GameManager.instance.GameOver();
        }
    }
    IEnumerator RegainHelth()
    {
        //set active regen
        //reset damage
        damageActive = true;
        isDamaged = false;
        float startWait = 0;
        while (startWait < healthRegainSpeed)
        {
            //wait one second then add to counter
            yield return new WaitForSeconds(1);
            startWait++;
            //if the player got damaged while regen, exit regen state
            if (isDamaged)
            {
                startWait = 0;
                damageActive = false;
                break;
            }
        }
        //if the player got damaged, skip the regen animation
        if (!isDamaged)
        {
            HP = HPOrig;
            UpdatePlayerUI();
            startWait = 0;
            damageActive = false;
        }
    }
    public void SpawnPlayer()
    {
        HP = HPOrig;
        GameManager.instance.playerHealthBar.CrossFadeAlpha(0, 0, false);
        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawnPoint.transform.position;
        controller.enabled = true;
    }
    void UpdatePlayerUI()
    {
        //fade the health in and out
        GameManager.instance.playerHealthBar.CrossFadeAlpha((1 - ((float)HP / HPOrig)), (float).8, false);

    }
}
