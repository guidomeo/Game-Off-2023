using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float distance = 20f;
    [SerializeField] private float yMultiplier = 0.3f;

    private float t = 0f;
    private void Update()
    {
        float increment = speed * Time.deltaTime;
        t += increment;
        if (t > distance)
        {
            t -= distance;
            transform.position -= Vector3.left * distance;
        }
        transform.position += Vector3.left * increment;


        Vector3 pos = transform.localPosition;

        pos.y = -Mathf.Repeat(transform.parent.position.y * yMultiplier, distance);
        
        transform.localPosition = pos;
    }
}
