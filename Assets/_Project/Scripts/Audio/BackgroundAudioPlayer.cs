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

        public void Fade(float target, float duration)
        {
            tween?.Kill();
            tween = source.DOFade(target, duration).
                OnComplete(() => tween = null);
        }
    }

    [SerializeField] private float startFade;
    [SerializeField] private float fade;
    
    Dictionary<string, BackgroundAudio> backgroundAudioDict = new();

    public static BackgroundAudioPlayer instance;


    private void Awake()
    {
        instance = this;
        foreach (var source in GetComponentsInChildren<AudioSource>())
        {
            var backAudio = new BackgroundAudio { source = source, volume = source.volume };
            backgroundAudioDict.Add(source.gameObject.name, backAudio);
            
            bool play = backAudio.source.playOnAwake;
            backAudio.source.playOnAwake = true;
            backAudio.source.volume = 0f;
            if (play) backAudio.Fade(backAudio.volume, startFade);
        }
    }

    public void Play(string backgroundName)
    {
        var backAudio = backgroundAudioDict[backgroundName];
        if (backAudio.source.name != backgroundName) return;
        backAudio.Fade(backAudio.volume, fade);
    }
    
    public void Stop(string backgroundName)
    {
        var backAudio = backgroundAudioDict[backgroundName];
        if (backAudio.source.name != backgroundName) return;
        backAudio.Fade(0f, fade);
    }
}
