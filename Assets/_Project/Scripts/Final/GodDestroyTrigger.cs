using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodDestroyTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        
        BackgroundAudioPlayer.instance.Play("Soundtrack Snow");
        BackgroundAudioPlayer.instance.Play("Ambient Ground");
        BackgroundAudioPlayer.instance.Stop("Final Noise");
        Destroy(PencilGod.instance.gameObject);
        Destroy(gameObject);
    }
}
