using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MasterVolume : ScriptableObject
{
    [Range(0, 1)] public float volume;
}
