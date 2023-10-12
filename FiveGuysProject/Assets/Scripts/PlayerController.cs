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
    //[SerializeField] List<gunStats> gunList = new List<gunStats>(); // the amount of guns currently on the player.
    [SerializeField] GameObject gunModel; // the model of the players curr gun
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int startDamage;

    [Header("----- Rat Spray Stats -----")]
    [SerializeField] ParticleSystem sprayEffect;
    [SerializeField] int maxSprayAmmo;
    public int currSprayAmmo;
    [SerializeField] float sprayRegenSpeed;

    [Header("----- PowerUp Settings -----")]
    [SerializeField] float waitT;

    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audSpray;
    [Range(0, 1)][SerializeField] float audSprayVol;

    // Activates rat spray
    private bool sprayWeaponActive = true;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpedTimes;
    private int gunDamage;

    bool isShooting;
    bool isSpraying;
    bool sprayRegen;
    bool isSprinting;

    //int selectedGun; // the int that controls how the player selects their gun

    // powerup variables
    GameObject[] enemyToFind;
    bool[] powerActive = new bool[5];
    int powerIndex;
    private IEnumerator type1Routine;
    private IEnumerator type2Routine;
    private IEnumerator type3Routine;
    private IEnumerator type4Routine;
    private IEnumerator type5Routine;
    private int origJump;
    private bool isInvulnerable;
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
        powerIndex = -1;
        origJump = jumpMax;
        OrigSpeed = playerSpeed;
        HPOrig = HP;
        gunDamage = startDamage;
        origHealthRegen = healthRegainSpeed;
        origShootRate = shootRate;
        currSprayAmmo = maxSprayAmmo;
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

        //calls the method to let the player select weapons 
        //selectGun();

        if (Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(Shoot());

        if(sprayWeaponActive && Input.GetButton("Shoot2") && !isShooting)
            StartCoroutine(Spray());
        if (sprayWeaponActive && (currSprayAmmo < maxSprayAmmo) && !isSpraying && !sprayRegen)
            StartCoroutine(RegenSprayAmmo());

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
                type3Routine = InvulnerableCooldown();
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
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
            isSprinting = false;
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
        if(bullet != null)
        {
            GameObject currBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
            currBullet.transform.forward = shootDir.normalized;
        }


        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

    IEnumerator Spray()
    {
        if (currSprayAmmo > 0)
        {
            isShooting = true;
            isSpraying = true;

            currSprayAmmo--;

            aud.PlayOneShot(audSpray, audSprayVol);

            // Find hit position with raycast
            Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point; // aims at specific point on ray at the distance of the hit
            else
                targetPoint = ray.GetPoint(15); // some distant point on ray if not aiming at anything

            // play spray effect on contact
            if (sprayEffect != null)
            {
                Instantiate(sprayEffect, targetPoint, Quaternion.identity);
            }

            Collider[] hits = Physics.OverlapSphere(targetPoint, 3);
            foreach (Collider c in hits)
            {
                ISpray sprayable = c.GetComponent<ISpray>();
                if (sprayable != null)
                    sprayable.kill();
            }

            yield return new WaitForSeconds(0.2f);
            isShooting = false;
            isSpraying = false;
        }
    }
    IEnumerator RegenSprayAmmo()
    {
        sprayRegen = true;
        yield return new WaitForSeconds(sprayRegenSpeed);

        currSprayAmmo++;

        sprayRegen = false;
    }

    // Power Up functions
    IEnumerator JumpPowerCooldown()
    {
        GameManager.instance.powerJumpActive.SetActive(true);
        GameManager.instance.jumpPowerCoolDown.CrossFadeAlpha(1, 1, false);
        GameManager.instance.jumpPowerImage.CrossFadeAlpha(1, 1, false);
        float CD = waitT;
        while (CD > 0)
        {
            //wait one second then add to counter
            yield return new WaitForSeconds(1);
            CD--;
            if (CD <= 1)
            {
                GameManager.instance.jumpPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.jumpPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.JumpPowerCoolDown(CD);
            //if the player got damaged while regen, exit regen state
        }

        GameManager.instance.powerJumpActive.SetActive(false);
        jumpMax = origJump;
        powerActive[0] = false;
    }
    IEnumerator SpeedPowerCooldown()
    {
        float CD = waitT;
        GameManager.instance.powerSpeedActive.SetActive(true);
        GameManager.instance.speedPowerCoolDown.CrossFadeAlpha(1, 1, false);
        GameManager.instance.speedPowerImage.CrossFadeAlpha(1, 1, false);
        while (CD > 0)
        {
            //wait one second then add to counter
            yield return new WaitForSeconds(1);
            CD--;
            if (CD <= 1)
            {
                GameManager.instance.speedPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.speedPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.SpeedPowerCoolDown(CD);
            
            //if the player got damaged while regen, exit regen state
        }

        GameManager.instance.powerSpeedActive.SetActive(false);
        
        playerSpeed = OrigSpeed;
        if (isSprinting)
        {
            playerSpeed = playerSpeed * sprintMod;
        }
        Debug.Log(playerSpeed);
        powerActive[1] = false;
    }

    IEnumerator InvulnerableCooldown()
    {
        float CD = waitT;
        GameManager.instance.powerHealthActive.SetActive(true);
        GameManager.instance.healthPowerCoolDown.CrossFadeAlpha(1, 1, false);
        GameManager.instance.healthPowerImage.CrossFadeAlpha(1, 1, false);
        while (CD > 0)
        {
            //wait one second then add to counter
            yield return new WaitForSeconds(1);
            CD--;
            if (CD <= 1)
            {
                GameManager.instance.healthPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.healthPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.HealthPowerCoolDown(CD);
            //if the player got damaged while regen, exit regen state
        }

        GameManager.instance.powerHealthActive.SetActive(false);
        isInvulnerable = false;
        powerActive[2] = false;
    }
    IEnumerator ShootRatePowerCooldown()
    {
        float CD = waitT;
        GameManager.instance.powerShootActive.SetActive(true);
        GameManager.instance.shootPowerCoolDown.CrossFadeAlpha(1, 1, false);
        GameManager.instance.shootPowerImage.CrossFadeAlpha(1, 1, false);
        while (CD > 0)
        {
            //wait one second then add to counter
            yield return new WaitForSeconds(1);
            CD--;
            if (CD <= 1)
            {
                GameManager.instance.shootPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.shootPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.ShootPowerCoolDown(CD);
            //if the player got damaged while regen, exit regen state
        }

        GameManager.instance.powerShootActive.SetActive(false);
        shootRate = origShootRate;
        powerActive[3] = false;
    }
    IEnumerator EnemyHpPowerCooldown()
    {
        float CD = waitT;
        GameManager.instance.powerDmgActive.SetActive(true);
        GameManager.instance.dmgPowerCoolDown.CrossFadeAlpha(1, 1, false);
        GameManager.instance.dmgPowerImage.CrossFadeAlpha(1, 1, false);
        while (CD > 0)
        {
            //wait one second then add to counter
            yield return new WaitForSeconds(1);
            CD--;
            if (CD <= 1)
            {
                GameManager.instance.dmgPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.dmgPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.DmgPowerCoolDown(CD);
            //if the player got damaged while regen, exit regen state
        }

        GameManager.instance.powerDmgActive.SetActive(false);
        for (int i = 0; i < enemyToFind.Length; i++)
        {
            if (enemyToFind[i] != null)
            {
                if (enemyToFind[i].GetComponent<EnemyAI>() != null)
                {

                    EnemyAI enemyScript = enemyToFind[i].GetComponent<EnemyAI>();

                    enemyScript.SetHP(origEnemyHp);
                }
                else if (enemyToFind[i].GetComponent<MeleeEnemyAI>() != null)
                {

                    MeleeEnemyAI enemyScript = enemyToFind[i].GetComponent<MeleeEnemyAI>();

                    enemyScript.SetHP(origEnemyHp);
                }
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
        if (isSprinting)
        {
            playerSpeed = speed * sprintMod;
        }
        powerActive[1] = true;
        powerIndex = 1;
    }
    public void Invulnerability()
    {
        isInvulnerable = true;
        powerActive[2] = true;
        powerIndex = 2;
    }
    public void ShootRate(float shoot)
    {
        shootRate = shoot;
        powerActive[3] = true;
        powerIndex = 3;
    }
    public void EnemyHealthDown(int damage)
    {
        enemyToFind = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyToFind.Length; i++)
        {
            if (enemyToFind[i] != null)
            {
                EnemyAI enemyScript;
                MeleeEnemyAI meleeEnemyScript;
                if (enemyToFind[i].GetComponent<EnemyAI>() != null)
                {
                    enemyScript = enemyToFind[i].GetComponent<EnemyAI>();
                    if (origEnemyHp == 0)
                    {
                        origEnemyHp = enemyScript.GetHp();
                    }

                    enemyScript.SetHP(damage);
                }
                else if(enemyToFind[i].GetComponent<MeleeEnemyAI>() != null)
                {
                    meleeEnemyScript = enemyToFind[i].GetComponent<MeleeEnemyAI>();
                    if (origEnemyHp == 0)
                    {
                        origEnemyHp = meleeEnemyScript.GetHp();
                    }
                    meleeEnemyScript.SetHP(damage);
                }
                
            }
        }
        powerActive[4] = true;
        powerIndex = 4;
    }

    // Pickup functions
    public void IncreasePlayerMaxHealth(int addition)
    {
        HPOrig += addition;
        HP += addition;
    }
    public void IncreasePlayerMaxSpeed(float addition)
    {
        OrigSpeed += addition;
        playerSpeed = OrigSpeed;
        if (isSprinting)
            playerSpeed = playerSpeed * sprintMod;
    }
    public void IncreasePlayerRegenSpeed(int subtraction)
    {
        healthRegainSpeed -= subtraction;
    }
    public void IncreasePlayerDamage(int addition)
    {
        gunDamage += addition;
        bullet.GetComponent<PlayerBullet>().damage = gunDamage;
    }

    public void takeDamage(int amount)
    {
        if (!isInvulnerable)
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
        playerSpeed = OrigSpeed;
        healthRegainSpeed = origHealthRegen;
        gunDamage = startDamage;
        bullet.GetComponent<PlayerBullet>().damage = gunDamage;
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


    //everything involving gun pickup shennanigans \/
    public void setGunStats(gunStats gun)
    {
        //gunList.Add(gun);
        //stats
        startDamage = gun.shootDamage;
        bullet = gun.bullet;
        bullet.GetComponent<PlayerBullet>().damage = gun.shootDamage;
        bullet.GetComponent<PlayerBullet>().setDestroyTime(gun.shootTime);
        bullet.GetComponent<PlayerBullet>().sethitEffect(gun.hitEffect);

        shootRate = gun.shootRate;
        //model
        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;



    }
    //void selectGun()
    //{
    //    if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
    //    {
    //        selectedGun++;
    //        changeGun();

    //    }
    //    else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
    //    {
    //        selectedGun--;
    //        changeGun();
    //    }
    //}

    //void changeGun()
    //{
    //    startDamage = gunList[selectedGun].shootDamage;
    //    shootRate = gunList[selectedGun].shootRate;
    //    bullet = gunList[selectedGun].bullet;
    //    bullet.GetComponent<PlayerBullet>().damage = gunList[selectedGun].shootDamage;
    //    bullet.GetComponent<PlayerBullet>().setDestroyTime(gunList[selectedGun].shootTime);
    //    bullet.GetComponent<PlayerBullet>().sethitEffect(gunList[selectedGun].hitEffect);




    //    //model
    //    gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
    //    gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
    //}
}
