using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public float panDistance = 15f;
    public static AudioManager instance;

    List<AudioSource> audioSourceList = new();

    private List<AudioData> waitingAudioData = new();

    public float PanFromPosition(Vector2 position)
    {
        return (position.x - CameraController.Position.x) / panDistance;
    }
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance != this) return;
        
        instance = null;
        foreach (var audioData in waitingAudioData)
        {
            audioData.canPlay = true;
        }
    }

    public void Play(AudioData audioData, bool loop)
    {
        if (audioData.clips.Length == 0) return;

        if (!audioData.canPlay) return;
        if (audioData.TimeToPlayAgain > 0f)
        {
            audioData.canPlay = false;
            StartCoroutine(CO_WaitToCanPlay(audioData));
            waitingAudioData.Add(audioData);
        }


        if (audioData.Delay == 0f)
            PlaySound(audioData, loop);
        else
            StartCoroutine(CO_PlayDelayed(audioData, loop));
    }
    
    IEnumerator CO_WaitToCanPlay(AudioData audioData)
    {
        yield return new WaitForSeconds(audioData.TimeToPlayAgain);
        audioData.canPlay = true;
        waitingAudioData.Remove(audioData);
    }
    
    IEnumerator CO_PlayDelayed(AudioData audioData, bool loop)
    {
        yield return new WaitForSeconds(audioData.Delay);
        PlaySound(audioData, loop);
    }

    void PlaySound(AudioData audioData, bool loop)
    {
        AudioSource audioSource = null;
        foreach (AudioSource audioS in audioSourceList)
        {
            if (!audioS.isPlaying)
            {
                audioSource = audioS;
                break;
            }
        }
        if (audioSource == null)
        {
            GameObject obj = new GameObject($"Audio {audioSourceList.Count}");
            obj.transform.parent = transform;
            audioSource = obj.AddComponent<AudioSource>();
            audioSourceList.Add(audioSource);
        }
        
        audioData.Setup(audioSource);

        if (audioData.fadeInDuration > 0f)
        {
            float volume = audioSource.volume;
            audioSource.volume = 0f;
            audioSource.DOFade(volume, audioData.fadeInDuration);
        }
        audioSource.loop = loop;
        audioSource.Play();
    }
}
