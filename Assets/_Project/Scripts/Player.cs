using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform graphic;
    [SerializeField] private float minPencilAngle;
    [SerializeField] private float pencilRotationSpeed;
    [SerializeField] private Transform handLeft;
    [SerializeField] private Transform pencilParent;

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

    private float flip = 1;
    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        cc = GetComponent<PhysicCharacterController>();
    }

    private void Update()
    {
        
        Quaternion pencilTargetRotation = Quaternion.identity;
        Quaternion handTargetRotation = Quaternion.identity;
        
        if (DrawingManager.isDrawing)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = pos - (Vector2)pencilParent.position;
            if (dir.x < -0.5f) flip = -1;
            if (dir.x > 0.5f) flip = 1;
            if (flip < 0) dir.x *= -1;
            float angle = Vector2.SignedAngle(Vector2.right, dir);
            if (angle < minPencilAngle)
            {
                pencilTargetRotation = Quaternion.AngleAxis(180f, Vector3.forward);
                angle -= 180;
            }
            handTargetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            if (cc.MovementVelocity < -0.1f) flip = -1;
            if (cc.MovementVelocity > 0.1f) flip = 1;
        }
        
        animator.SetFloat(Speed, Mathf.Abs(cc.MovementVelocity) / cc.maxSpeed);
        graphic.localScale = new Vector3(flip, 1f, 1f);
        animator.SetBool(Cast, DrawingManager.isDrawing);

        float pencilT = 1f - Mathf.Pow(0.5f, pencilRotationSpeed * Time.deltaTime);
        pencilParent.localRotation = Quaternion.Lerp(pencilParent.localRotation, pencilTargetRotation, pencilT);
        handLeft.localRotation = Quaternion.Lerp(handLeft.localRotation, handTargetRotation, pencilT);
    }
}