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
    public void Settings()
    {
        GameManager.instance.SetSettings();
    }
    public void HelpMenu()
    {
        GameManager.instance.SetHelpMenu();
    }
    public void SaveSettings()
    {
        GameManager.instance.SaveSettings();
    }
    public void ControlsMenu()
    {
        
        GameManager.instance.SetControlsMenu();
    }
    public void ObjectivesMenu()
    {
        GameManager.instance.SetObjectiveMenu();
    }

    public void MainMenu()
    {
        GameManager.instance.SetMainMenu();
    }
    public void NextLevel()
    {
        
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
