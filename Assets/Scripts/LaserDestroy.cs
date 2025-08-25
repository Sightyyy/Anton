using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDestroy : MonoBehaviour
{
    public float destroyDelay = 0f;

    // Fungsi ini bisa dipanggil dari Animation Event
    public void DestroyObject()
    {
        if (destroyDelay > 0f)
            Destroy(gameObject, destroyDelay);
        else
            Destroy(gameObject);
    }
}
