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


    bool ratty ;
    bool bossy;
    private void Awake()
    {
        bossa.Pause();
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
            aud.Pause();
            bossa.Play();
            bossy = true;
        }
    }

    IEnumerator ratAud()
    {
        aud.PlayOneShot(rat, audRatVol);
        yield return new WaitForSeconds(0.5f);
    }



  
}
