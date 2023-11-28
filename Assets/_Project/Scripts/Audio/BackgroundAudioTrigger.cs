using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudioTrigger : MonoBehaviour
{
    [SerializeField] private List<string> backgroundNamesToPlay;
    [SerializeField] private List<string> backgroundNamesToStop;

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

        foreach (var backName in backgroundNamesToPlay)
        {
            BackgroundAudioPlayer.instance.Play(backName);
        }
        foreach (var backName in backgroundNamesToStop)
        {
            BackgroundAudioPlayer.instance.Stop(backName);
        }
    }
}
