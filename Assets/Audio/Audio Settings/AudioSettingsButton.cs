using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioSettingsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private float hoverAngle;
    [SerializeField] private float downAngle;
    [SerializeField] private float stateChangeAngle;
    [SerializeField] private Transform graphic;
    [SerializeField] private AudioData enter;
    [SerializeField] private AudioData exit;
    [SerializeField] private AudioData down;
    [SerializeField] private AudioData click;

    private bool mouseOver;
    private bool mouseDown;

    public Action onClick;

    private Quaternion TargetRotation
    {
        get
        {
            float angle = 0f;
            float sign = isOn ? 1f : -1f;
            if (isOn) angle += stateChangeAngle;
            if (mouseOver) angle += hoverAngle * sign;
            if (mouseDown) angle += downAngle * sign;
            
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
    }
    
    private bool isOn;
    public bool IsOn => isOn;
    public void SetIsOn(bool value, bool animate)
    {
        isOn = value;
        if (animate)
            ChangeAngleAnim();
        else
            ChangeAngleNoAnim();
    }
    
    void ChangeAngleNoAnim()
    {
        graphic.rotation = TargetRotation;
    }

    void ChangeAngleAnim()
    {
        graphic.DORotateQuaternion(TargetRotation, 0.5f);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        ChangeAngleAnim();
        enter.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ChangeAngleAnim();
        exit.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        mouseDown = true;
        ChangeAngleAnim();
        down.Play();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        mouseDown = false;
        ChangeAngleAnim();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        click.Play();
        isOn = !isOn;
        ChangeAngleAnim();
        onClick?.Invoke();
    }
}
