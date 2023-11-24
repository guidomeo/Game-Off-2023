using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] LineRenderer lineRend;
    [SerializeField] CapsuleCollider2D coll;
    public LayerMask wallMask;

    [NonSerialized] public Vector2 p1;
    [NonSerialized] public Vector2 p2;
    [NonSerialized] public bool valid;

    private float width;
    
    public float Lenght => Vector3.Distance(p1, p2);

    public void SetWidth(float width)
    {
        this.width = width;
        lineRend.widthMultiplier = width;
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
        
        lineRend.SetPositions(new Vector3[] {transform.InverseTransformPoint(p1), transform.InverseTransformPoint(p2)});
        Color color = valid ? new Color(0.2f, 0.2f, 0.2f) : Color.red;
        lineRend.startColor = color;
        lineRend.endColor = color;
        
        if (!valid) DrawingManager.instance.ShowCannotBuild();
    }

    public void End()
    {
        lineRend.startColor = Color.black;
        lineRend.endColor = Color.black;
        //Destroy(lineRend);
    }
}
