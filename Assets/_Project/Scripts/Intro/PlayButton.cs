using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Color colorNormal;
    [SerializeField] private Color colorEnter;
    [SerializeField] private Color colorClick;
    [SerializeField] private TMP_Text textComp;

    private bool mouseEnter = false;
    private void Awake()
    {
        textComp.color = colorNormal;
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
        textComp.DOColor(colorClick, duration);
    }

    private void OnMouseUp()
    {
        if (!mouseEnter) return;
        SceneManager.LoadScene(1);
    }
}
