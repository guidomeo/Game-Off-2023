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

    [NonSerialized] public float volumeMultiplier = 1f;
    [NonSerialized] public float pitchMultiplier = 1f;
    [NonSerialized] public float stereoPan = 0f;
    

    public void Play(params float[] volumes)
    {
        for (var i = 0; i < variants.Length; i++)
        {
            var variant = variants[i];
            Play(variant, volumes[i]);
        }
    }

    public void Play(int index)
    {
        var variant = variants[index];
        Play(variant, 1f);
    }

    void Play(Variant variant, float volume)
    {
        if (volume <= 0f) return;
        
        volume *= volumeMultiplier;
        
        variant.audioData.volumeMultiplier = volume;
        variant.audioData.pitchMultiplier = pitchMultiplier;
        variant.audioData.stereoPan = stereoPan;
            
        variant.audioData.Play();
    }
}