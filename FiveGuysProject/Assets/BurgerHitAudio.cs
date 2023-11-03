using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerHitAudio : MonoBehaviour
{
    [SerializeField] AudioSource HitSound;
    [SerializeField] SavedSettings sound;
    // Start is called before the first frame update
    void Start()
    {
        HitSound.volume = sound.SoundEffectVoulume;
    }
}
