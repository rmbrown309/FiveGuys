using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField] AudioSource sound;
    [SerializeField] SavedSettings volume;
    // Start is called before the first frame update
    private void Start()
    {
        sound.volume = volume.SoundEffectVoulume;
        sound.pitch = Random.Range(0.5f, 1.5f);
    }
    public void PlaySound()
    {
        sound.volume = volume.SoundEffectVoulume;
        sound.pitch = Random.Range(0.5f, 1.5f);
        sound.Play();
    }
    public void SetVolume()
    {
        sound.volume = volume.SoundEffectVoulume;
        sound.pitch = Random.Range(0.5f, 1.5f);
    }
}
