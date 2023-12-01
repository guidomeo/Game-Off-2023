using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGameTrigger : MonoBehaviour
{
    private bool used = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (used) return;
        Rigidbody2D rb = col.attachedRigidbody;
        if (rb == null) return;
        Player player = rb.GetComponent<Player>();
        if (player == null) return;
        used = true; 

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
