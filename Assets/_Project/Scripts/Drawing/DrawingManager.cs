using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrawingManager : MonoBehaviour
{
    public static bool isDrawing;
    
    public int maxNumberOfDrawings = 3;
    [SerializeField] private Drawing drawingPrefab;
    [SerializeField] private GameObject cannotBuildEffect;
    [SerializeField] private float normalWidth = 0.1f;
    [SerializeField] private float bigWidth = 0.8f;
    [Header("Pencil Sound")]
    [SerializeField] private float startDrawingSpeed;
    [SerializeField] private float fadeDuration = 0.1f;
    [SerializeField] private float pitchOffset;
    [SerializeField] private float drawingSpeedChangeSpeed;
    [SerializeField] private float drawingDirectionChangeSpeed;
    
    [SerializeField] private float drawingSpeedMin;
    [SerializeField] private float drawingSpeedMax;
    [SerializeField] [Range(0f, 1f)] private float pencilVolumePower = 0f;
    [SerializeField] [Range(0f, 1f)] private float pencilVolumeMultiplier;
    [SerializeField] private AnimationCurve pencilVolume;
    [SerializeField] private AnimationCurve pencilPitch;
    
    [SerializeField] private AudioSource pencilDrawingSource;
    [SerializeField] private AudioData playerVoice;
    [SerializeField] private AudioData clearAudio;
    [SerializeField] private float pitchClearMin;
    [SerializeField] private float pitchClearMax;
    [SerializeField] private float lengthToMaxPitchClear;

    [NonSerialized] public bool big = false;
    [NonSerialized] public bool canDraw = true;
    
    public float Width => big ? bigWidth : normalWidth;

    private Drawing drawing;

    private List<Drawing> drawings = new();

    public static DrawingManager instance;

    private float drawingSpeed;
    private Vector2 drawingDirection;

    private Vector2 lastMousePos;

    private float cannotBuildEffectScale;

    public static Action OnDrawingCancelled;

    private void Awake()
    {
        instance = this;
        isDrawing = false;
        cannotBuildEffect.SetActive(false);
        cannotBuildEffectScale = cannotBuildEffect.transform.localScale.x;
    }

    private void Update()
    {
        if (!canDraw) return;
        
        if (InputManager.mouseDown)
        {
            isDrawing = true;
            drawing = Instantiate(drawingPrefab);
            drawing.OnDrawingCompleted += OnDrawingCompleted;
            //fade?.Kill();
            pencilDrawingSource.time = Random.Range(0f, pencilDrawingSource.clip.length);
            pencilDrawingSource.Play();
        }
        if (Input.GetMouseButtonDown(1))
        {
            DestroyAllDrawings();
        }

        if (isDrawing && drawing.Valid)
        {
            playerVoice.Play();
            if (pencilSoundFade != null)
            {
                pencilSoundFade.Kill();
                pencilSoundFade = null;
            }
            
            float currentSpeed = Vector2.Distance(lastMousePos, InputManager.MousePosition) / Time.deltaTime;
            float speedT = 1f - Mathf.Pow(0.5f, Time.deltaTime * drawingSpeedChangeSpeed);
            drawingSpeed = Mathf.Lerp(drawingSpeed, currentSpeed, speedT);
            
            Vector2 currentDrawingDirection = (InputManager.MousePosition - lastMousePos).normalized;
            float directionT = 1f - Mathf.Pow(0.5f, Time.deltaTime * drawingDirectionChangeSpeed);
            drawingDirection = Vector2.Lerp(drawingDirection, currentDrawingDirection, directionT);

            float dot = Vector2.Dot(drawingDirection.normalized, Vector2.right);
            dot = (dot + 1f) / 2f;

            float t = Mathf.InverseLerp(drawingSpeedMin, drawingSpeedMax, drawingSpeed);
            //Debug.Log($"{t} {pencilVolume.Evaluate(t)}");
            pencilDrawingSource.volume = Mathf.Pow(pencilVolume.Evaluate(t) * pencilVolumeMultiplier, 1f - pencilVolumePower);
            pencilDrawingSource.pitch = pencilPitch.Evaluate(t) + dot * pitchOffset;
            pencilDrawingSource.panStereo = AudioManager.instance.PanFromPosition(InputManager.MousePosition);
        }
        else
        {
            if (pencilDrawingSource.volume > 0f && pencilSoundFade == null)
            {
                pencilSoundFade = pencilDrawingSource.
                    DOFade(0f, fadeDuration).OnComplete(
                    () =>
                    {
                        pencilSoundFade = null;
                        pencilDrawingSource.Stop();
                    });
            }
            
            drawingSpeed = startDrawingSpeed;
        }

        lastMousePos = InputManager.MousePosition;
    }

    private Tween pencilSoundFade;

    //private Tween fade;

    void OnDrawingCompleted(bool valid)
    {
        //fade = pencilDrawingSource.DOFade(0f, fadeDuration);
        
        isDrawing = false;
        if (valid)
        {
            drawings.Add(drawing);
            if (drawings.Count > maxNumberOfDrawings)
            {
                Clear(drawings[0]);
                drawings.RemoveAt(0);
            }
        }
        drawing.OnDrawingCompleted -= OnDrawingCompleted;
    }

    public void DestroyAllDrawings()
    {
        if (drawings.Count > 0)
        {
            OnDrawingCancelled?.Invoke();
        }
        foreach (var draw in drawings)
        {
            Clear(draw);
        }
        drawings.Clear();
    }

    void Clear(Drawing drawing)
    {
        drawing.DestructionEffect();
        Vector2 cameraPos = CameraController.instance.transform.position;
        Vector2 drawingPos = drawing.transform.position;
        clearAudio.volumeMultiplier = Mathf.Clamp01(1f - Vector2.Distance(cameraPos, drawingPos) / 20f);
        clearAudio.pitchMultiplier =
            Mathf.Lerp(pitchClearMin, pitchClearMax, drawing.totalLenght / lengthToMaxPitchClear);
        clearAudio.Play();
        Destroy(drawing.gameObject);
    }

    private Coroutine hideCannotBuildCoroutine;

    public void ShowCannotBuild(float time = 0f)
    {
        cannotBuildEffect.transform.position = InputManager.MousePosition;
        
        if (cannotBuildEffect.activeSelf) return;
        
        if (hideCannotBuildCoroutine != null) StopCoroutine(hideCannotBuildCoroutine);
        
        cannotBuildEffect.transform.localScale = cannotBuildEffectScale * Vector3.one;
        cannotBuildEffect.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        cannotBuildEffect.SetActive(true);
        
        if (time > 0f)
        {
            hideCannotBuildCoroutine = StartCoroutine(CO_HideCannotBuild(time));
        }
    }

    public void HideCannotBuild()
    {
        if (!cannotBuildEffect.activeSelf) return;
        if (hideCannotBuildCoroutine != null) StopCoroutine(hideCannotBuildCoroutine);
        cannotBuildEffect.SetActive(false);
    }
    
    

    IEnumerator CO_HideCannotBuild(float time)
    {
        yield return new WaitForSeconds(time);
        cannotBuildEffect.SetActive(false);
        hideCannotBuildCoroutine = null;
    }
    
    
}
