using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawing : MonoBehaviour
{
    [SerializeField] private float width = 0.1f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private Line linePrefab;
    
    private Rigidbody2D rb;

    private bool drawing = true;

    private Line currentLine;

    private List<Line> lines = new ();

    public Action<bool> OnDrawingCompleted; // valid

    private RaycastHit2D[] hits = new RaycastHit2D[20];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }

    void NewLine(Vector3 pos)
    {
        currentLine = Instantiate(linePrefab, transform);
        currentLine.SetWidth(width);
        currentLine.p1 = pos;
        currentLine.p2 = pos;
        currentLine.UpdateLine();
        lines.Add(currentLine);
    }

    private void Update()
    {
        if (!drawing) return;
        
        Vector2 pos = InputManager.MousePosition;

        if (currentLine == null) NewLine(pos);

        currentLine.p2 = pos;
        currentLine.UpdateLine();

        /*if (!currentLine.valid)
        {
            bool found = false;
            float minDistance = Mathf.Infinity;
            Vector2 minOutPos = Vector2.zero;
            int count = 8;
            float rayCastDistance = 0.4f;
            for (int i = 0; i < count; i++)
            {
                float angle = (float) i / count * Mathf.PI * 2f;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                int hitsCount = Physics2D.CircleCastNonAlloc(pos + dir * rayCastDistance, width / 2f, -dir, hits, rayCastDistance, currentLine.wallMask);
                
                if (hitsCount > 0)
                {
                    var hit = hits[hitsCount - 1];
                    Vector2 outPos = hit.centroid + 0.03f * hit.normal;
                    currentLine.p2 = outPos;
                    currentLine.UpdateLine();
                    if (currentLine.valid)
                    {
                        float distance = hit.distance;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            found = true;
                            minOutPos = outPos;
                        }
                    }
                }
            }

            if (found)
            {
                pos = minOutPos;
                currentLine.p2 = minOutPos;
                currentLine.UpdateLine();
            }
        }*/

        if (currentLine.valid && currentLine.Lenght > minDistance)
        {
            NewLine(pos);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            bool valid = EndDraw();
            OnDrawingCompleted?.Invoke(valid);
        }
    }

    bool EndDraw()
    {
        drawing = false;
        rb.isKinematic = false;

        if (!currentLine.valid)
        {
            Destroy(currentLine.gameObject);
            lines.RemoveAt(lines.Count-1);
        }

        if (lines.Count == 0)
        {
            Destroy(gameObject);
            return false;
        }

        float totalLenght = 0f;

        foreach (Line line in lines)
        {
            totalLenght += line.Lenght;
            line.gameObject.layer = 0;
        }
        gameObject.layer = 0;

        rb.mass = totalLenght;

        return true;
    }
}
