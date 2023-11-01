using UnityEngine;

[CreateAssetMenu]
public class SavedSettings : ScriptableObject
{
    [Range(0, 1)] public float volume;
    [Range(0, 1)] public float Sensitivity;
}
