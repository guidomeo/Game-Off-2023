using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicCharacterController : MonoBehaviour
{
    [Header("Movement settings")]
    public float acceleration = 50f;
    public float brakingForce = 100f;
    public float maxSpeed = 10f;
    public float gravity = 30f;
    [Header("Float settings")]
    public float raycastDistance = 0.5f;
    [Range(0f, 90f)] public float maxAngle = 45f;
    [Min(1f)] public float slopePower = 1f;
    public LayerMask wallMask;
    private int moveDir;

    private Rigidbody2D rb;

    private Vector2 velocity;

    private Vector2 gravityDir = Vector2.down;
    private Vector2 rightDir = Vector2.right;

    private bool onGround;

    public float MovementVelocity { get; private set; }

    private float currentMaxSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDir = 0;

        if (DrawingManager.isDrawing) return;
            
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDir -= 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDir += 1;
    }

    private void FixedUpdate()
    {
        CalculateTerrain();
        Move();
    }

    void CalculateTerrain()
    {
        onGround = false;
        currentMaxSpeed = maxSpeed;
        gravityDir = Vector2.down;
        rightDir = Vector2.right;
        
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.down, raycastDistance, wallMask);
        if (hit.collider == null) return;
        
        Vector2 dir = (hit.point - hit.centroid).normalized;
        rightDir = Vector2.Perpendicular(dir);
        float angle = Vector2.Angle(Vector2.down, dir);
        if (angle > maxAngle)
        {
            if (Mathf.Sign(dir.x) == Mathf.Sign(moveDir))
            {
                float angleDiff = angle - maxAngle;
                float maxAngleDiff = 90f - maxAngle;
                float angleT = Mathf.Pow(1f - angleDiff / maxAngleDiff, slopePower);
                currentMaxSpeed = angleT * maxSpeed;
            }
            //if (dir.x > 0 && moveDir == 1) moveDir = 0;
            //if (dir.x < 0 && moveDir == -1) moveDir = 0;
            return;
        }
        
        onGround = true;
        gravityDir = dir;
    }

    void Move()
    {
        float currentVelocity = Vector3.Dot(rb.velocity, rightDir);
        float targetVelocity = moveDir * currentMaxSpeed;
        
        float accelerationValue = acceleration;
        if (moveDir == 0f || currentVelocity > currentMaxSpeed)
        {
            accelerationValue = brakingForce;
        }
        
        float velocityDiff = targetVelocity - currentVelocity;
        float currAcceleration = accelerationValue * Mathf.Sign(velocityDiff);
        currAcceleration = ClampMagnitude(currAcceleration, velocityDiff / Time.deltaTime);

        MovementVelocity = currentVelocity + currAcceleration * Time.deltaTime;

        rb.AddForce(currAcceleration * rightDir);
        rb.AddForce(gravity * gravityDir);
    }

    float ClampMagnitude(float value, float maxValue)
    {
        maxValue = Mathf.Abs(maxValue);
        return Mathf.Clamp(value, -maxValue, maxValue);
    }
}
