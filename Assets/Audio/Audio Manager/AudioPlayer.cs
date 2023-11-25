using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioData audioData;
    public bool playOnStart;
    public bool loop;

    private void Start()
    {
        if (playOnStart) Play();
    }
    public void Play()
    {
        audioData.Play(loop);
    }
}
