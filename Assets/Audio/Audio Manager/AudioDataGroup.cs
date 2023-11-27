using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Audio Data Group", menuName = "Data/Audio Data Group")]
public class AudioDataGroup : ScriptableObject
{
    [System.Serializable]
    public class Variant
    {
        public string name;
        public AudioData audioData;
    }

    public Variant[] variants;

    public void Play(params float[] volumes)
    {
        for (var i = 0; i < variants.Length; i++)
        {
            var variant = variants[i];
            variant.audioData.volumeMultiplier = volumes[i];
            if (variant.audioData.volumeMultiplier <= 0f) continue;
            variant.audioData.Play();
        }
    }
}