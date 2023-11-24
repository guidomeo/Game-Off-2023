using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicCharacterController : MonoBehaviour
{
    [Header("Movement settings")]
    public float acceleration = 50f;
    public float brakingForce = 100f;
    [Min(1f)] public float changeDirectionAccMultiplier = 3f;
    public float maxSpeed = 10f;
    public float gravity = 30f;
    [Header("Floor settings")]
    public LayerMask wallMask;
    public float raycastDistance = 0.5f;
    [Header("Angles")]
    [Range(0f, 90f)] public float maxGoDownAngle = 70f;
    [Range(0f, 90f)] public float angleMaxSpeed = 30f;
    [Range(0f, 90f)] public float angleZeroSpeed = 80f;
    [Range(0f, 90f)] public float angleFriction = 45f;
    
    
    
    private int moveDir;

    private Rigidbody2D rb;

    private Vector2 velocity;

    private Vector2 gravityDir = Vector2.down;
    private Vector2 rightDir = Vector2.right;
    private float angle;

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
            
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDir = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDir = 1;
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
        angle = 0f;
        
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.down, raycastDistance, wallMask);
        if (hit.collider == null) return;
        
        Vector2 dir = (hit.point - hit.centroid).normalized;
        angle = Vector2.SignedAngle(Vector2.down, dir);
        onGround = true;
        gravityDir = dir;
        rightDir = Vector2.Perpendicular(dir);
    }

    void Move()
    {
        /*if (onGround)
        {
            float frictionSign = -Mathf.Sign(currentGroundSpeed);
            rb.AddForce(rightDir * (brakingForce * frictionSign));
        }*/
        
        MovementVelocity = Vector3.Dot(rb.velocity, rightDir);
        Debug.Log(angle);

        if (Mathf.Abs(angle) > angleMaxSpeed)
        {
            rightDir = Vector2.right;
            gravityDir = Vector2.down;
        }
        rb.AddForce(gravity * gravityDir);
        
        if (moveDir == 0)
        {
            if (Mathf.Abs(angle) < angleFriction)
            {
                int brakingDir = (int) -Mathf.Sign(MovementVelocity);
                float currBrakingForce = Mathf.Min(brakingForce, Mathf.Abs(MovementVelocity) / Time.deltaTime);
                rb.AddForce(rightDir * (currBrakingForce * brakingDir));
            }
        }
        else
        {
            float currentMaxSpeed = maxSpeed;
            //float angleT = Mathf.InverseLerp(angleZeroSpeed, angleMaxSpeed, angle);
            //Debug.Log(angleT);
            //currentMaxSpeed *= angleT;
            
            float currAcceleration = acceleration;
            if (moveDir == 1 && (MovementVelocity >= currentMaxSpeed || angle > angleZeroSpeed)) currAcceleration = 0f;
            if (moveDir == -1 && (MovementVelocity <= -currentMaxSpeed || angle < -angleZeroSpeed)) currAcceleration = 0f;
            if (Dir(MovementVelocity) != Dir(moveDir)) currAcceleration *= changeDirectionAccMultiplier;
            rb.AddForce(rightDir * (currAcceleration * moveDir));
        }

    }

    bool Dir(float n)
    {
        return n > 0f;
    }

    float ClampMagnitude(float value, float maxValue)
    {
        maxValue = Mathf.Abs(maxValue);
        return Mathf.Clamp(value, -maxValue, maxValue);
    }
}
