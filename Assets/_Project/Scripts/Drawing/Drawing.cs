using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawing : MonoBehaviour
{
    [SerializeField] private float width = 0.1f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float maxLength = 25f;
    [SerializeField] private Line linePrefab;
    public LayerMask wallMask;
    
    private Rigidbody2D rb;

    private bool drawing = true;

    private Line currentLine;

    private List<Line> lines = new ();

    public Action<bool> OnDrawingCompleted; // valid

    private RaycastHit2D[] hits = new RaycastHit2D[20];

    private float currentLength;

    private LineRenderer lineRend;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        lineRend = GetComponent<LineRenderer>();
        lineRend.widthMultiplier = width;
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

        if (currentLine == null)
        {
            pos = NearestFreePos(pos, pos);
            NewLine(pos);
            currentLength = 0f;
            if (!currentLine.valid)
            {
                DrawingManager.instance.ShowCannotBuild(0.5f);
                EndDraw();
                return;
            }
        }

        pos = NearestFreePos(pos, currentLine.p1);
        currentLine.p2 = pos;
        currentLine.UpdateLine();

        if (currentLine.valid)
            DrawingManager.instance.HideCannotBuild();
        else
            DrawingManager.instance.ShowCannotBuild();

        if (currentLine.valid)
        {
            float overLenght = currentLength + currentLine.Lenght - maxLength;
            if (overLenght > 0f)
            {
                currentLine.p2 += overLenght * (currentLine.p1 - currentLine.p2).normalized;
                currentLine.UpdateLine();
                EndDraw();
                return;
            }
            if (currentLine.Lenght > minDistance)
            {
                currentLength += currentLine.Lenght;
                NewLine(pos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            DrawingManager.instance.HideCannotBuild();
            EndDraw();
            return;
        }
    }

    void EndDraw()
    {
        bool valid = _EndDraw();
        OnDrawingCompleted?.Invoke(valid);
    }

    bool _EndDraw()
    {
        drawing = false;
        rb.isKinematic = false;

        if (!currentLine.valid) // Destroy last line if it is not valid
        {
            Destroy(currentLine.gameObject);
            lines.RemoveAt(lines.Count-1);
        }

        if (lines.Count == 0) // Destroy drawing if is made of no lines
        {
            Destroy(gameObject);
            return false;
        }

        float totalLenght = 0f;

        lineRend.positionCount = lines.Count + 1;

        lineRend.SetPosition(0, lines[0].p1);
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            lineRend.SetPosition(i + 1, line.p2);
            totalLenght += line.Lenght;
            line.gameObject.layer = 0;
            line.End();
        }

        gameObject.layer = 0;

        rb.mass = totalLenght;

        return true;
    }

    Vector2 NearestFreePos(Vector2 pos, Vector2 lastPos)
    {
        if (!Physics2D.OverlapCircle(pos, width / 2f, wallMask)) return pos;
        
        bool found = false;
        float minDistance = Mathf.Infinity;
        Vector2 minOutPos = Vector2.zero;
        int count = 8;
        float rayCastDistance = 0.4f;
        for (int i = 0; i < count; i++)
        {
            float angle = (float) i / count * Mathf.PI * 2f;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            int hitsCount = Physics2D.CircleCastNonAlloc(pos + dir * rayCastDistance, width / 2f, -dir, hits, rayCastDistance, wallMask);
            
            if (hitsCount > 0)
            {
                var hit = hits[hitsCount - 1];
                Vector2 outPos = hit.centroid + 0.03f * hit.normal;
                if (Physics2D.OverlapCircle(outPos, width / 2f, wallMask)) continue;
                float distance = Vector2.Distance(outPos, lastPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    found = true;
                    minOutPos = outPos;
                }
            }
        }

        return found ? minOutPos : pos;
    }
}
