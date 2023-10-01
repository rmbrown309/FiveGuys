using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonFunction : MonoBehaviour
{
    //Functions for the buttons to use
    //calls methods from the game manager
    public void Resume()
    {
        GameManager.instance.StateUnpaused();
    }
    public void Quit()
    {
        //quit the game
        Application.Quit();
    }
    public void Restart()
    {
        //restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Resume();
    }

    public void Respawn()
    {
        //spawn the player at a spawn point
        GameManager.instance.playerScript.SpawnPlayer();
        Resume();
    }
}
