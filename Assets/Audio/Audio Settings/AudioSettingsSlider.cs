using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsSlider : MonoBehaviour
{
    //[Range(0f, 1f)] [SerializeField] private float defaultValue = 1f;
    [SerializeField] string volumeVarName = "Music";
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color disabledTextColor = Color.gray;
    [SerializeField] private AudioData sound;
    [Header("References")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] private TMP_Text textComp;

    private bool playAudio = true;
    
    private void Awake()
    {
        slider.onValueChanged.AddListener(OnValueChange);
    }
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnValueChange);
    }
    private void Start()
    {
        mixer.GetFloat(volumeVarName, out float volume);
        float value = volume < -79f ? 0f : Mathf.Pow(10, volume / 20f);
        playAudio = false;
        slider.value = value;
        playAudio = true;
    }

    void OnValueChange(float value)
    {
        if (playAudio && sound != null) sound.Play();
        float volume = (value == 0f) ? -80f : Mathf.Log10(value) * 20f;
        mixer.SetFloat(volumeVarName, volume);
        textComp.color = (value == 0f) ? disabledTextColor : normalTextColor;
    }
}
