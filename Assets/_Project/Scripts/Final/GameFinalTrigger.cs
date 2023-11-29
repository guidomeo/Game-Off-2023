using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameFinalTrigger : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 3f;
    [SerializeField] private bool dropPencil;
    [SerializeField] private Vector2 dropForce;

    private bool triggered = false;
    
    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;

        if (triggered) return;
        
        if (dropPencil) player.DropPencil(dropForce);
        
        triggered = true;

        DOTween.To(
            () => player.cc.maxSpeed,
            s => player.cc.maxSpeed = s,
            playerSpeed,
            1f
            );
        player.cc.moveOnlyRight = true;

        DrawingManager.instance.canDraw = false;
        DrawingManager.instance.DestroyAllDrawings();
    }
}
