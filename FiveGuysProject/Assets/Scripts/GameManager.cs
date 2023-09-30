using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] GameObject looseMenu;

    public bool isPaused;
    float origTimeScale;

    void Awake()
    {
        //initiallize player variables;
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
        //make the active menu be the pause menu
        activeMenu = pauseMenu;
        activeMenu.SetActive(isPaused);
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
}
