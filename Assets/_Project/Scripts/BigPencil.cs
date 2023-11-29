using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BigPencil : MonoBehaviour
{
    [SerializeField] private float animSpeed;
    [SerializeField] private SpriteRenderer glow;

    private void Awake()
    {
        glow.color = new Color(1f,1f,1f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;

        Transform parent = transform.parent;

        player.cc.canMove = false;
        
        glow.DOFade(1f, 2f).OnComplete(() =>
        {
            player.PickBigPencil(parent, animSpeed);
            parent.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
            
            glow.DOFade(0f, 2f).SetDelay(3.5f);
        });
        
        Destroy(gameObject);
    }
}
