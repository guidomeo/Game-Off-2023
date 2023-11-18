using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector2 velocityA;
    public Vector2 velocityB;
    public Vector2 velocityC;

    private void OnValidate()
    {
        velocityC = SubtractVelocity(velocityA, velocityB);
    }


    Vector2 SubtractVelocity(Vector2 a, Vector2 b)
    {
        Vector2 project = Vector3.Project(a, b);
        float projectMag = project.magnitude;
        b = Vector2.ClampMagnitude(b, projectMag);
        a += b;
        return a;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector2.zero, velocityA);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(velocityA, velocityA + velocityB);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(Vector2.zero, velocityC);
    }
}
