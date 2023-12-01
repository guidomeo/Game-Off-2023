using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private int index;

    private void Start()
    {
        if (CollectableCanvas.instance.IsUnlocked(index)) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        
        CollectableCanvas.instance.Collect(index);
        Destroy(gameObject);
    }
}
