using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //singleton
    public static GameManager instance;

    [Header("-----Player------")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPoint;
    [Header("-----Menu UI-----")]
    [SerializeField] GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject settingsMenu;
    [Header("-----Information Box-----")]
    [SerializeField] GameObject healthActive;

    [SerializeField] TMP_Text enemiesRemainText;
    [SerializeField] TMP_Text scoreCount;
    [SerializeField] TMP_Text currentWaveCount;
    [SerializeField] TMP_Text pDamageUpCounter;
    [SerializeField] TMP_Text pMaxHealthCounter;
    [SerializeField] TMP_Text pRegenCounter;
    [SerializeField] TMP_Text pSpeedCounter;

    public GameObject waveUIText;
    public TMP_Text WaveUIText;
    private int pDamage;
    private int pHealth;
    private int pRegen;
    private int pSpeed;
    [Header("-----Main Weapon UI-----")]
    [SerializeField] TMP_Text ammoCurr;
    [SerializeField] Image _MainWeaponbar; 
    [Header("-----Secondary Weapon UI-----")]
    public GameObject SprayAmmoParent;
    [SerializeField] Image SprayAmmoBar;
    [Header("-----PickUps UI-----")]
    public GameObject pickupLabel;
    public TMP_Text pickupText;
    [Header("-----PowerUps UI-----")]
    public GameObject powerJumpActive;
    public GameObject powerSpeedActive;
    public GameObject powerHealthActive;
    public GameObject powerShootActive;
    public GameObject powerDmgActive;
    public GameObject PowerText;
    public TMP_Text powerTypeText;
    public TMP_Text jumpPowerCoolDown;
    public Image jumpPowerImage;
    public TMP_Text speedPowerCoolDown;
    public Image speedPowerImage;
    public TMP_Text healthPowerCoolDown;
    public Image healthPowerImage;
    public TMP_Text shootPowerCoolDown;
    public Image shootPowerImage;
    public TMP_Text dmgPowerCoolDown;
    public Image dmgPowerImage;
    public Image playerHealthBar;
    public GameObject collectable;
    [Header("-----Quest Text-----")]
    [SerializeField] GameObject quest;
    public TMP_Text questText;
    [Header("-----Other-----")]
    public int maxWaves; // Set the maximum number of waves allowed in the level
    public int waves;
    public bool noEnemies;
    public bool isPaused;
    public int enemiesRemain;
    public int score;
    float origTimeScale;
    private float fillTime;
    [SerializeField] Image shoveBar;
    [SerializeField] Slider slider;
    void Awake()
    {
        //initiallize player variables;
        FadePowerups();
        healthActive.SetActive(true);
        origTimeScale = Time.timeScale;
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPoint = GameObject.FindWithTag("Player Spawn Point");
        waves = 1;
        enableWaveUIText();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(waves);

        //once esc is pressed and there arent any menus active pause the game
        if(Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            StatePaused();
            //make the active menu be the pause menu
            //activeMenu = pauseMenu;
            //activeMenu.SetActive(isPaused);
            setActive(pauseMenu);
        }
        if (Input.GetButtonDown("Tab")) { 
            
        }
    }
    private void FadePowerups()
    {
        jumpPowerCoolDown.CrossFadeAlpha(0, 1, false);
        jumpPowerImage.CrossFadeAlpha(0, 1, false);
        speedPowerCoolDown.CrossFadeAlpha(0, 1, false);
        speedPowerImage.CrossFadeAlpha(0, 1, false);
        healthPowerCoolDown.CrossFadeAlpha(0, 1, false);
        healthPowerImage.CrossFadeAlpha(0, 1, false);
        shootPowerCoolDown.CrossFadeAlpha(0, 1, false);
        shootPowerImage.CrossFadeAlpha(0, 1, false);
        dmgPowerCoolDown.CrossFadeAlpha(0, 1, false);
        dmgPowerImage.CrossFadeAlpha(0, 1, false);
    }
    public void StatePaused()
    {
        //flip bool
        isPaused = !isPaused;
        //slow game down to 0
        Time.timeScale = 0f;
        //make the cursor visible again
        Cursor.visible = true;
        //let the cursor move around the screen only
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpaused()
    {
        //flip our pause
        isPaused = !isPaused;
        //set the speed of the game to orig speed
        Time.timeScale = origTimeScale;
        //make cursor invis
        Cursor.visible = false;
        //lock the cursor to the center
        Cursor.lockState = CursorLockMode.Locked;
        //reset our active menu
        activeMenu.SetActive(false);
        activeMenu = null;
    }
    public void SetSettings()
    {
        activeMenu.SetActive(false);
        setActive(settingsMenu);
    }
    public void SaveSettings()
    {
        activeMenu.SetActive(false);
        setActive(pauseMenu);
    }

    public void UpdateWinCondition(int amount)
    {
        //add a counter to the enemies 
        enemiesRemain += amount;
        enemiesRemainText.text = enemiesRemain.ToString("0");
        //when there are no enemies or waves remaining pull the win menu up
        if(enemiesRemain < 1 && waves == maxWaves)
        {
            StartCoroutine(winGame());
        }
        
    }
    public IEnumerator winGame()
    {
        noEnemies = true;

        yield return new WaitForSeconds(3);

        StatePaused();
        setActive(winMenu);
    }

    public void IncreasePlayerScore(int num)
    {
        score += num;
        scoreCount.text = score.ToString("0");
    }
    public void IncreaseWaveCount(int num)
    {
        if (num <= maxWaves)
        {
            waves = num;
            currentWaveCount.text = waves.ToString("0");
            enableWaveUIText();
        }
    }
    public void IncreaseDamagePickUpCounter()
    {
        pDamage++;
        pDamageUpCounter.text = pDamage.ToString("0");
    }
    public void IncreaseHealthPickUpCounter()
    {
        pHealth++;
        pMaxHealthCounter.text = pHealth.ToString("0");
    }
    public void IncreaseRegenPickUpCounter()
    {
        pRegen++;
        pRegenCounter.text = pRegen.ToString("0");
    }
    public void IncreaseSpeedPickUpCounter()
    {
        pSpeed++;
        pSpeedCounter.text = pSpeed.ToString("0");
    }
    public void JumpPowerCoolDown(float coolDown)
    {
        jumpPowerCoolDown.text = coolDown.ToString("0");
    }
    public void SpeedPowerCoolDown(float coolDown)
    {
        speedPowerCoolDown.text = coolDown.ToString("0");
    }
    public void HealthPowerCoolDown(float coolDown)
    {
        healthPowerCoolDown.text = coolDown.ToString("0");
    }
    public void ShootPowerCoolDown(float coolDown)
    {
        shootPowerCoolDown.text = coolDown.ToString("0");
    }
    public void DmgPowerCoolDown(float coolDown)
    {
        dmgPowerCoolDown.text = coolDown.ToString("0");
    }
    public void SetPowerText(string powerType)
    {
        powerTypeText.text = powerType;
    }

    public void GameOver()
    {
        //pause the menu
        StatePaused();
        //set the active menu
        setActive(loseMenu);
    }

    //set the paramater menu to be the active menu
    void setActive(GameObject setActive)
    {
        activeMenu = setActive;
        activeMenu.SetActive(true);
    }

    public void updateSprayAmmoUI(int current, int max)
    {
        float ammoVal = current / (max * 1.0f);
        float amount = (ammoVal / .5f) * 180f / 360;
        SprayAmmoBar.fillAmount = amount;
    }
    public void updateShoveUI( float shoveCooldown)
    {
        
        slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, fillTime );
        fillTime += 0.5f * Time.deltaTime;
    }
    public void ResetShoveUI()
    {
        //Debug.Log("Slider reset");
        //Debug.Log(slider.minValue);
        fillTime = 0;
        slider.value = slider.minValue;

    }

    public void updateAmmmo(int _curr, int _ammoMax)
    {
        ammoCurr.text = _curr.ToString("F0");
        float ammoVal = _curr / (_ammoMax * 1.0f);
       //Debug.Log("ammo curr:" + _curr);
       //Debug.Log("ammo max:" + _ammoMax);
       //Debug.Log("ammo value:" + ammoVal);
        float amount = (ammoVal / .5f) * 180f / 360;
        _MainWeaponbar.fillAmount = amount;

    }
    public void enableCollectable(bool enable)
    {
        collectable.SetActive(enable);
    }
    // Next two methids control the Giant Text for each new wave
    public void enableWaveUIText()
    {
        StartCoroutine(WaveUISpawnRoutine());
    }

    IEnumerator WaveUISpawnRoutine()
    {
        WaveUIText.text = "Wave " + (waves);
        waveUIText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        waveUIText.gameObject.SetActive(false);
    }
}
