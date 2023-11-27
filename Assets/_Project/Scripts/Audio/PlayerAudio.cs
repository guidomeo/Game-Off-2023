using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioDataGroup audioGroundStep;
    [SerializeField] private AudioDataGroup audioLineStep;

    private AudioDataGroup audioStep;
    
    private Player player;

    private bool inCave;
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

            inCave = EnvManager.PointInCave(transform.position);
        }
    }

    public void Step()
    {
        if (audioStep == null) return;
        audioStep.Play(inCave ? 1 : 0);
    }
}
