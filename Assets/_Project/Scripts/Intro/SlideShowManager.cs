using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideShowManager : MonoBehaviour
{
    [Serializable]
    struct Slide
    {
        public Vector2 posFrom;
        public Vector2 posTo;
        public float scaleFrom;
        public float scaleTo;
        public Sprite sprite;
        public List<string> text;
    }

    [SerializeField] private float textDuration;
    [SerializeField] [Range(0,1)] private float textEndPercentage;
    [SerializeField] private float endFadeDuration = 3f;
    [SerializeField] private Slide[] slides;
    [SerializeField] private SpriteRenderer spriteRend;
    [SerializeField] private TMP_Text textComp;
    [SerializeField] private TMP_Text holdToSkip;

    private float timer = 0f;
    private int slideIndex = 0;
    private int textIndex = 0;
    private bool end = false;

    private Tween doFadeTween;
    private float holdTimer = 0f;

    private void Awake()
    {
        holdToSkip.DOFade(0f, 0.05f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            doFadeTween?.Kill();
            doFadeTween = holdToSkip.DOFade(1f, 0.7f).SetEase(Ease.OutQuad);
        }
        
        if (Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;
            if (holdTimer > 2f)
            {
                GoNext();
            }
        }
        else
        {
            holdTimer = 0f;
            if (doFadeTween != null && holdToSkip.color.a > 0.9f)
            {
                doFadeTween?.Kill();
                doFadeTween = holdToSkip.DOFade(0f, 0.7f).SetEase(Ease.InQuad);
            }
        }
        
        
        Slide slide = slides[slideIndex];
        
        float textT = timer / textDuration;
        
        textComp.text = CutString(slide.text[textIndex], Mathf.Clamp01(textT / textEndPercentage));

        float slideT = (textDuration * textIndex + timer) / (textDuration * slide.text.Count);
        
        spriteRend.sprite = slide.sprite;
        spriteRend.transform.position = Vector2.LerpUnclamped(slide.posFrom, slide.posTo, slideT);
        spriteRend.transform.localScale = Vector3.one * Mathf.LerpUnclamped(slide.scaleFrom, slide.scaleTo, slideT);

        timer += Time.deltaTime;
        
        if (end) return;
        
        if (timer >= textDuration)
        {
            timer -= textDuration;
            if (textIndex < slide.text.Count - 1)
            {
                textIndex++;
            }
            else
            {
                if (slideIndex < slides.Length - 1)
                {
                    textIndex = 0;
                    slideIndex++;
                }
                else
                {
                    timer += textDuration;
                    spriteRend.DOFade(0f, endFadeDuration);
                    textComp.DOFade(0f, endFadeDuration).OnComplete(() =>
                    {
                        GoNext();
                    });
                    end = true;
                }
            }
        }
    }

    void GoNext()
    {
        SceneManager.LoadScene(2);
    }

    string CutString(string text, float t)
    {
        int length = text.Length;

        int pos = Mathf.FloorToInt(length * t);
        string result = text.Substring(0, pos);
        result += "<color=#00000000>";
        result += text.Substring(pos);
        return result;
    }
}
