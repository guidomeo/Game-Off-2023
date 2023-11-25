using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    List<AudioSource> audioSourceList = new();
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
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
        audioSource.loop = loop;
        audioSource.Play();
    }
}
