using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 offset;
    [SerializeField] private float speed;
    [SerializeField] private Player player;

    private void LateUpdate()
    {
        float t = 1f - Mathf.Pow(0.5f, speed * Time.deltaTime);
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, player.transform.position + (Vector3) offset, t);
        pos.z = transform.position.z;
        transform.position = pos;
    }
}
