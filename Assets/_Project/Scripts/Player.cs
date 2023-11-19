using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveAcc = 300f;
    [SerializeField] private float friction = 10f;
    [SerializeField] private float maxXSpeed = 10f;
    [SerializeField] private float maxYUpSpeed = 10f;
    [SerializeField] private float distanceToCheck = 0.5f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Transform graphic;
    [SerializeField] private float speedToChangeSpeedX;

    private float moveDir = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    
    Vector2 gravityDir;
    private Vector2 rayDir;
    private RaycastHit2D hit;
    private Vector2 rightDir;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private float speedX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        moveDir = 0f;
        if (DrawingManager.isDrawing) return;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDir -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDir += 1f;
    }

    private void FixedUpdate()
    {
        Vector2 targetRightDir = Vector2.right;
        gravityDir = Vector2.down;
        rightDir = Vector2.right;
        rayDir = new Vector2(moveDir, -1f);
        rayDir.Normalize();
        hit = Physics2D.CircleCast(transform.position, 0.5f, rayDir, distanceToCheck, wallMask);
        if (hit.collider != null)
        {
            Vector2 targetGravityDir = (hit.point - hit.centroid).normalized;
            targetRightDir = Vector2.Perpendicular(targetGravityDir);
            if (Vector2.Angle(Vector2.down, targetGravityDir) <= maxAngle)
            {
                gravityDir = targetGravityDir;
                rightDir = targetRightDir;
            }
        }
        
        Vector2 movement = rightDir * (moveDir * moveAcc * Time.fixedDeltaTime);
        
        if (hit.collider != null)
        {
            if (Vector3.Dot(movement, targetRightDir) > 0)
            {
                movement = SubtractVelocity(movement, -targetRightDir * (friction * Time.fixedDeltaTime));
            }
            else
            {
                movement = SubtractVelocity(movement, targetRightDir * (friction * Time.fixedDeltaTime));
            }
        }

        
        Vector2 velocity = rb.velocity;

        velocity += gravityDir * (gravity * Time.fixedDeltaTime);
        velocity += movement;
        velocity.x = Mathf.Clamp(velocity.x, -maxXSpeed, maxXSpeed);
        velocity.y = Mathf.Min(velocity.y, maxYUpSpeed);
        rb.velocity = velocity;

        float t = 1f - Mathf.Pow(0.5f, speedToChangeSpeedX * Time.deltaTime);
        speedX = Mathf.Lerp(speedX, rb.velocity.x, t);
        
        animator.SetFloat(Speed, Mathf.Abs(speedX) / maxXSpeed);
        graphic.localScale = new Vector3(Mathf.Sign(speedX), 1f, 1f);
    }

    Vector2 SubtractVelocity(Vector2 a, Vector2 b)
    {
        Vector2 project = Vector3.Project(a, b);
        float projectMag = project.magnitude;
        b = Vector2.ClampMagnitude(b, projectMag);
        a += b;
        return a;
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + (Vector3) gravityDir);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + (Vector3) rightDir);
        
        Gizmos.color = new Color(0f, 0f, 0f, 0.5f);
        
        Gizmos.DrawWireSphere(pos, 0.5f);
        Gizmos.DrawWireSphere(pos + (Vector3) rayDir * distanceToCheck, 0.5f);
        
        if (hit.collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.centroid, 0.5f);
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
    }
}
