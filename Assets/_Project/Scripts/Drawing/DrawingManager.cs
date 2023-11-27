using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrawingManager : MonoBehaviour
{
    public static bool isDrawing;
    
    [SerializeField] private int maxNumberOfDrawings = 3;
    [SerializeField] private Drawing drawingPrefab;
    [SerializeField] private GameObject cannotBuildEffect;
    [Header("Pencil Sound")]
    [SerializeField] private float startDrawingSpeed;
    [SerializeField] private float fadeSpeed = 0.1f;
    [SerializeField] private float pitchOffset;
    [SerializeField] private float drawingSpeedChangeSpeed;
    [SerializeField] private float drawingDirectionChangeSpeed;
    
    [SerializeField] private float drawingSpeedMin;
    [SerializeField] private float drawingSpeedMax;
    [SerializeField] [Range(0f, 1f)] private float pencilVolumeMultiplier;
    [SerializeField] private AnimationCurve pencilVolume;
    [SerializeField] private AnimationCurve pencilPitch;
    
    [SerializeField] private AudioSource pencilDrawingSource;

    private Drawing drawing;

    private List<Drawing> drawings = new();

    public static DrawingManager instance;

    private float drawingSpeed;
    private Vector2 drawingDirection;

    private Vector2 lastMousePos;

    private void Awake()
    {
        instance = this;
        isDrawing = false;
        cannotBuildEffect.SetActive(false);
    }

    private void Update()
    {
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
            pencilDrawingSource.volume = pencilVolume.Evaluate(t) * pencilVolumeMultiplier;
            pencilDrawingSource.pitch = pencilPitch.Evaluate(t) + dot * pitchOffset;
            pencilDrawingSource.panStereo = AudioManager.instance.PanFromPosition(InputManager.MousePosition);
        }
        else
        {
            if (pencilDrawingSource.volume > 0f)
            {
                pencilDrawingSource.volume -= fadeSpeed * Time.deltaTime;
                if (pencilDrawingSource.volume < 0f)
                {
                    pencilDrawingSource.Stop();
                }
            }
            
            drawingSpeed = startDrawingSpeed;
        }

        lastMousePos = InputManager.MousePosition;
    }

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
                Destroy(drawings[0].gameObject);
                drawings.RemoveAt(0);
            }
        }
        drawing.OnDrawingCompleted -= OnDrawingCompleted;
    }

    public void DestroyAllDrawings()
    {
        foreach (var draw in drawings)
        {
            Destroy(draw.gameObject);
        }
        drawings.Clear();
    }

    private Coroutine hideCannotBuildCoroutine;

    public void ShowCannotBuild(float time = 0f)
    {
        if (hideCannotBuildCoroutine != null) StopCoroutine(hideCannotBuildCoroutine);
        
        cannotBuildEffect.transform.position = InputManager.MousePosition;
        cannotBuildEffect.SetActive(true);
        
        if (time > 0f)
        {
            hideCannotBuildCoroutine = StartCoroutine(CO_HideCannotBuild(time));
        }
    }

    public void HideCannotBuild()
    {
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
