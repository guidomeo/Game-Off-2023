using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Drawing : MonoBehaviour
{
    //[SerializeField] float rayCastDistance = 0.4f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float maxLength = 25f;
    [SerializeField] private Line linePrefab;
    [SerializeField] private LineGraphic lineGraphicPrefab;
    [SerializeField] private Transform colliderParent;
    [SerializeField] private Transform graphicParent;
    [Header("Contact Audio")]
    [SerializeField] private AudioDataGroup contactAudio;
    [SerializeField] private float minContactVelocity;
    [SerializeField] private float maxContactVelocity;
    [SerializeField] private float pitchMin = 1f;
    [SerializeField] private float pitchMax = 1f;
    
    public LayerMask wallMask;
    
    private Rigidbody2D rb;

    private bool drawing = true;

    private Line currentLine;
    private LineGraphic currentLineGraphic;

    private List<Line> lines = new ();
    private List<LineGraphic> lineGraphics = new ();
    public Action<bool> OnDrawingCompleted; // valid
    private RaycastHit2D[] hits = new RaycastHit2D[20];
    private float currentLength;
    private LineRenderer lineRend;
    private float totalLenght;
    public bool Valid => currentLine != null && currentLine.valid;

    private float width;
    
    private void Awake()
    {
        width = DrawingManager.instance.Width;
        
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        lineRend = GetComponent<LineRenderer>();
        lineRend.widthMultiplier = width;
    }

    void NewLine(Vector3 pos)
    {
        currentLine = Instantiate(linePrefab, colliderParent);
        currentLineGraphic = Instantiate(lineGraphicPrefab, graphicParent);
        currentLine.SetWidth(width);
        currentLineGraphic.SetWidth(width);
        currentLine.p1 = pos;
        currentLine.p2 = pos;
        currentLine.UpdateLine();
        currentLineGraphic.UpdateLine(currentLine);
        lines.Add(currentLine);
        lineGraphics.Add(currentLineGraphic);
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
        currentLineGraphic.UpdateLine(currentLine);

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
                currentLineGraphic.UpdateLine(currentLine);
                EndDraw();
                return;
            }
            if (currentLine.Lenght > minDistance)
            {
                currentLength += currentLine.Lenght;
                NewLine(pos);
            }
        }

        if (InputManager.mouseUp)
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
            Destroy(currentLineGraphic.gameObject);
            int index = lines.Count - 1;
            lines.RemoveAt(index);
            lineGraphics.RemoveAt(index);
        }

        if (lines.Count == 0) // Destroy drawing if is made of no lines
        {
            Destroy(gameObject);
            return false;
        }

        totalLenght = 0f;

        lineRend.positionCount = lines.Count + 1;

        Vector2 midPoint = Vector2.zero;
        
        lineRend.SetPosition(0, lines[0].p1);
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var lineGraphic = lineGraphics[i];
            lineRend.SetPosition(i + 1, line.p2);
            totalLenght += line.Lenght;
            line.gameObject.layer = 0;
            lineGraphic.End();
            midPoint += line.MidPoint;
        }
        midPoint /= lines.Count;

        Vector2 linePos = colliderParent.transform.position;
        transform.position = midPoint;
        colliderParent.transform.position = linePos;
        graphicParent.transform.position = linePos;

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
        for (int i = 0; i < count; i++)
        {
            float angle = (float) i / count * Mathf.PI * 2f;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            float rayCastDistance = width * 2f;
            int hitsCount = Physics2D.CircleCastNonAlloc(pos + dir * rayCastDistance, width / 2f, -dir, hits, rayCastDistance, wallMask);
            
            if (hitsCount > 0)
            {
                var hit = hits[0];
                Vector2 outPos = hit.centroid + 0.05f * hit.normal;
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.GetContact(i);
            
            Rigidbody2D contactRb = contact.rigidbody;
            if (contactRb != null && contactRb.GetComponent<Player>() != null) continue;

            float velocity = contact.relativeVelocity.magnitude;
            
            if (velocity < minContactVelocity) continue;
            
            float volumeT = Mathf.InverseLerp(minContactVelocity, maxContactVelocity, velocity);
            contactAudio.volumeMultiplier = volumeT;
            
            float pitchT = Mathf.InverseLerp(0f, maxLength / 2f, totalLenght);
            contactAudio.pitchMultiplier = Mathf.Lerp(pitchMin, pitchMax, pitchT);
            
            contactAudio.stereoPan = AudioManager.instance.PanFromPosition(contact.point);
            
            bool inCave = EnvManager.PointInCave(contact.point);
            contactAudio.Play(inCave ? 1 : 0);
            
        }
    }

    public void DestructionEffect()
    {
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            line.DestructionEffect();
        }
    }
}
