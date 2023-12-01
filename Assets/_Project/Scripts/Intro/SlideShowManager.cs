using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SlideShowManager : MonoBehaviour
{
    [Serializable]
    public struct Slide
    {
        public Vector2 posFrom;
        public Vector2 posTo;
        public float scaleFrom;
        public float scaleTo;
        public Sprite sprite;
        [TextArea] public List<string> text;
    }

    [SerializeField] private int nextScene;
    [SerializeField] private float textDuration;
    [SerializeField] [Range(0,1)] private float textEndPercentage;
    [SerializeField] private float endFadeDuration = 3f;
    public Slide[] slides;
    [SerializeField] private SpriteRenderer spriteRend;
    [SerializeField] private TMP_Text textComp;
    [SerializeField] private TMP_Text holdToSkip;
    [SerializeField] private string backToPlay = "Soundtrack Intro";
    [SerializeField] private SpriteRenderer fadeRend;
    [SerializeField] private float startFade = 0.6f;
    [SerializeField] private bool canSkip = false;

    [NonSerialized] public float timer = 0f;
    private int slideIndex = 0;
    private int textIndex = 0;
    private bool end = false;

    public bool holdAnimEnd = false;
    private Tween doFadeTween;
    private float holdTimer = 0f;

    private bool click = false;

    private bool started = false;

    private void Start()
    {
        Color c = holdToSkip.color;
        c .a = 0f;
        holdToSkip.color = c;
        
        fadeRend.color = Color.black;
        fadeRend.DOFade(0f, startFade).OnComplete(() =>
        {
            started = true;
            BackgroundAudioPlayer.instance.StopAll(1f);
            BackgroundAudioPlayer.instance.Play(backToPlay);
        });
    }

    private void Update()
    {
        if (started)
        {
            if (!OverUI() && Input.GetMouseButtonDown(0))
            {
                click = true;
                holdAnimEnd = false;
                doFadeTween?.Kill();
                doFadeTween = holdToSkip.DOFade(1f, 0.7f).SetEase(Ease.OutQuad)
                    .OnComplete(() => holdAnimEnd = true);
            }

            if (canSkip && click)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer > 2f)
                {
                    GoNext(0.5f);
                }
            }

            if (!Input.GetMouseButton(0))
            {
                click = false;
                holdTimer = 0f;
                if (holdAnimEnd)
                {
                    holdAnimEnd = false;
                    doFadeTween?.Kill();
                    doFadeTween = holdToSkip.DOFade(0f, 0.7f).SetEase(Ease.InQuad);
                }
            }
        }


        Slide slide = slides[slideIndex];
        
        float textT = timer / textDuration;
        
        textComp.text = CutString(slide.text[textIndex], Mathf.Clamp01(textT / textEndPercentage));

        float slideT = (textDuration * textIndex + timer) / (textDuration * slide.text.Count);
        
        spriteRend.sprite = slide.sprite;
        spriteRend.transform.position = Vector2.LerpUnclamped(slide.posFrom, slide.posTo, slideT);
        spriteRend.transform.localScale = Vector3.one * Mathf.LerpUnclamped(slide.scaleFrom, slide.scaleTo, slideT);

        if (started)
        {
            float increment = Time.deltaTime;
            if (!canSkip && Input.GetMouseButton(0)) increment *= 5f;
            timer += increment;
        }
        
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
                    
                    GoNext(endFadeDuration);
                }
            }
        }
    }

    void GoNext(float duration)
    {
        end = true;
        fadeRend.DOFade(1f, duration).OnComplete(() =>
        {
            SceneManager.LoadScene(nextScene);
        });
    }

    string CutString(string text, float t)
    {
        string cutString = RemoveTags(text);
        int length = cutString.Length;

        int pos = Mathf.FloorToInt(length * t);
        string result = SubString(text, pos, out string next);
        result += "</u><color=#00000000>";
        result += RemoveTags(next);
        return result;
    }

    string RemoveTags(string text)
    {
        string result = "";
        bool openTag = false;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '<')
            {
                openTag = true;
            }
            else if (c == '>')
            {
                openTag = false;
            }
            else if (!openTag)
            {
                result += c;
            }
        }

        return result;
    }

    string SubString(string text, int pos, out string next)
    {
        next = "";
        string result = "";
        bool openTag = false;
        int posCounter = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (posCounter < pos)
            {
                result += c;
            }
            else
            {
                next += c;
            }
            if (c == '<')
            {
                openTag = true;
            }
            else if (c == '>')
            {
                openTag = false;
            }
            else if (!openTag)
            {
                posCounter++;
            }
        }

        return result;
    }
    
    List<RaycastResult> GetEventSystemRaycastResults()
    {   
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventData, raycastResults );

        return raycastResults;
    }

    bool OverUI() => GetEventSystemRaycastResults().Count > 0;
}
