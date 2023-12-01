using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private CanvasGroup[] tutorialPanels;

    public static TutorialCanvas instance;
    private void Awake()
    {
        instance = this;
        
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            tutorialPanels[i].gameObject.SetActive(false);
        }
    }

    public void TutorialTrigger(int index, bool active, float delay = 0f)
    {
        CanvasGroup image = tutorialPanels[index];
        GameObject imageObj = image.gameObject;
        if (active)
        {
            if (!imageObj.activeSelf)
            {
                imageObj.SetActive(true);
                image.alpha = 0f;
                image.DOFade(1f, duration).SetDelay(delay);
            }
        }
        else
        {
            if (imageObj.activeSelf)
            {
                image.alpha = 1f;
                image.DOFade(0f, duration).OnComplete(() =>
                {
                    imageObj.SetActive(false);
                });
            }
        }
    }
}
