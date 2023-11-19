using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] private Transform respawnTr;
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        rb.velocity = Vector2.zero;
        rb.position = respawnTr.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(respawnTr.position, 0.5f);
    }
}
