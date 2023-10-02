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
    [SerializeField] GameObject healthActive;
    [SerializeField] TMP_Text enemiesRemainText;
    [SerializeField] TMP_Text scoreCount;

    public Image playerHealthBar;
    public bool isPaused;
    int enemiesRemain;
    int score;
    float origTimeScale;

    void Awake()
    {
        //initiallize player variables;
        healthActive.SetActive(true);
        origTimeScale = Time.timeScale;
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPoint = GameObject.FindWithTag("Player Spawn Point");
    }

    // Update is called once per frame
    void Update()
    {
        //once esc is pressed and there arent any menus active pause the game
        if(Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            StatePaused();
            //make the active menu be the pause menu
            //activeMenu = pauseMenu;
            //activeMenu.SetActive(isPaused);
            setActive(pauseMenu);
        }
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

    public void UpdateWinCondition(int amount)
    {
        //add a counter to the enemies 
        enemiesRemain += amount;
        enemiesRemainText.text = enemiesRemain.ToString("0");
        //when there are no enemies remaining pull the win menu up
        if(enemiesRemain < 1)
        {
            StatePaused();
            setActive(winMenu);
        }
    }
    public void IncreasePlayerScore(int num)
    {
        score += num;
        scoreCount.text = score.ToString("0");
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
}
