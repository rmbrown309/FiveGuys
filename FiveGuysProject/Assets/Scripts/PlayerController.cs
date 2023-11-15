using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamage, IPower
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform shovePos;
    [SerializeField] Transform endOfBarrel;
    [SerializeField] CameraController cam;
    
    [Header("----- Player Stats -----")]
    [Range(1, 15)] [SerializeField] float HP;
    [Range(1, 10)] [SerializeField] float playerSpeed;
    [Range(1, 3)] [SerializeField] float sprintMod; // amount playerSpeed is multiplied when sprinting
    [Range(1, 3)] [SerializeField] int jumpMax; // number of jumps player can perform before landing
    [Range(8, 30)] [SerializeField] float jumpHeight;
    [Range(-10, -40)] [SerializeField] float gravityValue;
    public int jumpedTimes;
    //[SerializeField] SavedPickUps savedPickUps;
    [SerializeField] float healthRegainSpeed;
    [SerializeField] int collectedItems;
    [Header("----- Gun Stats -----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>(); // the amount of guns currently on the player.
    [SerializeField] gunStats defGun; // the amount of guns currently on the player.
    [SerializeField] gunStats[] SuperGuns;
    [SerializeField] GameObject gunModel; // the model of the players curr gun
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float startDamage;
    [SerializeField] ParticleSystem muzzleFlash;

    int weaponID;
    public int ammoID { get; set; }
    [Header("----- Grenade Stats -----")]
    [SerializeField] GameObject grenade;
    [SerializeField] float tossRate;


    [Header("----- Rat Spray Stats -----")]
    [SerializeField] ParticleSystem sprayEffect;
    [SerializeField] int sprayDistance;
    [SerializeField] int maxSprayAmmo;
    public int currSprayAmmo;
    [SerializeField] float sprayRegenSpeed;
    [SerializeField] bool sprayWeaponActive;

    [Header("----- Shove Stats -----")]
    [SerializeField] GameObject shoveHandModel;
    [SerializeField] ParticleSystem shoveEffect;
    [SerializeField] float shoveCooldown;
    [SerializeField] int shoveForce;

    [Header("----- PowerUp Settings -----")]
    [SerializeField] float waitT;
    IEnumerator[] powerUpCorutine;

    [Header("----- Audio Stuff -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audSpray;
    [Range(0, 1)] [SerializeField] float audSprayVol;
    [SerializeField] AudioClip[] audDamage;
    [Range(0, 1)] [SerializeField] float audDamageVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)] [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)] [SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audShove;
    [Range(0, 1)][SerializeField] float audShoveVol;
    [SerializeField] AudioClip[] audDeath;
    [Range(0, 1)][SerializeField] float audDeathVol;
    [SerializeField] AudioClip[] noAmmo;
    [Range(0, 1)] [SerializeField] float noAmmoVol;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private float gunDamage;
    private float extraDamage;

    bool isShooting;
    bool isSpraying;
    public bool isShoving;
    bool sprayRegen;
    bool isSprinting;
    bool footstepsPlaying;
    int selectedGun; // the int that controls how the player selects their gun
    bool isTossing;

    // powerup variables
    GameObject[] enemyToFind;
    private int origJump;
    private bool isInvulnerable;
    private float origHealthRegen;
    private float origShootRate;
    private float origEnemyHp;
    private float HPOrig;
    private float OrigSpeed;

    //Regen detectors
    bool isDamaged;
    bool damageActive;
    //Power Up
    [SerializeField]bool canMove;
    bool isPowered;
    int powerType;
    //shoot shennanigans
    Transform GunShootPos;
    ParticleSystem MuzzleFlashPos;
    Vector3 pos;
    Vector3 fungle;
    float origShotvol;
    Vector3 origWeaponPos;
    Quaternion origWeaponRotation;
    //Shove

    private void Start()
    {
        //GameManager.instance.StateUnpaused();

        GameManager.instance.numberOfLives = 3;
        canMove = true;
        powerUpCorutine = new IEnumerator[5];
        origJump = jumpMax;
        OrigSpeed = playerSpeed;
        HPOrig = HP;
        gunDamage = startDamage;
        extraDamage = 0;
        setGunStats(defGun);
        origShotvol = 1;
        origHealthRegen = healthRegainSpeed;
        origShootRate = shootRate;
        currSprayAmmo = maxSprayAmmo;
        gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
        origWeaponPos = new Vector3(gunModel.transform.localPosition.x, gunModel.transform.localPosition.y, gunModel.transform.localPosition.z);
        origWeaponRotation = new Quaternion(gunModel.transform.localRotation.x, gunModel.transform.localRotation.y, gunModel.transform.localRotation.z, gunModel.transform.localRotation.w);
        GameManager.instance.updateAmmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
        GameManager.instance.updateSprayAmmoUI(currSprayAmmo, maxSprayAmmo);
        if (GameManager.instance.playerSpawnPoint != null)
        {
            SpawnPlayer();
        }
        //if(SceneManager.GetActiveScene().buildIndex == 3)
        //{
        //
        //    setGunStats(savedPickUps.gun);
        //}
        //if (GameManager.instance.enemiesRemain == 0)
        //    GameManager.instance.noEnemies = true;
    }

    void Update()
    {
        Movement();

        if (Input.GetButton("Shoot") && !isShooting && gunList.Count > 0 && !GameManager.instance.isPaused)//if it is a shotgun then fire multiple times
        {
            //by looping the shoot multiple time we instantiate the correct number of pellets to be shot because bullets are only instantiated when shoot is called.
            for (int i = 0; i < gunList[selectedGun].numOfPellets; i++)
            {
                if (gunList[selectedGun].isM16)
                {
                    StartCoroutine(Burst());
                }
                else
                {
                    if (!gunList[selectedGun].isM16)
                    {
                        StartCoroutine(Shoot());

                    }
                }
            }
            if (gunList[selectedGun].ammoCur != 0 && gunList[selectedGun].isShotgun)
            {
                gunList[selectedGun].ammoCur--;
                GameManager.instance.updateAmmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
            }
            if (gunList[selectedGun].ammoCur == 0 && !isShooting)
            {
                StartCoroutine(noAmmoSound());
            }

        }

        if (sprayWeaponActive && Input.GetButton("Shoot2") && !isShooting && !GameManager.instance.isPaused)
            StartCoroutine(Spray());
        if (Input.GetButton("Grenade") && !isTossing && !GameManager.instance.isPaused)
        {
            StartCoroutine(TossGrenade());
        }
        if (sprayWeaponActive && (currSprayAmmo < maxSprayAmmo) && !isSpraying && !sprayRegen)
            StartCoroutine(RegenSprayAmmo());
        if (isShoving)
        {
            GameManager.instance.updateShoveUI(shoveCooldown);
        }
        else
        {
            GameManager.instance.ResetShoveUI();
        }

        if (Input.GetButton("Shove") && !isShoving && !GameManager.instance.isPaused)
            StartCoroutine(Shove());

        //if player got damaged AND there isnt an active regen happening
        //then regen health
        if (isDamaged && !damageActive)
        {
            StartCoroutine(RegainHelth());
        }
    }

    void Movement()
    {
        Sprint();



        // Keeps player velocity from going negative and resets ability to jump while grounded
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && move.normalized.magnitude > 0.3f && !footstepsPlaying)
        {
            StartCoroutine(playFootsteps());
        }
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        // wasd movement input
        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;
        if (canMove)
        {
            controller.Move(move * Time.deltaTime * playerSpeed);
        }


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax && !GameManager.instance.isPaused)
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);

        }

        // makes gravity work on player
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    public void SetMoveBool(Vector3 position)
    {
        canMove = false;
        gameObject.transform.SetPositionAndRotation(position, gameObject.transform.rotation);
    }
    IEnumerator playFootsteps()
    {
        footstepsPlaying = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (!isSprinting)
        {
            yield return new WaitForSeconds(0.4f);

        }
        else
            yield return new WaitForSeconds(0.25f);

        footstepsPlaying = false;
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
        if (gunList[selectedGun].ammoCur > 0 && gunList.Count > 0)
        {
            isShooting = true;

            //plays gunshot audio and ticks the ammo down for the players current gun
            aud.pitch = Random.Range(0.95f, 1.05f);
            aud.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].audShotVol);

            if (!gunList[selectedGun].isShotgun && !gunList[selectedGun].isM16)
            {
                gunList[selectedGun].ammoCur--;
                GameManager.instance.updateAmmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
            }

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

            if (gunList[selectedGun].isShotgun)
            {
                shootDir.x = shootDir.x + Random.Range(-gunList[selectedGun].horizSpread, +gunList[selectedGun].horizSpread);
                shootDir.y = shootDir.y + Random.Range(-gunList[selectedGun].vertSpread, +gunList[selectedGun].vertSpread);
                shootDir.z = shootDir.z + Random.Range(-gunList[selectedGun].zSpread, +gunList[selectedGun].zSpread);
            }

           
            // Instantiates bullet object and redirects its rotation toward the shootDir
            if (bullet != null)
            {
                MuzzleFlashPos.Play();
                GameObject currBullet = Instantiate(bullet, GunShootPos.transform.position, Quaternion.identity);
                currBullet.transform.forward = shootDir.normalized;
             
            }
     

            Quaternion newRotation = new Quaternion(origWeaponRotation.x - 0.025f, origWeaponRotation.y, origWeaponRotation.z, origWeaponRotation.w);
            //recoil by moving z
            Vector3 newWeaponPos = new Vector3(origWeaponPos.x, origWeaponPos.y, origWeaponPos.z - 0.05f);

            gunModel.transform.localRotation = newRotation;
            gunModel.transform.localPosition = newWeaponPos;
            yield return new WaitForSeconds(shootRate / 4);
            if (gunList[selectedGun].weaponID == 3)
            {

                aud.pitch = Random.Range(0.95f, 1.05f);
                aud.PlayOneShot(gunList[selectedGun].reloadSound, gunList[selectedGun].audReloadVol);
            }
            gunModel.transform.localPosition = origWeaponPos;
            gunModel.transform.localRotation = origWeaponRotation;
            yield return new WaitForSeconds(shootRate * 0.75f);

            if (gunList[selectedGun].weaponID == 9)
                yield return new WaitForSeconds(0.001f);

            isShooting = false;

            if (gunList[selectedGun].isPowerWeapon && gunList[selectedGun].ammoCur == 0)
            {
                while (gunList[selectedGun].isPowerWeapon)
                    selectedGun--;
                setGunStats(gunList[selectedGun]);
            }
        }
     
    }

    IEnumerator Spray()
    {
        if (currSprayAmmo > 0)
        {
            isShooting = true;
            isSpraying = true;

            currSprayAmmo--;
            GameManager.instance.updateSprayAmmoUI(currSprayAmmo, maxSprayAmmo);

            aud.PlayOneShot(audSpray, audSprayVol);

            // Find hit position with raycast
            Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, sprayDistance))
                targetPoint = hit.point; // aims at specific point on ray at the distance of the hit
            else
                targetPoint = ray.GetPoint(sprayDistance); // some distant point on ray if not aiming at anything

            // play spray effect on contact
            if (sprayEffect != null)
            {
                Instantiate(sprayEffect, targetPoint, Quaternion.identity);
            }

            Collider[] hits = Physics.OverlapSphere(targetPoint, 4);
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
        GameManager.instance.updateSprayAmmoUI(currSprayAmmo, maxSprayAmmo);

        sprayRegen = false;
    }

    IEnumerator Shove()
    {
        isShoving = true;
        shoveHandModel.SetActive(true);
        aud.PlayOneShot(audShove[Random.Range(0, audShove.Length)], audShoveVol);
        GameManager.instance.ResetShoveUI();
        Collider[] hits = Physics.OverlapSphere(shovePos.position, 3);
        foreach (Collider c in hits)
        {
            IPhysics shoveable = c.GetComponent<IPhysics>();
            if (shoveable != null)
                shoveable.TakePhysics(transform.forward * shoveForce);
        }
        StartCoroutine(cam.ShakeCam(0.2f, 0.05f));
        shoveEffect.Play();
        yield return new WaitForSeconds(shoveCooldown * 0.1f);
        shoveHandModel.SetActive(false);
        yield return new WaitForSeconds(shoveCooldown * 0.9f);
        isShoving = false;
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
                GameManager.instance.jumpPower = false;
                GameManager.instance.jumpPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.jumpPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.JumpPowerCoolDown(CD);
            //if the player got damaged while regen, exit regen state
        }

       // GameManager.instance.powerJumpActive.SetActive(false);
        jumpMax = origJump;
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
                GameManager.instance.speedPower = false;
                GameManager.instance.speedPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.speedPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.SpeedPowerCoolDown(CD);

            //if the player got damaged while regen, exit regen state
        }

       // GameManager.instance.powerSpeedActive.SetActive(false);

        playerSpeed = OrigSpeed;
        if (isSprinting)
        {
            playerSpeed = playerSpeed * sprintMod;
        }
        Debug.Log(playerSpeed);
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
                GameManager.instance.invulnerablePower = false;
                GameManager.instance.healthPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.healthPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.HealthPowerCoolDown(CD);
            //if the player got damaged while regen, exit regen state
        }

        //GameManager.instance.powerHealthActive.SetActive(false);
        isInvulnerable = false;
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
                GameManager.instance.fireSpeedPower = false;
                GameManager.instance.shootPowerCoolDown.CrossFadeAlpha(0, 1, false);
                GameManager.instance.shootPowerImage.CrossFadeAlpha(0, 1, false);
            }
            GameManager.instance.ShootPowerCoolDown(CD);
            //if the player got damaged while regen, exit regen state
        }

       //GameManager.instance.powerShootActive.SetActive(false);
        shootRate = gunList[selectedGun].shootRate;
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
    }
    
    IEnumerator PowerTextCD(string typeCD)
    {
        GameManager.instance.SetPowerText(typeCD);
        GameManager.instance.powerText = true;
        GameManager.instance.PowerText.SetActive(true);
        //Debug.Log(true);
        yield return new WaitForSeconds(2);
        //Debug.Log(typeCD);

        //GameManager.instance.powerText = false;
        GameManager.instance.PowerText.SetActive(false);
        //Debug.Log(false);
    }
    public void JumpPower(int jumps)
    {
        if (powerUpCorutine[0] != null)
        {
            StopCoroutine(powerUpCorutine[0]);
            StopCoroutine(PowerTextCD("Double Jump"));
        }
        GameManager.instance.jumpPower = true;
        jumpMax = jumps;
        StartCoroutine(PowerTextCD("Double Jump"));
        powerUpCorutine[0] = JumpPowerCooldown();
        StartCoroutine(powerUpCorutine[0]);
    }
    public void SpeedBoost(float speed)
    {
        if (powerUpCorutine[1] != null)
        {
            StopCoroutine(powerUpCorutine[1]);
            StopCoroutine(PowerTextCD("Speed Boost"));
        }
        playerSpeed = speed;
        if (isSprinting)
        {
            playerSpeed = speed * sprintMod;
        }
        GameManager.instance.speedPower = true;
        StartCoroutine(PowerTextCD("Speed Boost"));
        powerUpCorutine[1] = SpeedPowerCooldown();
        StartCoroutine(powerUpCorutine[1]);
    }
    public void SpeedReduction(float speed)
    {
        if (powerUpCorutine[1] != null)
        {
            StopCoroutine(powerUpCorutine[1]);
            StopCoroutine(PowerTextCD("Speed Reduced"));
        }
        playerSpeed = speed;
        if (isSprinting)
        {
            playerSpeed = speed * sprintMod;
        }

        StartCoroutine(PowerTextCD("Speed Reduced"));
        powerUpCorutine[1] = SpeedPowerCooldown();
        StartCoroutine(powerUpCorutine[1]);
    }
    public void Invulnerability()
    {
        if (powerUpCorutine[2] != null)
        {
            StopCoroutine(powerUpCorutine[2]);
            StopCoroutine(PowerTextCD("Invulnerable"));
        }
        isInvulnerable = true;
        GameManager.instance.invulnerablePower = true;
        StartCoroutine(PowerTextCD("Invulnerable"));
        powerUpCorutine[2] = InvulnerableCooldown();
        StartCoroutine(powerUpCorutine[2]);
    }
    public void ShootRate(float shoot)
    {
        if (powerUpCorutine[3] != null)
        {
            StopCoroutine(powerUpCorutine[3]);
            StopCoroutine(PowerTextCD("Rapid Fire"));
        }
        GameManager.instance.fireSpeedPower = true;
        shootRate /= shoot;
        StartCoroutine(PowerTextCD("Rapid Fire"));
        powerUpCorutine[3] = ShootRatePowerCooldown();
        StartCoroutine(powerUpCorutine[3]);
    }

    public void EnemyHealthDown(int damage)
    {
        if (powerUpCorutine[4] != null)
        {
            StopCoroutine(powerUpCorutine[4]);
            StopCoroutine(PowerTextCD("Enemy Health Down"));
        }
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
                else if (enemyToFind[i].GetComponent<MeleeEnemyAI>() != null)
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
        //powerActive[4] = true;
        //powerIndex = 4;
        StartCoroutine(PowerTextCD("Enemy Health Down"));
        powerUpCorutine[4] = EnemyHpPowerCooldown();
        StartCoroutine(powerUpCorutine[4]);
    }
    public void AmmoRefillPower()
    {
       
            gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
            GameManager.instance.updateAmmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
            StartCoroutine(PowerTextCD("Ammo Refill"));
        

    }
    public void SuperWeapons()
    {
        StartCoroutine(PowerTextCD("Super Weapon"));
        int rand = Random.Range(0, 3);
        Debug.Log(rand);
        setGunStats(SuperGuns[rand]);
        gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
        GameManager.instance.updateAmmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
    }
    // Pickup functions
    public void IncreasePlayerMaxHealth(int addition)
    {
        GameManager.instance.IncreaseHealthPickUpCounter();
        HPOrig += addition;
        HP += addition;
    }
    public void IncreasePlayerMaxSpeed(float addition)
    {
        GameManager.instance.IncreaseSpeedPickUpCounter();
        OrigSpeed += addition;
        playerSpeed = OrigSpeed;
        if (isSprinting)
            playerSpeed = playerSpeed * sprintMod;
    }
    public void IncreasePlayerRegenSpeed(int subtraction)
    {
        GameManager.instance.IncreaseRegenPickUpCounter();
        healthRegainSpeed -= subtraction;
    }
    public void IncreasePlayerDamage(int addition)
    {
        GameManager.instance.IncreaseDamagePickUpCounter();
        extraDamage += addition;
        bullet.GetComponent<PlayerBullet>().damage = gunList[selectedGun].shootDamage + extraDamage;
    }
    public void RefillAmmo()
    {
        if (gunList[selectedGun].isPowerWeapon == false)
        {
            gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
            GameManager.instance.updateAmmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
        }
    }
    public void GetRatKiller()
    {
        sprayWeaponActive = true;
        GameManager.instance.SprayAmmoParent.SetActive(true);
    }

    public void takeDamage(float amount)
    {
        if (!isInvulnerable)
        {
            aud.pitch = Random.Range(0.95f, 1.05f);
            HP -= amount;
            isDamaged = true;
            UpdatePlayerUI();

            // Future Implementation for when a game over menu needs to appear
            if (HP < 1)
            {
                aud.PlayOneShot(audDeath[Random.Range(0, audDeath.Length)], audDeathVol);
                GameManager.instance.GameOver();
            }
            else
            {
                StartCoroutine(cam.ShakeCam(0.2f, 0.1f));
                aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);
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

        gunList.Add(gun);
        //stats
        startDamage = gun.shootDamage;
        bullet = gun.bullet;
        if(bullet.GetComponent<PlayerBullet>() != null)
        {
            bullet.GetComponent<PlayerBullet>().damage = gun.shootDamage + extraDamage;
            bullet.GetComponent<PlayerBullet>().setDestroyTime(gun.shootTime);
            bullet.GetComponent<PlayerBullet>().sethitEffect(gun.hitEffect);

        }
        ammoID = gun.ammoID;
        weaponID = gun.weaponID;
        shootRate = gun.shootRate;
        //model
        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
        selectedGun = gunList.Count - 1;
        if (!gunList[selectedGun].isPowerWeapon && gunList.Count > 1)
        {
            if (!gunList[selectedGun - 1].isPowerWeapon)
                GameManager.instance.updateAmmmo(gunList[selectedGun].ammoMax, gunList[selectedGun].ammoMax);
        }
        SetGunOffset();
        if (gunList[selectedGun].numOfPellets == 1 && !gunList[selectedGun].isM16) //handles every other weapon
        {
            if (gunList[selectedGun].weaponID == 8 && gunList[selectedGun].weaponID == 7) //handles grenade thump
                gunList[selectedGun].audShotVol = noAmmoVol / 2;
            if (gunList[selectedGun].weaponID == 1) //handles subgun
                gunList[selectedGun].audShotVol = noAmmoVol / 4;
            if (gunList[selectedGun].weaponID == 2) //handles sniper
                gunList[selectedGun].audShotVol = noAmmoVol / 2;
            else //borger
                gunList[selectedGun].audShotVol = origShotvol * noAmmoVol;
        }

        if (gunList[selectedGun].weaponID == 5)// handles the m16
            gunList[selectedGun].audShotVol = noAmmoVol / 1.25f;

        if (gunList[selectedGun].weaponID == 4) //handles double barrel being egregiously loud
            gunList[selectedGun].audShotVol = noAmmoVol / 20;

        if (gunList[selectedGun].weaponID == 9) //handles minigun being egregiously loud
            gunList[selectedGun].audShotVol = noAmmoVol / 20;

        if (gunList[selectedGun].numOfPellets > 1 && gunList[selectedGun].weaponID != 4 && gunList[selectedGun].weaponID != 9) //handles every other shotgun
        {
            gunList[selectedGun].audShotVol = (noAmmoVol / 5);
            gunList[selectedGun].audReloadVol = (noAmmoVol / 10);

        }
        //picked = true;


    }
    public int GetGunID()
    {
        return weaponID;
    }
    

    IEnumerator Burst()
    {
        if (gunList[selectedGun].ammoCur > 0 && gunList.Count > 0)
        {
            isShooting = true;
            Quaternion newRotation = new Quaternion(origWeaponRotation.x, origWeaponRotation.y, origWeaponRotation.z, origWeaponRotation.w); ;
            Vector3 newWeaponPos = new Vector3(origWeaponPos.x, origWeaponPos.y, origWeaponPos.z - 0.05f);

            //plays gunshot audio and ticks the ammo down for the players current gun
            aud.pitch = Random.Range(0.95f, 1.05f);

            aud.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].audShotVol);

            if (!gunList[selectedGun].isShotgun && !gunList[selectedGun].isM16)
            {
                gunList[selectedGun].ammoCur--;
                GameManager.instance.updateAmmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
            }

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

            if (gunList[selectedGun].isShotgun)
            {
                shootDir.x = shootDir.x + Random.Range(-gunList[selectedGun].horizSpread, +gunList[selectedGun].horizSpread);
                shootDir.y = shootDir.y + Random.Range(-gunList[selectedGun].vertSpread, +gunList[selectedGun].vertSpread);
                shootDir.z = shootDir.z + Random.Range(-gunList[selectedGun].zSpread, +gunList[selectedGun].zSpread);
            }


            // Instantiates bullet object and redirects its rotation toward the shootDir
            if (bullet != null)
            {

                if (gunList[selectedGun].isM16 && gunList[selectedGun].weaponID != 4)
                {
                    newRotation.x = origWeaponRotation.x - 0.01f;
                    MuzzleFlashPos.Play();
                    GameObject currBullet = Instantiate(bullet, GunShootPos.transform.position, Quaternion.identity);
                    currBullet.transform.forward = shootDir.normalized;
                    gunModel.transform.localRotation = newRotation;
                    gunModel.transform.localPosition = newWeaponPos;
                    yield return new WaitForSeconds(0.08f);
                    gunModel.transform.localPosition = origWeaponPos;
                    gunModel.transform.localRotation = origWeaponRotation;

                    MuzzleFlashPos.Play();
                    GameObject nextBullet = Instantiate(bullet, GunShootPos.transform.position, Quaternion.identity);
                    nextBullet.transform.forward = shootDir.normalized;
                    yield return new WaitForSeconds(0.08f);
                    MuzzleFlashPos.Play();

                    GameObject lastBullet = Instantiate(bullet, GunShootPos.transform.position, Quaternion.identity);
                    lastBullet.transform.forward = shootDir.normalized;
                    yield return new WaitForSeconds(0.08f);

                }

                if (gunList[selectedGun].weaponID == 4 )
                {
                    newRotation.x = origWeaponRotation.x - 0.05f;
                    MuzzleFlashPos.Play();
                    GameObject currBullet = Instantiate(bullet, GunShootPos.transform.position, Quaternion.identity);
                    currBullet.transform.forward = shootDir.normalized;
                    gunModel.transform.localRotation = newRotation;
                    gunModel.transform.localPosition = newWeaponPos;
                    yield return new WaitForSeconds(0.2f);
                    aud.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].audShotVol);
                    MuzzleFlashPos.Play();
                    gunModel.transform.localPosition = origWeaponPos;
                    gunModel.transform.localRotation = origWeaponRotation;
                    GameObject nextBullet = Instantiate(bullet, GunShootPos.transform.position, Quaternion.identity);
                    nextBullet.transform.forward = shootDir.normalized;
                    yield return new WaitForSeconds(0.2f);



                }


            }

            gunModel.transform.localRotation = newRotation;
            gunModel.transform.localPosition = newWeaponPos;
            yield return new WaitForSeconds(shootRate / 4);

            gunModel.transform.localPosition = origWeaponPos;
            gunModel.transform.localRotation = origWeaponRotation;
            yield return new WaitForSeconds(shootRate * 0.75f); isShooting = false;
        }
    }

    IEnumerator TossGrenade()
    {
        isTossing = true;
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point; // aims at specific point on ray at the distance of the hit
        else
            targetPoint = ray.GetPoint(50); // some distant point on ray if not aiming at anything

        // Calculate shooting direction
        Vector3 shootDir = targetPoint - shootPos.position;
        GameObject currBullet = Instantiate(grenade, shootPos.transform.position, Quaternion.identity);
        currBullet.transform.forward = shootDir.normalized;


        yield return new WaitForSeconds(tossRate);
        isTossing = false;

    }

    IEnumerator noAmmoSound()
    {
        isShooting = true;
        aud.pitch = 1;
        aud.PlayOneShot(noAmmo[Random.Range(0, noAmmo.Length)], noAmmoVol);
        yield return new WaitForSeconds(0.25f);
        isShooting = false;


    }


    public void SetAudio(float newVolume)
    {
        audDamageVol = newVolume;
        audJumpVol = newVolume;
        audSprayVol = newVolume/2;
        audStepsVol = newVolume/4;
        audShoveVol = newVolume * 2;
        audDeathVol = newVolume * 3;
        if( gunList.Count > 0)
        {
            if (gunList[selectedGun].numOfPellets == 1 && !gunList[selectedGun].isM16) //handles every other weapon
            {
                if (gunList[selectedGun].weaponID == 8 && gunList[selectedGun].weaponID == 7) //handles grenade thump
                    gunList[selectedGun].audShotVol = newVolume / 2;
                if (gunList[selectedGun].weaponID == 1) //handles subgun
                    gunList[selectedGun].audShotVol = newVolume / 4;
                if (gunList[selectedGun].weaponID == 2) //handles sniper
                    gunList[selectedGun].audShotVol = newVolume / 2;
                else //borger
                    gunList[selectedGun].audShotVol = origShotvol * newVolume;
            }

            if (gunList[selectedGun].weaponID == 5)// handles the m16
                gunList[selectedGun].audShotVol = newVolume / 1.25f;

            if (gunList[selectedGun].weaponID == 4) //handles double barrel being egregiously loud
                gunList[selectedGun].audShotVol = newVolume / 20;

            if (gunList[selectedGun].weaponID == 9) //handles minigun being egregiously loud
                gunList[selectedGun].audShotVol = newVolume / 20;

            if (gunList[selectedGun].numOfPellets > 1 && gunList[selectedGun].weaponID != 4 && gunList[selectedGun].weaponID != 9) //handles every other shotgun
            {
                gunList[selectedGun].audShotVol = (newVolume / 5);
                gunList[selectedGun].audReloadVol = (newVolume / 10);

            }
        }
        noAmmoVol = newVolume;
    }
    public void SetCollectables(int item)
    {
        collectedItems += item;
    }
    public int GetCollectables()
    {
        return collectedItems;
    }

    public void SetGunOffset()
    {
        if(gunList.Count < 2)
        {
            pos = new Vector3(shootPos.transform.position.x, shootPos.transform.position.y, shootPos.transform.position.z);
            pos.x += gunList[selectedGun].xOffset;
            pos.y += gunList[selectedGun].yOffset;
            pos.z += gunList[selectedGun].zOffset;
            GunShootPos = Instantiate(endOfBarrel, pos, shootPos.rotation, Camera.main.transform);
            MuzzleFlashPos = Instantiate(muzzleFlash, pos, shootPos.rotation, Camera.main.transform);
            fungle = new Vector3(GunShootPos.transform.localPosition.x, GunShootPos.transform.localPosition.y, GunShootPos.transform.localPosition.z);

        }


        fungle.x += gunList[selectedGun].xOffset;
        fungle.y += gunList[selectedGun].yOffset;
        fungle.z += gunList[selectedGun].zOffset;

        GunShootPos.transform.localPosition = fungle;
        MuzzleFlashPos.transform.localPosition = fungle;

        fungle.x -= gunList[selectedGun].xOffset;
        fungle.y -= gunList[selectedGun].yOffset;
        fungle.z -= gunList[selectedGun].zOffset;


        //if (gunList.Count >= 2)
        //{
        //    pos.x -= gunList[selectedGun].xOffset;
        //    pos.y -= gunList[selectedGun].yOffset;
        //    pos.z -= gunList[selectedGun].zOffset;
        //}




        ////shootPos.transform.position =

        //shootPos.transform.position = pos;
        //shootPos.rotation = Quaternion.identity;


        //shootPos.transform.position.z;

    }
}
