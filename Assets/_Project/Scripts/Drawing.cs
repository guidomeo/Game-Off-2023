using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawing : MonoBehaviour
{
    [SerializeField] private float width = 0.1f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private Line linePrefab;
    
    private Camera cam;
    private Rigidbody2D rb;

    private bool drawing = true;

    private Line currentLine;

    private List<Line> lines = new ();

    private void Awake()
    {
        cam = Camera.main;
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
        
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (currentLine == null) NewLine(pos);

        currentLine.p2 = pos;
        currentLine.UpdateLine();

        if (currentLine.valid && currentLine.Lenght > minDistance)
        {
            NewLine(pos);
        }
        
        if (Input.GetMouseButtonUp(0))
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
                return;
            }

            float totalLenght = 0f;

            foreach (Line line in lines)
            {
                totalLenght += line.Lenght;
                line.gameObject.layer = 0;
            }
            gameObject.layer = 0;

            rb.mass = totalLenght;
        }
    }
}
