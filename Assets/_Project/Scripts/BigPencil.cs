using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPencil : MonoBehaviour
{
    [SerializeField] private float animSpeed;
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;

        player.PickBigPencil(this, animSpeed);

        GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        
        Destroy(this);
    }
}
