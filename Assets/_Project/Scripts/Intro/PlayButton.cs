using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private AudioData clickAudio;
    [SerializeField] private float duration;
    [SerializeField] private Color colorNormal;
    [SerializeField] private Color colorEnter;
    [SerializeField] private Color colorClick;
    [SerializeField] private TMP_Text textComp;
    [SerializeField] private SpriteRenderer fadeRend;

    private bool mouseEnter = false;
    private void Awake()
    {
        textComp.color = colorNormal;
        fadeRend.color = new Color(0f, 0f, 0f, 0f);
    }

    private void Start()
    {
        BackgroundAudioPlayer.instance.ResetPlay();
    }

    private void OnValidate()
    {
        textComp.color = colorNormal;
    }

    private void OnMouseEnter()
    {
        mouseEnter = true;
        textComp.DOColor(colorEnter, duration);
    }
    private void OnMouseExit()
    {
        mouseEnter = false;
        textComp.DOColor(colorNormal, duration);
    }

    private void OnMouseDown()
    {
        clickAudio.Play();
        textComp.DOColor(colorClick, duration);
    }

    private void OnMouseUp()
    {
        if (!mouseEnter) return;
        fadeRend.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene(1);
        });
    }
}
