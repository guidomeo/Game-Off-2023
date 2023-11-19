using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicCharacterController : MonoBehaviour
{
    [Header("Movement settings")]
    public float acceleration;
    public float brakingForce;
    public float maxSpeed;
    public float gravity;
    [Header("Float settings")]
    public float raycastDistance;
    public float maxAngle;
    public LayerMask wallMask;
    private int moveDir;

    private Rigidbody2D rb;

    private Vector2 velocity;

    private Vector2 gravityDir = Vector2.down;
    private Vector2 rightDir = Vector2.right;

    private bool onGround;

    public float MovementVelocity { get; private set; }

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
        gravityDir = Vector2.down;
        rightDir = Vector2.right;
        
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.down, raycastDistance, wallMask);
        if (hit.collider == null) return;
        
        Vector2 dir = (hit.point - hit.centroid).normalized;
        if (Vector2.Angle(Vector2.down, dir) > maxAngle)
        {
            if (dir.x > 0 && moveDir == 1) moveDir = 0;
            if (dir.x < 0 && moveDir == -1) moveDir = 0;
            return;
        }
        
        onGround = true;
        gravityDir = dir;
        rightDir = Vector2.Perpendicular(gravityDir);
    }

    void Move()
    {
        float currentVelocity = Vector3.Dot(rb.velocity, rightDir);
        float targetVelocity = moveDir * maxSpeed;
        
        float accelerationValue = acceleration;
        if (moveDir == 0f || currentVelocity > maxSpeed)
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
