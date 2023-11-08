using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public int weaponID;
    public int ammoID;
    public float shootRate;
    public float shootDamage;
    public float shootTime;
    public int ammoCur;
    public int ammoMax;
    public int cost;
    public string gunName;
    public bool isShotgun;
    public bool isM16;
    public bool isPowerWeapon;
    public int numOfPellets;
    public float horizSpread;
    public float vertSpread;
    public float zSpread;
    public GameObject model;
    public GameObject bullet;
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0, 1)] public float audShotVol;
    public float xOffset;
    public float yOffset;
    public float zOffset;


}

