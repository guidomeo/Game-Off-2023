using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBounds : MonoBehaviour
{
    [SerializeField] private float aspectW = 16f;
    [SerializeField] private float aspectH = 9f;
    
    Camera cam;
    private void Start()
    {
        if (!Application.isPlaying) return;
        
        ChangeAspect();
        Destroy(this);
    }

    private void Update()
    {
        ChangeAspect();
    }

    void ChangeAspect()
    {
        if (cam == null) cam = GetComponent<Camera>();
        
        float aspect = aspectW / aspectH;
        
        cam.rect = new Rect(0f, 0f, 1f, 1f);
        
        var variance = aspect / cam.aspect;
        if (variance < 1f)
        {
            cam.rect = new Rect ((1f - variance) / 2f, 0 , variance, 1f);
        }
        else
        {
            variance = 1f / variance;
            cam.rect = new Rect (0, (1f - variance) / 2f , 1f, variance);
        }
    }
}
