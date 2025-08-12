using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RbSwitch : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void DisableRb()
    {
        rb.simulated = false;
    }

    public void EnableRb()
    {
        rb.simulated = true;
    }
}
