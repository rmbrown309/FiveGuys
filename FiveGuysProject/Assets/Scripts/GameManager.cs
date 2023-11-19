using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    //singleton
    public static GameManager instance;

    [Header("-----Player------")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPoint;
    public int numberOfLives { get; set; }
    [Header("-----Menu UI-----")]

    [SerializeField] GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject nextLevelMenu;
    [SerializeField] GameObject helpMenu;
    [SerializeField] GameObject objectiveMenu;
    [SerializeField] GameObject controlsMenu;
    //agree menu settings
    [SerializeField] GameObject agreeMenu;
    [SerializeField] GameObject agreeMenuTittle;

    //agere quit loose menu
    [SerializeField] GameObject agreeMenuLoose;
    [SerializeField] GameObject agreeMenuRespawnLoose;
    //button
    [SerializeField] GameObject[] quitButtons;
    [SerializeField] GameObject findButtonMenu;
    [SerializeField] GameObject loadingMenu;

    [SerializeField] TMP_Text objectiveText;
    [SerializeField] TMP_Text findButtonText;

    [SerializeField] string levelOneObjective;
    [SerializeField] string levelTwoObjective;
    [SerializeField] string levelThreeObjective;

    [Header("-----Information Box-----")]
    [SerializeField] GameObject healthActive;

    [SerializeField] TMP_Text enemiesRemainText;
    [SerializeField] TMP_Text scoreCount;
    [SerializeField] TMP_Text currentWaveCount;
    [SerializeField] TMP_Text pDamageUpCounter;
    [SerializeField] GameObject pMaxHealthOverlay;
    [SerializeField] GameObject pRegenOverlay;
    [SerializeField] GameObject pSpeedOverlay;


    public GameObject waveUIText;
    public TMP_Text WaveUIText;
    private int pDamage;
    [Header("-----Main Weapon UI-----")]
    [SerializeField] TMP_Text ammoCurr;
    [SerializeField] Image _MainWeaponbar;
    [Header("-----Secondary Weapon UI-----")]
    public GameObject SprayAmmoParent;
    [SerializeField] Image SprayAmmoBar;
    [Header("-----PickUps UI-----")]
    public GameObject pickupLabel;
    public GameObject pickupLabelContainer;
    public TMP_Text pickupText;
    [SerializeField] TMP_Text currentCollectible;
    [SerializeField] TMP_Text maxCollectible;
    [SerializeField] int maxCollectables;
    [SerializeField] int collected;
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
    public bool jumpPower { get; set; }
    public bool speedPower { get; set; }
    public bool invulnerablePower { get; set; }
    public bool fireSpeedPower { get; set; }
    public bool powerText { get; set; }
    public bool weaponPower { get; set; }

    [Header("-----Quest Text-----")]
    [SerializeField] GameObject quest;
    public TMP_Text questText;
    [Header("-----Other-----")]
    public int maxWaves; // Set the maximum number of waves allowed in the level
    public int waves;
    public bool noEnemies;
    public bool isPaused;
    public bool collectablesActive;
    public bool bossPhaseTwo;
    public bool FinalBossDead;
    public int enemiesRemain;
    public int score;
    float origTimeScale;
    private float fillTime;
    [SerializeField] Image shoveBar;
    [SerializeField] Slider slider;
    [SerializeField] Image bossHealthBar;
    [SerializeField] GameObject bossHealthObject;
    [SerializeField] GameObject[] lives;
    [SerializeField] GameObject[] livesLoose;
    [SerializeField] GameObject soundHandlerLoose;
    bool winCondition;

    void Awake()
    {
        //initiallize player variables;
        collected = 0;
        FadePowerups();
        maxCollectible.SetText(maxCollectables.ToString("0"));
        healthActive.SetActive(true);
        origTimeScale = Time.timeScale;
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPoint = GameObject.FindWithTag("Player Spawn Point");
        waves = 0;
        enableWaveUIText();
        //maxWaves = 1;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            foreach (GameObject button in quitButtons)
                button.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(waves);

        //once esc is pressed and there arent any menus active pause the game
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("P") && activeMenu == null)
        {
           // StatePaused();
            if (winCondition)
            {
            }
            else
            {
                StatePaused();
                //make the active menu be the pause menu
                //activeMenu = pauseMenu;
                //activeMenu.SetActive(isPaused);
                setActive(pauseMenu);
            }
        }

        if(enemiesRemain == 0 && waves + 1 <= maxWaves)
            IncreaseWaveCount();
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
            //let the cursor move around
            Cursor.lockState = CursorLockMode.None;
            if (activeMenu != null)
            {
                activeMenu.SetActive(false);
                activeMenu = null;
            }
        
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
    public void SetQuestText(string quest)
    {
        questText.SetText(quest);
    }
    public void SetQuestState(bool state)
    {
        quest.SetActive(state);
    }
    public void SetMainMenu()
    {
        isPaused = !isPaused;
        Time.timeScale = origTimeScale;
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
    public void SetCreditMenu()
    {
        isPaused = !isPaused;
        Time.timeScale = origTimeScale;
        SceneManager.LoadScene(sceneBuildIndex: 6);
    }
    public void SetSettings()
    {
        activeMenu.SetActive(false);
        setActive(settingsMenu);
    }
    public void SetLoading()
    {
        loadingMenu.SetActive(true);
    }
    public void SaveSettings()
    {
        activeMenu.SetActive(false);
        setActive(pauseMenu);
    }
    public void SetHelpMenu()
    {
        activeMenu.SetActive(false);
        setActive(helpMenu);
    }
    public void SetAgreeMenu()
    {
        if(activeMenu != null)
        {
            activeMenu.SetActive(false);
        }
        setActive(agreeMenu);
    }
    public void SetAgreeMenuTittle()
    {
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
        }
        setActive(agreeMenuTittle);
    }
    public void SetAgreeRespawnLoose()
    {
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
        }
        setActive(agreeMenuRespawnLoose);
    }
    public void SetAgreeMenuLoose()
    {
        activeMenu.SetActive(false);
        setActive(agreeMenuLoose);
    }

    public void SetObjectiveMenu()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                objectiveText.SetText(levelOneObjective);
                break;
            case 2:
                objectiveText.SetText(levelTwoObjective);
                break;
            case 3:
                objectiveText.SetText(levelThreeObjective);
                break;
        }
        activeMenu.SetActive(false);
        setActive(objectiveMenu);
    }
    public void SetFindButton(string message = "")
    {
        if(message != "")
        {
            findButtonText.SetText(message);
        }
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
        }
        
        StatePaused();
        setActive(findButtonMenu);
    }
    public void SetControlsMenu()
    {
        activeMenu.SetActive(false);
        setActive(controlsMenu);
    }
    public void GameOverMenu()
    {
        activeMenu.SetActive(false);
        setActive(loseMenu);
    }
    public void SetText(TMP_Text text, string textToSet)
    {
        text.SetText(textToSet);
    }
    public void AddCurrentCollectables()
    {
        collected++;
        currentCollectible.SetText(collected.ToString("0"));
    }
    public void UpdateWinCondition(int amount)
    {
        //add a counter to the enemies 
        Debug.Log("Win Condition");
        enemiesRemain += amount;
        if (enemiesRemainText != null)
            enemiesRemainText.text = enemiesRemain.ToString("0");
        //when there are no enemies or waves remaining pull the win menu up
        if ((enemiesRemain < 1 && waves == maxWaves) || FinalBossDead)
        {
            winCondition = true;
            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2)
            {
                //activeMenu = winMenu;
                StartCoroutine(NextLevelMenu(winMenu));
            }
            else
            {
                //activeMenu = nextLevelMenu;

                StartCoroutine(NextLevelMenu(nextLevelMenu));

            }
        }
    }
    public void SetBossBossObject(bool value)
    {
        bossHealthObject.SetActive(value);
    }
    public IEnumerator NextLevelMenu(GameObject Menu)
    {
        
        noEnemies = true;
        yield return new WaitForSeconds(3);
        StatePaused();
        
        setActive(Menu);
    }
    public void IncreasePlayerScore(int num)
    {
        score += num;
        scoreCount.text = score.ToString("0");
    }
    public void NextLevel()
    {
        Time.timeScale = origTimeScale;
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void IncreaseWaveCount()
    {
        if (waves + 1 <= maxWaves)
        {
            waves++;
            if (currentWaveCount != null)
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
        pMaxHealthOverlay.SetActive(false);
    }
    public void IncreaseRegenPickUpCounter()
    {
        pRegenOverlay.SetActive(false);
    }
    public void IncreaseSpeedPickUpCounter()
    {
        pSpeedOverlay.SetActive(false);
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
    public void SetLives(int life, bool state = false)
    {
        lives[life].SetActive(state);
        livesLoose[life].SetActive(state);
    }
    public void GameOver()
    {
        //pause the menu
        StatePaused();
        //set the active menu
        soundHandlerLoose.GetComponent<SoundHandler>().SetVolume();

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
    public void updateShoveUI(float shoveCooldown)
    {

        slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, fillTime);
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
        if (waveUIText != null)
            StartCoroutine(WaveUISpawnRoutine());
    }
    public void SetBossHealthBar(float value)
    {
        bossHealthBar.fillAmount = value;
    }
    IEnumerator WaveUISpawnRoutine()
    {
        WaveUIText.text = "Wave " + (waves);
        waveUIText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        waveUIText.gameObject.SetActive(false);
    }
}
