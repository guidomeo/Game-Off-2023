using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveAcc = 300f;
    [SerializeField] private float maxXSpeed = 10f;
    [SerializeField] private float maxYUpSpeed = 10f;
    [SerializeField] private float distanceUp = 0.5f;
    [SerializeField] private float distanceToCheck = 0.5f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private LayerMask wallMask;

    private float moveDir = 0f;

    private Rigidbody2D rb;
    
    Vector2 gravityDir;
    private Vector2 rayDir;
    private RaycastHit2D hit;
    private Vector2 rightDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDir = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDir -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDir += 1f;
    }

    private void FixedUpdate()
    {
        gravityDir = Vector2.down;
        rayDir = new Vector2(moveDir, -1f);
        rayDir.Normalize();
        hit = Physics2D.CircleCast((Vector2) transform.position + Vector2.up * distanceUp, 0.5f, rayDir, distanceToCheck, wallMask);
        if (hit.collider != null)
        {
            Vector2 targetGravityDir = (hit.point - hit.centroid).normalized;
            if (Vector2.Angle(Vector2.down, targetGravityDir) <= maxAngle)
            {
                gravityDir = targetGravityDir;
            }
        }

        rightDir = new Vector2(-gravityDir.y, gravityDir.x);
        
        Vector2 velocity = rb.velocity;

        velocity += gravityDir * (gravity * Time.fixedDeltaTime);
        velocity += rightDir * (moveDir * moveAcc * Time.fixedDeltaTime);
        velocity.x = Mathf.Clamp(velocity.x, -maxXSpeed, maxXSpeed);
        velocity.y = Mathf.Min(velocity.y, maxYUpSpeed);
        rb.velocity = velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3) gravityDir);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3) rightDir);
        
        Gizmos.color = new Color(0f, 0f, 0f, 0.5f);
        Vector2 origin = (Vector2)transform.position + Vector2.up * distanceUp;
        Gizmos.DrawWireSphere(origin, 0.5f);
        Gizmos.DrawWireSphere(origin + rayDir * distanceToCheck, 0.5f);
        
        if (hit.collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.centroid, 0.5f);
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
    }
}
