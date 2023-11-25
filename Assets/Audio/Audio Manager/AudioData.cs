using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Audio Data", menuName = "Data/Audio Data")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public class RandomSettings
    {
        [Min(0f)] public float volumeUp = 0f;
        [Min(0f)] public float volumeDown = 0f;
        [Min(0f)] public float pitchUp = 0f;
        [Min(0f)] public float pitchDown = 0f;
    }

    public AudioClip[] clips;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;
    [Range(-3f, 3f)] [SerializeField] private float pitch = 1f;
    [Min(0f)] [SerializeField] private float delay = 0f;
    [Min(0f)] [SerializeField] private float timeToPlayAgain = 0f;
    [SerializeField] private RandomSettings randomSettings;

    [NonSerialized] public float volumeMultiplier = 1f;

    public float Delay => delay;
    public float TimeToPlayAgain => timeToPlayAgain;
    
    public void Play(bool loop = false) => AudioManager.instance.Play(this, loop);

    private int lastClipIndex = -1;

    [NonSerialized] public bool canPlay = true;
    
    public void Setup(AudioSource source)
    {
        if (clips.Length == 0) return;
        source.outputAudioMixerGroup = audioMixerGroup;

        int clipIndex;
        do
        {
            clipIndex = Random.Range(0, clips.Length);
        } while (clipIndex == lastClipIndex && clips.Length > 1);
        lastClipIndex = clipIndex;
        source.clip = clips[clipIndex];
        
        
        source.volume = (volume + Random.Range(-randomSettings.volumeDown, randomSettings.volumeUp)) * volumeMultiplier;
        source.pitch = pitch + Random.Range(-randomSettings.pitchDown, randomSettings.pitchUp);
    }
}