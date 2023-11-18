using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public static bool isDrawing;
    
    [SerializeField] private int maxNumberOfDrawings = 3;
    [SerializeField] private Drawing drawingPrefab;

    private Drawing drawing;

    private List<Drawing> drawings = new();

    private void Awake()
    {
        isDrawing = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            drawing = Instantiate(drawingPrefab);
            drawing.OnDrawingCompleted += OnDrawingCompleted;
        }
    }

    void OnDrawingCompleted(bool valid)
    {
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
}
