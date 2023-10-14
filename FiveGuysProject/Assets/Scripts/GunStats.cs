using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public float shootRate;
    public int shootDamage;
    public int shootTime;
    public int ammoCur;
    public int ammoMax;

    public GameObject model;
    public GameObject bullet;
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0, 1)] public float audShotVol;
}

