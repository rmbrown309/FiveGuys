using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public float shootRate;
    public float shootDamage;
    public float shootTime;
    public int ammoCur;
    public int ammoMax;
    public int cost;
    public string gunName;
    public bool isShotgun;
    public int numOfPellets;
    public float inaccuracy;
    public GameObject model;
    public GameObject bullet;
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0, 1)] public float audShotVol;
}

