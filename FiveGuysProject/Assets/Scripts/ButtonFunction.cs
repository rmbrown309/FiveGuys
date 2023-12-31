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
        GameManager.instance.SetLoading();
        Application.Quit();
    }
    public void Restart()
    {
        //restart the scene
        GameManager.instance.SetLoading();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Resume();
    }

    public void Respawn()
    {
        //spawn the player at a spawn point
        if (GameManager.instance.numberOfLives > 0)
        {

            GameManager.instance.SetLives(GameManager.instance.numberOfLives - 1, false);
            GameManager.instance.numberOfLives -= 1;
            GameManager.instance.playerScript.SpawnPlayer();
            Resume();
        }
    }
    public void Settings()
    {
        GameManager.instance.SetSettings();
    }
    public void FindButton()
    {
        GameManager.instance.SetFindButton();
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
        GameManager.instance.SetLoading();
        GameManager.instance.SetMainMenu();
    }
    public void Credits()
    {
        GameManager.instance.SetLoading();
        GameManager.instance.SetCreditMenu();
    }
    public void NextLevel()
    {
        GameManager.instance.SetLoading();
        GameManager.instance.NextLevel();
    }
    public void AgreeMenu()
    {
        GameManager.instance.SetAgreeMenu();
    }
    public void AgreeMenuTittle()
    {
        GameManager.instance.SetAgreeMenuTittle();
    }
    public void AgreeMenuLooseRespawn()
    {
        GameManager.instance.SetAgreeRespawnLoose();
    }
    public void AgreeLooseMenu()
    {
        GameManager.instance.SetAgreeMenuLoose();
    }
    public void GameOverMenu()
    {
        GameManager.instance.GameOverMenu();
    }
}
