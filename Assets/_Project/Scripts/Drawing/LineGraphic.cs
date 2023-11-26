using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGraphic : MonoBehaviour
{
    [SerializeField] LineRenderer lineRend;
    
    public void SetWidth(float width)
    {
        lineRend.widthMultiplier = width;
    }
    
    public void UpdateLine(Line line)
    {
        float angle = Vector2.SignedAngle(Vector2.down, line.p2 - line.p1);
        Vector2 position = (line.p1 + line.p2) / 2f;

        transform.position = position;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        lineRend.SetPositions(new Vector3[] {transform.InverseTransformPoint(line.p1), transform.InverseTransformPoint(line.p2)});
        Color color = line.valid ? new Color(0.2f, 0.2f, 0.2f) : Color.red;
        lineRend.startColor = color;
        lineRend.endColor = color;
        
        //if (!line.valid) DrawingManager.instance.ShowCannotBuild();
    }
    
    public void End()
    {
        lineRend.startColor = Color.black;
        lineRend.endColor = Color.black;
    }
}
