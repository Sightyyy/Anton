using UnityEngine;

public class RbSwitch : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D cl;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        cl = GetComponentInParent<Collider2D>();
    }

    public void DisableRb()
    {
        rb.simulated = false;
        cl.enabled = false;
    }

    public void EnableRb()
    {
        rb.simulated = true;
        cl.enabled = true;
    }
}
