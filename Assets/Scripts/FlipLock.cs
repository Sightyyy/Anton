using UnityEngine;

public class LockFlip : MonoBehaviour
{
    private Vector3 initialScale;
    private Transform parent;

    void Start()
    {
        parent = transform.parent;
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (parent != null)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(parent.localScale.x) * Mathf.Sign(initialScale.x),
                Mathf.Abs(parent.localScale.y) * Mathf.Sign(initialScale.y),
                initialScale.z
            );
        }
    }
}
