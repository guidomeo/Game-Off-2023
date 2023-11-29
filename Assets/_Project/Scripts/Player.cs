using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform graphic;
    [SerializeField] private float minPencilAngle;
    [SerializeField] private float pencilRotationSpeed;
    [SerializeField] private Transform handLeft;
    [SerializeField] private Transform pencilPos;
    [SerializeField] private Transform pencilParent;
    [SerializeField] private Rigidbody2D pencilRb;
    [SerializeField] private AudioData bigPencilJingle;

    private float moveDir = 0f;

    [NonSerialized] public Rigidbody2D rb;
    private Animator animator;
    [NonSerialized] public PhysicCharacterController cc;
    
    Vector2 gravityDir;
    private Vector2 rayDir;
    private RaycastHit2D hit;
    private Vector2 rightDir;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Cast = Animator.StringToHash("Cast");

    public float NormalizedSpeed => Mathf.Abs(cc.MovementVelocity) / cc.maxSpeed;
    
    private float flip = 1;
    private static readonly int Property = Animator.StringToHash("Big Pencil");

    [NonSerialized] public float maxSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        cc = GetComponent<PhysicCharacterController>();
        maxSpeed = cc.maxSpeed;
    }

    private void Update()
    {
        
        Quaternion pencilTargetRotation = Quaternion.identity;
        Quaternion handTargetRotation = Quaternion.identity;
        
        if (DrawingManager.isDrawing)
        {
            Vector2 pos = InputManager.MousePosition;
            Vector2 dir = pos - (Vector2)pencilPos.position;
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
            if (cc.MovementVelocity < -1f) flip = -1;
            if (cc.MovementVelocity > 1f) flip = 1;
        }
        
        
        
        animator.SetFloat(Speed, NormalizedSpeed);
        graphic.localScale = new Vector3(flip, 1f, 1f);
        animator.SetBool(Cast, DrawingManager.isDrawing);

        float pencilT = 1f - Mathf.Pow(0.5f, pencilRotationSpeed * Time.deltaTime);
        pencilPos.localRotation = Quaternion.Lerp(pencilPos.localRotation, pencilTargetRotation, pencilT);
        handLeft.localRotation = Quaternion.Lerp(handLeft.localRotation, handTargetRotation, pencilT);
    }

    public void DropPencil(Vector2 force)
    {
        pencilRb.isKinematic = false;
        pencilRb.transform.SetParent(null);
        pencilRb.AddForce(force, ForceMode2D.Impulse);
        pencilRb.GetComponent<Collider2D>().enabled = true;
    }

    public void PickBigPencil(Transform pencilTr, float speed)
    {
        bigPencilJingle.Play();
        
        pencilRb.gameObject.SetActive(false);
        cc.canMove = false;
        
        animator.SetTrigger(Property);

        StartCoroutine(DoFinal(pencilTr, speed));
    }

    IEnumerator DoFinal(Transform pencilTr, float speed)
    {
        yield return null;
        pencilTr.SetParent(pencilParent);
        pencilTr.DOLocalMove(Vector3.zero, speed);
        pencilTr.DOLocalRotate(Vector3.zero, speed);

        yield return new WaitForSeconds(9.5f);

        PencilGod.instance.gameObject.SetActive(true);
        CameraController.instance.ActivateStaticOverlay();
        
        yield return new WaitForSeconds(1.7f);
        
        FinalCanvas.instance.Activate();
        
        yield return new WaitForSeconds(8f);
        BackgroundAudioPlayer.instance.Play("Final Noise");
        yield return new WaitForSeconds(2f);
        
        FinalCanvas.instance.HideCinemaPanel();
        
        cc.maxSpeed = maxSpeed;
        cc.moveOnlyRight = false;
        cc.canMove = true;
        
        DrawingManager.instance.canDraw = true;
        DrawingManager.instance.big = true;
    }
}
