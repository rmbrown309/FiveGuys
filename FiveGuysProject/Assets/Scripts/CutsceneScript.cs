using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutSceneScript : MonoBehaviour
{
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource sfx;
    [SerializeField] SavedSettings settings;

    void Start()
    {
        music.volume = settings.MusicVolume;
        sfx.volume = settings.SoundEffectVoulume;
        StartCoroutine(NextScene());
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("P"))
        {
            StopAllCoroutines();
            StartCoroutine(SkipScene());
        }
    }

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator SkipScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}