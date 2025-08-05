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

    public bool isDead = false;

    private bool isDashing = false;
    private bool isDoingAction = false;
    private bool isHardLocked = false;

    public bool IsHardLocked() => isHardLocked;



    private float dashCooldownTimer = 0f;
    private float staminaRegenDelay = 0f;
    private float staminaRegenTimer = 0f;
    private float skillCooldownTimer = 0f;
    private float ultimateCooldownTimer = 0f;

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
        HandleSkillCooldown();
        HandleUltimateCooldown();
        UpdateAnimatorState();
    }

    private void FixedUpdate()
    {
        if (isDead || isDashing || isHardLocked) return;

        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleInput()
    {
        if (isDead || isHardLocked) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        UpdateFacingDirection();

        if (isDoingAction) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton2))
            StartCoroutine(HandleBasicAttack());

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3))
            StartCoroutine(HandleSkill());

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton1))
            StartCoroutine(HandleUltimate());

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton0))
            HandleDash();
    }




    private IEnumerator HandleBasicAttack()
    {
        if (isDoingAction || anim == null) yield break;

        isDoingAction = true;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(1.1f);

        isDoingAction = false;
    }


    private IEnumerator HandleSkill()
    {
        if (stamina < 10 || isDoingAction || anim == null || skillCooldownTimer > 0f) yield break;

        isDoingAction = true;
        isHardLocked = true;
        stamina -= 10;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;
        skillCooldownTimer = 8f;

        anim.SetTrigger("Skill");

        yield return new WaitForSeconds(1f);

        isDoingAction = false;
        isHardLocked = false;
    }

    private void HandleSkillCooldown()
    {
        if (skillCooldownTimer > 0f)
            skillCooldownTimer -= Time.deltaTime;
    }

    private IEnumerator HandleUltimate()
    {
        if (stamina < 30 || isDoingAction || anim == null || ultimateCooldownTimer > 0f) yield break;

        isDoingAction = true;
        isHardLocked = true;
        stamina -= 30;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;
        ultimateCooldownTimer = 20f;

        anim.SetTrigger("Ultimate");

        yield return new WaitForSeconds(1.5f);

        isDoingAction = false;
        isHardLocked = false;
    }

    private void HandleUltimateCooldown()
    {
        if (ultimateCooldownTimer > 0f)
            ultimateCooldownTimer -= Time.deltaTime;
    }

    private void HandleDash()
    {
        if (isDashing || dashCooldownTimer > 0f || stamina < 5 || isDoingAction) return;

        stamina -= 5;
        isDashing = true;
        dashCooldownTimer = 1f;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;

        Vector2 dashDirection = (movementInput != Vector2.zero) ?
            movementInput.normalized :
            (transform.rotation.y == 0 ? Vector2.right : Vector2.left);

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
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (movementInput.x < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void UpdateAnimatorState()
    {
        if (health <= 0 && !isDead)
        {
            anim.SetInteger("State", 2);
            isDoingAction = true;
            isDead = true;
            movementInput = Vector2.zero;
            return;
        }

        if (isDoingAction || isDead) return;

        if (movementInput != Vector2.zero)
            anim.SetInteger("State", 1);
        else
            anim.SetInteger("State", 0);
    }



    public int GetHealth() => health;
    public int GetStamina() => stamina;
    public void SetHealth(int value) => health = Mathf.Clamp(value, 0, 100);
    public void SetStamina(int value) => stamina = Mathf.Clamp(value, 0, 100);
}

