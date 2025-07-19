using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Player Stats")]
    public int health = 100;
    public int stamina = 100;

    [Header("Visual Reference")]
    public GameObject visual;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    private bool isBasicAttacking = false;
    private bool isDashing = false;
    private bool isUsingSkill = false;
    private bool isUsingUltimate = false;

    private float dashCooldownTimer = 0f;
    private float staminaRegenDelay = 0f;
    private float staminaRegenTimer = 0f;

    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (visual == null)
            visual = transform.Find("Visual")?.gameObject;

        if (visual != null)
            anim = visual.GetComponent<Animator>();
        else
            Debug.LogError("Visual GameObject tidak ditemukan!");
    }

    private void Update()
    {
        HandleInput();
        HandleDashCooldown();
        HandleStaminaRegen();
    }

    private void FixedUpdate()
    {
        if (!isDashing && !isUsingSkill && !isUsingUltimate)
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleInput()
    {
        if (isUsingSkill || isUsingUltimate) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton2))
            StartCoroutine(HandleBasicAttack());

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3))
            StartCoroutine(HandleSkill());

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton1))
            StartCoroutine(HandleUltimate());

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton0))
            HandleDash();

        UpdateFacingDirection();
    }

    private IEnumerator HandleBasicAttack()
    {
        if (isBasicAttacking || isUsingSkill || isUsingUltimate || anim == null) yield break;

        isBasicAttacking = true;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            return !state.IsName("Attack") || state.normalizedTime >= 1f;
        });

        isBasicAttacking = false;
    }

    private IEnumerator HandleSkill()
    {
        if (stamina < 10 || anim == null || isUsingSkill || isUsingUltimate) yield break;

        isUsingSkill = true;
        stamina -= 10;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;

        rb.simulated = false;
        movementInput = Vector2.zero;

        anim.SetTrigger("Skill");

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            return state.IsName("Skill") && state.normalizedTime >= 1f;
        });

        rb.simulated = true;
        isUsingSkill = false;
    }

    private IEnumerator HandleUltimate()
    {
        if (stamina < 30 || anim == null || isUsingUltimate || isUsingSkill) yield break;

        isUsingUltimate = true;
        stamina -= 30;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;

        rb.simulated = false;
        movementInput = Vector2.zero;

        anim.SetTrigger("Ultimate");

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            return state.IsName("Ultimate") && state.normalizedTime >= 1f;
        });

        rb.simulated = true;
        isUsingUltimate = false;
    }

    private void HandleDash()
    {
        if (isDashing || dashCooldownTimer > 0f || stamina < 5 || isUsingSkill || isUsingUltimate) return;

        stamina -= 5;
        isDashing = true;
        dashCooldownTimer = 1f;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;

        Vector2 dashDirection = (transform.rotation.y == 0) ? Vector2.left : Vector2.right;
        Vector2 dashTarget = rb.position + dashDirection * 5f;

        StartCoroutine(DashMovement(dashTarget));
    }

    private IEnumerator DashMovement(Vector2 targetPosition)
    {
        rb.MovePosition(targetPosition);
        yield return new WaitForSeconds(0.1f);
        isDashing = false;
    }

    private void HandleDashCooldown()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    private void HandleStaminaRegen()
    {
        if (stamina >= 100) return;

        if (staminaRegenDelay > 0f)
        {
            staminaRegenDelay -= Time.deltaTime;
            return;
        }

        staminaRegenTimer += Time.deltaTime;

        if (staminaRegenTimer >= 1f)
        {
            stamina += 5;
            stamina = Mathf.Clamp(stamina, 0, 100);
            staminaRegenTimer = 0f;
        }
    }

    private void UpdateFacingDirection()
    {
        if (movementInput.x > 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (movementInput.x < 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public int GetHealth() => health;
    public int GetStamina() => stamina;
    public void SetHealth(int value) => health = Mathf.Clamp(value, 0, 100);
    public void SetStamina(int value) => stamina = Mathf.Clamp(value, 0, 100);
}
