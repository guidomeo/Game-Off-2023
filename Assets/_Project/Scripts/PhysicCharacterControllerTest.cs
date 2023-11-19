using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysicCharacterControllerTest : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] [Range(0, 1)] private float speedFactor;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private AnimationCurve accelerationFactorFromDot;
    [SerializeField] private float maxAccelerationForce;
    [SerializeField] private AnimationCurve maxAccelerationFactorFromDot;
    [SerializeField] private float gravity;
    [Header("Float settings")]
    [SerializeField] private float raycastDistance;
    [SerializeField] private float floatHeight;
    [SerializeField] private float maxAngle;
    [SerializeField] private float springStrength;
    [SerializeField] private float springDamper;
    [SerializeField] private LayerMask wallMask;
    private float moveDir;

    private Rigidbody2D rb;

    private Vector2 velocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        Float();
        Move();
    }

    void Float()
    {
        Vector2 rayDir = Vector2.down;
        
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, rayDir, raycastDistance, wallMask);

        if (hit.collider == null) return;
        
        Vector2 dir = (hit.point - hit.centroid).normalized;
        if (Vector2.Angle(Vector2.down, dir) > maxAngle) return;
        
        Vector2 vel = rb.velocity;

        Vector2 otherVel = Vector2.zero;
        Rigidbody2D hitBody = hit.rigidbody;
        if (hitBody != null)
        {
            otherVel = hitBody.velocity;
        }

        float rayDirVel = Vector2.Dot(rayDir, vel);
        float otherRayDirVel = Vector2.Dot(rayDir, otherVel);

        float relVel = rayDirVel - otherRayDirVel;

        float x = hit.distance - floatHeight;

        float springForce = x * springStrength - relVel * springDamper;
        
        rb.AddForce(rayDir * springForce);

        if (hitBody != null)
        {
            hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
        }
    }

    void Move()
    {
        Vector2 goalVelocity = Vector2.right * (moveDir * speedFactor * maxSpeed);
        
        /*Vector2 velocityDir = velocity.normalized;

        float velDot = Vector2.Dot(velocityDir, goalVelocity);

        float currAcceleration = acceleration * accelerationFactorFromDot.Evaluate(velDot);

        velocity = Vector2.MoveTowards(velocity, goalVelocity, currAcceleration * Time.deltaTime);

        Vector2 neededAcceleration = (velocity - rb.velocity) / Time.deltaTime;

        float currMaxAcceleration = maxAccelerationForce * maxAccelerationFactorFromDot.Evaluate(velDot);

        neededAcceleration = Vector2.ClampMagnitude(neededAcceleration, currMaxAcceleration);
        
        rb.AddForce(neededAcceleration);

        rb.AddForce(gravity * Vector2.down);*/

        rb.AddForce(goalVelocity);
    }
}
