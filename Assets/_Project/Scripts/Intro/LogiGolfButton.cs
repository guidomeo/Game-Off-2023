using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogiGolfButton : MonoBehaviour
{
    [SerializeField] private float timeToEnableClick;
    [SerializeField] private AudioData clickAudio;
    [SerializeField] private SlideShowManager slideShowManager;

    private bool mouseEnter = false;

    private string normalText, enterText;

    private void Awake()
    {
        normalText = slideShowManager.slides[0].text[0];
        enterText = slideShowManager.slides[0].text[0]
            .Replace("green", "blue")
            .Replace("<sprite=0>", "<sprite=1>");
        
    }

    private void Update()
    {
        if (slideShowManager.timer > timeToEnableClick) Destroy(GetComponent<SpriteRenderer>());
    }

    private void OnMouseEnter()
    {
        mouseEnter = true;
        slideShowManager.slides[0].text[0] = enterText;
    }
    private void OnMouseExit()
    {
        mouseEnter = false;
        slideShowManager.slides[0].text[0] = normalText;
    }

    private void OnMouseDown()
    {
        if (slideShowManager.timer > timeToEnableClick)
        {
            clickAudio.Play();
        }
    }

    private void OnMouseUp()
    {
        if (!mouseEnter) return;
        if (slideShowManager.timer > timeToEnableClick)
        {
            Application.OpenURL("https://store.steampowered.com/app/2388030/LogiGolf/");
        }
    }
}
