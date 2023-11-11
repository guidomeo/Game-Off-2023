using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private Drawing drawingPrefab;
    
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(drawingPrefab);
        }
    }
}
