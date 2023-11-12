using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioSource bossa;

    //[SerializeField] AudioClip music;
    //[Range(0, 1)] public float audVol;

    [SerializeField] AudioSource[] Music;

    [SerializeField] AudioClip rat;
    [Range(0, 10)] public float audRatVol;
    [SerializeField] AudioClip winSound;
    [Range(0, 1)][SerializeField] float winVol;
    [SerializeField]GameObject[] SoundEffects;
    [SerializeField] GameObject[] WeaponSoundEffects;
    bool ratty;
    bool bossy;
    bool winner;
    private void Awake()
    {
        SoundEffects = GameObject.FindGameObjectsWithTag("PickUp");
        WeaponSoundEffects = GameObject.FindGameObjectsWithTag("GunPickUps");
        //WeaponHitEffects = GameObject.FindGameObjectsWithTag("GunHitAudio");
        bossa.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (ratty == false && GameManager.instance.waves == 2)
        {
            StartCoroutine(ratAud());
            ratty = true;
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1) && bossy == false && GameManager.instance.waves == 5)
        {
            aud.Stop();
            bossa.Play();
            bossy = true;
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3) && bossy == false && GameManager.instance.collectablesActive)
        {
            aud.Stop();
            bossa.Play();
            bossy = true;
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(5) && bossy == false && GameManager.instance.bossPhaseTwo)
        {
            aud.Stop();
            bossa.Play();
            bossy = true;
        }

        if (winner == false && GameManager.instance.noEnemies && GameManager.instance.waves == 5)
        {
            winner = true;
            aud.PlayOneShot(winSound);
        }
    }
    public void SetMusicAudio(float newVolume)
    {
        for(int i = 0; i < Music.Length; i++)
        {
            Music[i].volume = newVolume;
        }
        //audRatVol = 10 * newVolume;
        //winVol = newVolume;
    }
    public void SetEffectsAudio(float newVolume)
    {
        
        for(int i = 0; i < SoundEffects.Length; i++)
        {
            if(SoundEffects[i].GetComponent<Pickups>() != null)
            {
                SoundEffects[i].GetComponent<Pickups>().SetAudio(newVolume);
            }
            
        }
        for (int i = 0; i < WeaponSoundEffects.Length; i++)
        {
            if (WeaponSoundEffects[i].GetComponent<GunPickups>() != null)
            {
                WeaponSoundEffects[i].GetComponent<GunPickups>().SetAudio(newVolume);
            }
        }
        // pickUpAudio.GetComponent<Pickups>().SetAudio(newVolume);
        // gunPickUpAudio.GetComponent<GunPickups>().SetAudio(newVolume);
    }
    IEnumerator ratAud()
    {
        aud.PlayOneShot(rat, audRatVol);
        yield return new WaitForSeconds(0.5f);
    }



  
}
