using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioData audioGroundStep;
    [SerializeField] private AudioData audioLineStep;

    private AudioData audioStep;
    
    private Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        audioStep = null;
        if (player.cc.onGround)
        {
            bool isDrawing = false;
            Rigidbody2D rb = player.cc.hit.rigidbody;
            if (rb != null && rb.GetComponent<Drawing>() != null) isDrawing = true;
            audioStep = isDrawing ? audioLineStep : audioGroundStep;
            
            audioStep.volumeMultiplier = player.NormalizedSpeed;
        }
    }

    public void Step()
    {
        if (audioStep == null) return;
        audioStep.Play();
    }
}
