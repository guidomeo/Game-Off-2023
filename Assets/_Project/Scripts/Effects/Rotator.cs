using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float speed;
    
    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(speed * Time.deltaTime, Vector3.forward);
    }
}
