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

    private float timer = 0f;
    private int slideIndex = 0;
    private int textIndex = 0;
    private bool end = false;
    private void Update()
    {
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
                        SceneManager.LoadScene(2);
                    });
                    end = true;
                }
            }
        }
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
