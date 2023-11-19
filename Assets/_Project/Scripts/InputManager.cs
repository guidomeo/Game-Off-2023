using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static Vector2 MousePosition { get; private set; }
    
    Camera cam;

    private Plane plane = new (Vector3.forward, 0f);
    
    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, 0f, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0f, Screen.height);
        Ray ray = cam.ScreenPointToRay(mousePos);
        if (!plane.Raycast(ray, out float enter)) return;
        MousePosition = ray.GetPoint(enter);
    }
}
