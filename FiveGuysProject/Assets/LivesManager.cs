using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    [SerializeField] GameObject[] lives;
    public void SetLives(int life, bool state = false)
    {
        lives[life].SetActive(state);
    }
}
