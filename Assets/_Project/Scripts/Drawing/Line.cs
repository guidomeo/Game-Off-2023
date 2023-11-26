using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] CapsuleCollider2D coll;
    public LayerMask wallMask;

    [NonSerialized] public Vector2 p1;
    [NonSerialized] public Vector2 p2;
    [NonSerialized] public bool valid;

    private float width;
    
    public float Lenght => Vector3.Distance(p1, p2);
    public Vector2 MidPoint => (p1 + p2) / 2f;
    

    public void SetWidth(float width)
    {
        this.width = width;
    }

    public void UpdateLine()
    {
        Vector2 size = new Vector2(width, Lenght + width);
        float angle = Vector2.SignedAngle(Vector2.down, p2 - p1);
        Vector2 position = (p1 + p2) / 2f;

        transform.position = position;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        coll.size = size;

        valid = !Physics2D.OverlapCapsule(position, size, CapsuleDirection2D.Vertical, angle, wallMask);
        coll.enabled = valid;
    }
}
