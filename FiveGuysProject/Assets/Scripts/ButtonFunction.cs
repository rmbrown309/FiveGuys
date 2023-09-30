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
        Application.Quit();
    }
}
