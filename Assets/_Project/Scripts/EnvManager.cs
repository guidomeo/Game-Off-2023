using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvManager : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayerMask;

    public static EnvManager instance;
    private void Awake()
    {
        instance = this;
    }

    public static bool PointInCave(Vector2 point)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, instance.wallLayerMask);
        return (coll != null);
    }
}
