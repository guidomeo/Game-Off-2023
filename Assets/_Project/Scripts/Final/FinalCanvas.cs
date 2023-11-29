using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FinalCanvas : MonoBehaviour
{
    [SerializeField] private GameObject textPanel;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private GameObject cinemaPanel;
    [SerializeField] private CanvasGroup cinemaCanvasGroup;
    [SerializeField] private TMP_Text textComp;

    public static FinalCanvas instance;

    private void Awake()
    {
        instance = this;
        textPanel.SetActive(false);
        cinemaPanel.SetActive(false);
    }

    public void ShowCinemaPanel()
    {
        if (cinemaPanel.activeSelf) return;
        cinemaPanel.SetActive(true);
        cinemaCanvasGroup.alpha = 0f;
        cinemaCanvasGroup.DOFade(1f, 3f);
    }
    
    public void HideCinemaPanel()
    {
        cinemaCanvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            cinemaPanel.SetActive(false);
        });
    }

    public void Activate()
    {
        textPanel.SetActive(true);

        Sequence anim = DOTween.Sequence();

        string text = textComp.text;
        textComp.text = "";
        textCanvasGroup.alpha = 0f;
        
        int textLength = 0;
        
        anim.Append(textCanvasGroup.DOFade(1f, 0.5f));
        anim.Append(DOTween.To(
            () => textLength,
            l =>
            {
                textLength = l;
                textComp.text = text.Substring(0, textLength);
            },
            text.Length,
            10f
            ));
        anim.AppendInterval(1f);
        anim.Append(textCanvasGroup.DOFade(0f, 0.5f));
        anim.OnComplete(() => textPanel.SetActive(false));
    }
}
