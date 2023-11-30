using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundAudioPlayer : MonoBehaviour
{
    [Serializable]
    public class BackgroundAudio
    {
        public AudioSource source;
        public float volume;
        public Tween tween;

        public Tween Fade(float target, float duration)
        {
            tween?.Kill();
            tween = source.DOFade(target, duration).
                OnComplete(() => tween = null);
            return tween;
        }
    }

    [SerializeField] private float defaultFade = 2f;
    
    Dictionary<string, BackgroundAudio> backgroundAudioDict = new();

    public static BackgroundAudioPlayer instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        foreach (var source in GetComponentsInChildren<AudioSource>())
        {
            var backAudio = new BackgroundAudio { source = source, volume = source.volume };
            backgroundAudioDict.Add(source.gameObject.name, backAudio);
            
            //bool play = backAudio.source.playOnAwake;
            //backAudio.source.playOnAwake = false;
            backAudio.source.volume = 0f;
            //if (play) backAudio.Fade(backAudio.volume, defaultFade);
            //source.Play();
        }
    }

    private void OnDestroy()
    {
        if (instance != this) return;
        
        instance = null;
    }

    public void Play(string backgroundName, float fade = 0f)
    {
        fade = (fade == 0f) ? defaultFade : fade;
        var backAudio = backgroundAudioDict[backgroundName];
        if (backAudio.source.name != backgroundName) return;
        if (!backAudio.source.isPlaying)
        {
            backAudio.source.Play();
        }
        backAudio.Fade(backAudio.volume, fade);
    }
    
    public void ChangeVolume(string backgroundName, float volumeMultiplier, float fade = 0f)
    {
        fade = (fade == 0f) ? defaultFade : fade;
        var backAudio = backgroundAudioDict[backgroundName];
        backAudio.Fade(backAudio.volume * volumeMultiplier, fade);
    }
    
    public void Stop(string backgroundName, float fade = 0f)
    {
        fade = (fade == 0f) ? defaultFade : fade;
        var backAudio = backgroundAudioDict[backgroundName];
        backAudio.Fade(0f, fade);
    }
    
    public void StopAll(float fade = 0f)
    {
        fade = (fade == 0f) ? defaultFade : fade;
        foreach (var pair in backgroundAudioDict)
        {
            var backAudio = pair.Value;
            backAudio.Fade(0f, fade);
        }
        
    }

    public void ResetPlay(float fade = 0f)
    {
        fade = (fade == 0f) ? defaultFade : fade;
        foreach (var pair in backgroundAudioDict)
        {
            var backAudio = pair.Value;
            backAudio.Fade(0f, fade).OnComplete(
                () =>
                {
                    backAudio.source.Stop();
                });
        }
    }
}
