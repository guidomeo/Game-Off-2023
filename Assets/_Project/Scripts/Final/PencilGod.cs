using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PencilGod : MonoBehaviour
{
    [SerializeField] private float deathDistance = 3;
    [SerializeField] private float overlayDistance = 15;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float rangeMaxForce;
    [SerializeField] private float horizon;
    [SerializeField] private float minForce;
    [SerializeField] private Player player;

    public static PencilGod instance;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

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
        
        float distance = Mathf.Max(0f, player.transform.position.x - xPos);
        if (distance < rangeMaxForce)
        {
            float t = Mathf.Clamp01(distance / range);
            player.cc.moveDirAdd = Mathf.Lerp(2f, minForce, t);
        }
        else if (distance < range)
        {
            player.cc.moveDirAdd = minForce;
        }
        else
        {
            player.cc.moveDirAdd = 0f;
        }

        float trueDistance = player.transform.position.x - (transform.position.x + deathDistance);
        if (trueDistance < 0f)
        {
            gameOverTime += Time.deltaTime;
            if (gameOverTime > 2f)
            {
                SceneManager.LoadScene(0);
            }
            return;
        }
        CameraController.instance.SetStaticOverlay(1f - trueDistance / overlayDistance);
    }

    private float gameOverTime = 0f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            transform.position + horizon * Vector3.right + Vector3.up * 99f,
            transform.position + horizon * Vector3.right + Vector3.down * 99f
            );
        Gizmos.DrawLine(
            transform.position + (horizon + rangeMaxForce) * Vector3.right + Vector3.up * 99f,
            transform.position + (horizon + rangeMaxForce) * Vector3.right + Vector3.down * 99f
        );
        Gizmos.DrawLine(
            transform.position + (horizon + range) * Vector3.right + Vector3.up * 99f,
            transform.position + (horizon + range) * Vector3.right + Vector3.down * 99f
        );
    }
}
