using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BigPencil : MonoBehaviour
{
    [SerializeField] private float animSpeed;
    [SerializeField] private SpriteRenderer glow;
    [SerializeField] private Transform pencilAnim;

    //private Sequence loopAnim;
    private void Awake()
    {
        glow.color = new Color(1f,1f,1f, 0f);
        //loopAnim = DOTween.Sequence();
        //loopAnim.Append(pencilAnim.DOLocalMove(new Vector3(-0.139f, 0.196f), 1f));
        //loopAnim.Append(pencilAnim.DOLocalMove(Vector3.zero, 1f));
        //loopAnim.SetLoops(-1);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;

        Transform parent = transform.parent;

        player.cc.canMove = false;
        //parent.GetComponent<SpriteRenderer>().sortingLayerName = "Player";

        //loopAnim.Kill();
        //Destroy(pencilAnim.GetComponent<Animator>());
        Sequence anim = DOTween.Sequence();
        //anim.Append(pencilAnim.DOLocalMove(Vector3.zero, 0.1f));
        //anim.Join(pencilAnim.DOLocalRotate(Vector3.zero, 0.1f));
        //anim.Append(pencilAnim.DOLocalRotate(Vector3.forward * 30f, 0.5f));
        //anim.Append(pencilAnim.DOLocalRotate(Vector3.forward * -30f, 1f));
        //anim.Append(pencilAnim.DOLocalRotate(Vector3.forward * 30f, 0.2f));
        //anim.Append(pencilAnim.DOLocalRotate(Vector3.forward * -30f, 0.4f));
        anim.Append(pencilAnim.DOLocalRotate(Vector3.forward * 0f, 0.1f));
        
        anim.OnComplete(() =>
        {
            player.PickBigPencil(parent, animSpeed);
            
            glow.DOFade(0f, 1.5f).SetDelay(3.5f);
        });
        
        Destroy(gameObject);
    }
}
