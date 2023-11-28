using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameFinalTrigger : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 3f;

    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;

        if (triggered) return;
        
        triggered = true;

        DOTween.To(
            () => player.cc.maxSpeed,
            s => player.cc.maxSpeed = s,
            playerSpeed,
            2f
            );
        player.cc.moveOnlyRight = true;
    }
}
