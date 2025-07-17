using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 100;
    public int stamina = 50;
    public int defense = 5;
    public float moveSpeed = 3f;
    public int contactDamage = 10;

    private Rigidbody2D rb;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player with tag 'Player' not found in scene.");
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        UpdateFacingDirection(direction);
        Move(direction);
    }

    public void TakeDamage(int amount)
    {
        int finalDamage = Mathf.Max(0, amount - defense);
        health -= finalDamage;
        health = Mathf.Clamp(health, 0, 999);

        Debug.Log($"{gameObject.name} received {finalDamage} damage after defense. Current HP: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    public void Move(Vector2 direction)
    {
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DamageTaken playerDamage = collision.GetComponent<DamageTaken>();
            if (playerDamage != null)
            {
                playerDamage.ApplyDamage(contactDamage);
            }
        }
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

}
