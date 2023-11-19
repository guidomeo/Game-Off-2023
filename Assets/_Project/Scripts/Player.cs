using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform graphic;
    [SerializeField] private float speedToChangeSpeedX;
    [SerializeField] private Transform handLeft;

    private float moveDir = 0f;

    private Camera cam;
    private Rigidbody2D rb;
    private Animator animator;
    private PhysicCharacterController cc;
    
    Vector2 gravityDir;
    private Vector2 rayDir;
    private RaycastHit2D hit;
    private Vector2 rightDir;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Cast = Animator.StringToHash("Cast");

    private int currenDir = 1;
    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        cc = GetComponent<PhysicCharacterController>();
    }

    private void Update()
    {
        float flip = 0;
        

        if (DrawingManager.isDrawing)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = pos - (Vector2)handLeft.position;
            flip = Mathf.Sign(dir.x);
            float angle = Vector2.SignedAngle(Vector2.right, dir);
            if (flip < 0) angle = 180 - angle;
            handLeft.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            if (cc.MovementVelocity < -0.1f) currenDir = -1;
            if (cc.MovementVelocity > 0.1f) currenDir = 1;
            flip = currenDir;
            handLeft.localRotation = Quaternion.identity;
        }
        
        animator.SetFloat(Speed, Mathf.Abs(cc.MovementVelocity) / cc.maxSpeed);
        graphic.localScale = new Vector3(flip, 1f, 1f);
        animator.SetBool(Cast, DrawingManager.isDrawing);
    }
}
