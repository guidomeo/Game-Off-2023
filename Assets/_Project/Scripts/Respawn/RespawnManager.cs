using System;
using System.Collections.Generic;
using UnityEngine;


public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Player player;

    private static SortedList<float, Vector3> respawnPoints;
    
    private static Player s_player;

    private static Vector3 lastRespawnPoint;

    private int index = 0;
    private void Awake()
    {
        s_player = player;
        respawnPoints = new();
        
        var playerPos = s_player.transform.position;
        SetRespawnPoint(playerPos);
        RegisterCheckpoint(playerPos);
    }

    private void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown($"{i}"))
            {
                index = index * 10 + i;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var positions = respawnPoints.Values;
            if (index >= 0 && index < positions.Count)
            {
                RespawnAt(positions[index]);
            }
            index = 0;
        }
    }

    public static void SetRespawnPoint(Vector3 pos)
    {
        lastRespawnPoint = pos;
    }

    public static void Respawn()
    {
        RespawnAt(lastRespawnPoint);
    }
    
    static void RespawnAt(Vector3 position)
    {
        s_player.rb.velocity = Vector2.zero;
        s_player.rb.position = position;
    }

    public static void RegisterCheckpoint(Vector3 respawnPoint)
    {
        respawnPoints.Add(respawnPoint.x, respawnPoint);
    }
}
