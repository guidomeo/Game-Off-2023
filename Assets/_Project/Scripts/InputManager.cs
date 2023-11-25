using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static Vector2 MousePosition { get; private set; }
    
    Camera cam;

    private Plane plane = new (Vector3.forward, 0f);

    public static bool mouseDown;
    public static bool mouseUp;

    private bool isClicking;
    private bool calledMouseDown;
    
    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        mouseDown = false;
        mouseUp = false;
        if (Input.GetMouseButton(0))
        {
            if (!isClicking)
            {
                isClicking = true;
                if (!OverUI())
                {
                    mouseDown = true;
                    calledMouseDown = true;
                }
            }
        }
        else
        {
            if (isClicking)
            {
                isClicking = false;
                if (calledMouseDown)
                {
                    calledMouseDown = false;
                    mouseUp = true;
                }
            }
        }
        
        Vector2 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, 0f, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0f, Screen.height);
        Ray ray = cam.ScreenPointToRay(mousePos);
        if (!plane.Raycast(ray, out float enter)) return;
        MousePosition = ray.GetPoint(enter);
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
