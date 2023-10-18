using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioSource bossa;

    //[SerializeField] AudioClip music;
    //[Range(0, 1)] public float audVol;

    [SerializeField] AudioClip rat;
    [Range(0, 10)] public float audRatVol;
    [SerializeField] AudioClip winSound;
    [Range(0, 1)][SerializeField] float winVol;

    bool ratty;
    bool bossy;
    bool winner;
    private void Awake()
    {
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

        if (bossy == false && GameManager.instance.waves == 5)
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

    IEnumerator ratAud()
    {
        aud.PlayOneShot(rat, audRatVol);
        yield return new WaitForSeconds(0.5f);
    }



  
}
