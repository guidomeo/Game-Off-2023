using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCameraBounds : MonoBehaviour
{
    private RectTransform rectTr;
    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTr.anchorMin = CameraBounds.rect.min;
        rectTr.anchorMax = CameraBounds.rect.max;
    }
}
