using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilGod : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float horizon;
    [SerializeField] private float force;
    [SerializeField] private Player player;

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        pos.y = player.transform.position.y;
        transform.position = pos;
    }

    private void FixedUpdate()
    {
        float xPos = transform.position.x + horizon;
        
        float distance = Mathf.Abs(player.transform.position.x - xPos);
        if (distance < range)
        {
            float t = Mathf.Clamp01(distance / range);

            player.cc.moveDirAdd = Mathf.Lerp(2f, 0f,t);
        }
        else
        {
            player.cc.moveDirAdd = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            transform.position + horizon * Vector3.right + Vector3.up * 99f,
            transform.position + horizon * Vector3.right + Vector3.down * 99f
            );
        Gizmos.DrawLine(
            transform.position + (horizon + range) * Vector3.right + Vector3.up * 99f,
            transform.position + (horizon + range) * Vector3.right + Vector3.down * 99f
        );
    }
}
