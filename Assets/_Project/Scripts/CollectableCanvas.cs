using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CollectableCanvas : MonoBehaviour
{
    [SerializeField] private Image[] collectables;
    [SerializeField] private AudioData[] sounds;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject allItemsObj;
    
    public static CollectableCanvas instance;

    private int count = 0;

    private void Awake()
    {
        if (instance != null)
        {
            if (count > 0) instance.ShowItems();
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        
        gameObject.SetActive(false);
        allItemsObj.SetActive(false);
        foreach (var img in collectables)
        {
            img.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if (instance != this) return;
        
        instance = null;
    }

    private Tween fadeOut;

    public void ShowItems()
    {
        fadeOut?.Kill();
        canvasGroup.DOFade(1f, 2f);
        fadeOut = canvasGroup.DOFade(0f, 2f).SetDelay(10f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Collect(int index)
    {
        fadeOut?.Kill();
        
        sounds[count].Play();
        count++;
        
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        
        Image img = collectables[index];
        var tr = img.transform;
        tr.gameObject.SetActive(true);
        img.color = new Color(1f, 1f, 1f, 0f);
        tr.localScale = Vector3.one * 2f;

        bool allUnlocked = (count == collectables.Length);
        if (allUnlocked) DrawingManager.instance.maxNumberOfDrawings = 3;
        
        canvasGroup.DOFade(1f, 2f).OnComplete(() =>
        {
            img.DOFade(1f, 1f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                tr.DOScale(1f, 1f).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    if (!allUnlocked) return;
                    
                    allItemsObj.SetActive(true);
                    fadeOut = canvasGroup.DOFade(0f, 2f).SetDelay(10f).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                });
            });
        });

        if (!allUnlocked)
        {
            fadeOut = canvasGroup.DOFade(0f, 2f).SetDelay(10f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        
    }

    public bool IsUnlocked(int index)
    {
        return collectables[index].gameObject.activeSelf;
    }
}
