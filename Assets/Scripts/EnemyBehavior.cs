using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 100;
    public int health = 100;
    public int stamina = 50;
    public int defense = 5;
    public float moveSpeed = 3f;
    public int contactDamage = 10;

    [Header("Health Bar")]
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private float smoothSpeed = 5f;

    private Rigidbody2D rb;
    private Transform player;
    private float targetFillAmount = 1f; // Added missing variable

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeHealthBar();
        FindPlayer();
    }

    private void InitializeHealthBar()
    {
        if (healthBarCanvas != null)
        {
            // Assign Main Camera to World Space Canvas
            healthBarCanvas.worldCamera = Camera.main;

            healthFillImage.fillAmount = 1f;
            healthFillImage.color = healthGradient.Evaluate(1f);
        }
        else
        {
            Debug.LogError("HealthBarCanvas reference is missing!", this);
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found in scene.", this);
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        UpdateMovement();
        UpdateHealthBar();
    }

    private void UpdateMovement()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        UpdateFacingDirection(direction);
        Move(direction);
    }

    private void UpdateHealthBar()
    {
        if (healthFillImage == null) return;

        targetFillAmount = (float)health / maxHealth; // Calculate target fill

        healthFillImage.fillAmount = Mathf.Lerp(
            healthFillImage.fillAmount,
            targetFillAmount,
            Time.deltaTime * smoothSpeed
        );

        healthFillImage.color = healthGradient.Evaluate(healthFillImage.fillAmount);
    }

    public void TakeDamage(int amount)
    {
        int finalDamage = Mathf.Max(0, amount - defense);
        health -= finalDamage;
        health = Mathf.Clamp(health, 0, maxHealth);

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
            var playerDamage = collision.GetComponent<DamageTaken>();
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
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = Vector3.one;
        }
    }

    // Make health bar face camera (for 3D games)
    private void LateUpdate()
    {
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.forward = Camera.main.transform.forward;
        }
    }
}