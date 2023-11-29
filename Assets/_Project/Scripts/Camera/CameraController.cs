using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 offset;
    [SerializeField] private float speed;
    [SerializeField] private Player player;
    [SerializeField] private SpriteRenderer staticOverlay;

    public static Vector2 Position { get; private set; }

    public static CameraController instance;

    private void Awake()
    {
        instance = this;
        staticOverlay.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        float t = 1f - Mathf.Pow(0.5f, speed * Time.deltaTime);
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, player.transform.position + (Vector3) offset, t);
        pos.z = transform.position.z;
        transform.position = pos;
        Position = pos;
    }

    public void ActivateStaticOverlay()
    {
        SetStaticOverlay(0f);
        staticOverlay.gameObject.SetActive(true);
    }

    public void SetStaticOverlay(float value)
    {
        staticOverlay.color = new Color(1f, 1f, 1f, value);
    }
}
