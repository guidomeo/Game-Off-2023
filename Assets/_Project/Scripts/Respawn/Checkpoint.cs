using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer rend;
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.enabled = false;
    }

    private void Start()
    {
        RespawnManager.RegisterCheckpoint(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        RespawnManager.SetRespawnPoint(transform.position);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
