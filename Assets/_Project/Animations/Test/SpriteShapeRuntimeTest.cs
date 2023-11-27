using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class SpriteShapeRuntimeTest : MonoBehaviour
{
    public SpriteShapeController contr;
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;

    void Update()
    {
        //contr.spline.
        if (Application.isPlaying)
        {
            contr.spline.SetPosition(0, p1);
            contr.spline.SetPosition(1, p2);
            contr.spline.SetPosition(2, p3);
        }
    }

    [ContextMenu("Update Values")]
    public void UpdateValues()
    {
        Undo.RegisterCompleteObjectUndo(this , "ciao");
        p1 = contr.spline.GetPosition(0);
        p2 = contr.spline.GetPosition(1);
        p3 = contr.spline.GetPosition(2);
    }
}
