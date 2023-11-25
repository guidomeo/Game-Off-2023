using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private bool open;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private RectTransform panel;
    [SerializeField] private Vector2 movement;
    [SerializeField] private AudioSettingsButton button;

    private Vector2 startPanelPos;
    private Vector2 PanelPos => startPanelPos + (button.IsOn ? Vector2.zero : movement);
    public void Awake()
    {
        startPanelPos = panel.anchoredPosition;
        button.SetIsOn(open, false);
        button.onClick += OnClick;
    }

    private void OnDestroy()
    {
        button.onClick -= OnClick;
    }

    private void Start()
    {
        panel.anchoredPosition = PanelPos;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!MouseOverAudioSettings())
            {
                if (button.IsOn)
                {
                    button.SetIsOn(false, true);
                    OnClick();
                }
            }
        }
    }

    void OnClick()
    {
        panel.DOAnchorPos(PanelPos, animationDuration);
    }
    
    bool MouseOverAudioSettings()
    {
        return GetEventSystemRaycastResults().Any(
                curRaycastResult => curRaycastResult.gameObject.GetComponentInParent<AudioSettings>()
            );
    }
    List<RaycastResult> GetEventSystemRaycastResults()
    {   
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventData, raycastResults );

        return raycastResults;
    }
}
