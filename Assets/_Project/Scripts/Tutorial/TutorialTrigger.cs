using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private float delay;

    private void Awake()
    {
        Destroy(GetComponent<SpriteRenderer>());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        
        TutorialCanvas.instance.TutorialTrigger(index, true, delay);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        if (player.transform.position.x < transform.position.x) return;
        TutorialCanvas.instance.TutorialTrigger(index, false);
        Destroy(gameObject);
    }
}
