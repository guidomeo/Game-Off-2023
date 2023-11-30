using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogiGolfButton : MonoBehaviour
{
    [SerializeField] private AudioData clickAudio;

    private bool mouseEnter = false;
    private void OnMouseEnter()
    {
        mouseEnter = true;
    }
    private void OnMouseExit()
    {
        mouseEnter = false;
    }

    private void OnMouseDown()
    {
        clickAudio.Play();
    }

    private void OnMouseUp()
    {
        if (!mouseEnter) return;
        Application.OpenURL("https://store.steampowered.com/app/2388030/LogiGolf/");
    }
}
