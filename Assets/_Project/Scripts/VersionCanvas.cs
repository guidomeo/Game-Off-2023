using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionCanvas : MonoBehaviour
{
    public static VersionCanvas instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    private void OnDestroy()
    {
        if (instance != this) return;
        
        instance = null;
    }
}
