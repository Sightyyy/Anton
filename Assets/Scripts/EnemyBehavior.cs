using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 100;
    public int currentHealth;
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
    private Animator animator;
    public Transform player;
    public event Action onDeath;
    private float targetFillAmount = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        InitializeHealthBar();
        FindPlayer();

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"{gameObject.name} tidak memiliki Animator. Animasi tidak akan dimainkan.");
        }
    }

    private void InitializeHealthBar()
    {
        if (healthBarCanvas != null)
        {
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
        SmoothHealthBar();
    }

    private void UpdateMovement()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        bool isMoving = direction.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetInteger("State", isMoving ? 1 : 0);
        }

        UpdateFacingDirection(direction);
        Move(direction);
    }


    private void SmoothHealthBar()
    {
        if (healthFillImage == null) return;

        targetFillAmount = (float)currentHealth / maxHealth;
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
        int oldHealth = currentHealth;

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
            healthFillImage.color = healthGradient.Evaluate(healthFillImage.fillAmount);
        }

        Debug.Log(
            $"{gameObject.name} terkena serangan!" +
            $"\nDamage mentah: {amount}" +
            $"\nDefense: {defense}" +
            $"\nDamage akhir: {finalDamage}" +
            $"\nHP Sebelum: {oldHealth}" +
            $"\nHP Sesudah: {currentHealth}"
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        onDeath?.Invoke();
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
            if (animator != null)
            {
                animator.Play("Attack");
            }

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
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = Vector3.one;
    }

    private void LateUpdate()
    {
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.forward = Camera.main.transform.forward;
        }
    }
}