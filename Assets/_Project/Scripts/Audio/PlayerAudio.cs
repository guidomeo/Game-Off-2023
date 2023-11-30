using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioDataGroup audioGroundStep;
    [SerializeField] private AudioDataGroup audioSnowStep;
    [SerializeField] private AudioDataGroup audioLineStep;
    [SerializeField] private float inCaveAmbientMultiplier = 0.5f;
    [SerializeField] private float snowX;

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
            audioStep = isDrawing ? audioLineStep : (transform.position.x > snowX ? audioSnowStep : audioGroundStep);
            
            audioStep.volumeMultiplier = player.NormalizedSpeed;

            bool isInCave = EnvManager.PointInCave(transform.position);
            if (isInCave != inCave)
            {
                inCave = isInCave;
                BackgroundAudioPlayer.instance.ChangeVolume(
                    "Ambient Ground",
                    inCave ? inCaveAmbientMultiplier : 2f,
                    1f
                    );
            }
        }
    }

    public void Step()
    {
        if (audioStep == null) return;
        audioStep.Play(inCave ? 1 : 0);
    }
}
