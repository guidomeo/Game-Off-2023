using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodDestroyTrigger : MonoBehaviour
{
    private bool used = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (used) return;
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        used = true;
        
        BackgroundAudioPlayer.instance.Play("Soundtrack Snow");
        BackgroundAudioPlayer.instance.Play("Ambient Ground");
        BackgroundAudioPlayer.instance.Stop("Final Noise");
        Destroy(PencilGod.instance.gameObject);
        Destroy(gameObject);
    }
}
