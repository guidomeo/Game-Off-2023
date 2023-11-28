using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;

        player.cc.maxSpeed = 2f;
        player.cc.moveOnlyRight = true;
    }
}
