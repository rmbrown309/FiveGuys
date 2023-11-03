using UnityEngine;

[CreateAssetMenu]
public class SavedSettings : ScriptableObject
{
    [Range(0, 1)] public float masterVolume;
    [Range(0, 1)] public float MusicVolume;
    [Range(0, 1)] public float SoundEffectVoulume;
    [Range(0, 1)] public float Sensitivity;
}
