using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Player Stats")]
    public int maxHealth;
    public int health;
    public int maxStamina;
    public int stamina;
    public float healingPercent = 0.07f;

    [Header("Visual Reference")]
    public GameObject visual;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    [SerializeField] private Image skillCooldownImage;
    [SerializeField] private Image ultimateCooldownImage;
    [SerializeField] private Image healingCooldownImage;
    [SerializeField] private TMP_Text debuffText;

    public bool isWeakened = false;
    public bool isBurning = false;
    public bool isPoisoned = false;
    public bool isSlowed = false;
    public bool isKnockbacked = false;
    public bool isDead = false;
    private bool isDashing = false;
    public bool isInvulnerable = false;
    public bool isDoingAction = false;
    private bool isHardLocked = false;
    public bool IsHardLocked() => isHardLocked;

    private float weakenTimer = 0f;
    private float burnTimer = 0f;
    private float poisonTimer = 0f;
    private float slowTimer = 0f;
    private float knockbackCooldown = 0f;
    private float originalSpeed;
    private SpriteRenderer spriteRenderer;
    private float dashCooldownTimer = 0f;
    private float staminaRegenDelay = 0f;
    private float staminaRegenTimer = 0f;
    private float skillCooldownTimer = 0f;
    private float ultimateCooldownTimer = 0f;
    private float healingCooldownTimer = 0f;

    private Animator anim;
    AudioCollection audioCollection;
    private void Awake()
    {
        // component refs
        rb = GetComponent<Rigidbody2D>();

        if (visual == null)
            visual = transform.Find("Visual")?.gameObject;

        if (visual != null)
        {
            spriteRenderer = visual.GetComponent<SpriteRenderer>();
            anim = visual.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Visual GameObject tidak ditemukan!");
        }

        originalSpeed = moveSpeed;
        audioCollection = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioCollection>();
        if (maxHealth <= 0) maxHealth = 100;
        health = Mathf.Clamp(health > 0 ? health : maxHealth, 0, maxHealth);
    }

    private void Update()
    {
        HandleInput();
        HandleDashCooldown();
        HandleStaminaRegen();
        HandleSkillCooldown();
        HandleUltimateCooldown();
        HandleHealingCooldown();
        HandleKnockbackCooldown();
        UpdateAnimatorState();
        UpdateDebuffUI();
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

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton4))
            StartCoroutine(HandleHealing());
    }

    private IEnumerator HandleBasicAttack()
    {
        if (isDoingAction || anim == null) yield break;

        isDoingAction = true;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.55f);

        isDoingAction = false;
    }



    private IEnumerator HandleSkill()
    {
        if (stamina < 10 || isDoingAction || anim == null || skillCooldownTimer > 0f) yield break;

        isDoingAction = true;
        isHardLocked = true;
        isInvulnerable = true;
        stamina -= 10;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;
        skillCooldownTimer = 8f;
        if (skillCooldownImage) skillCooldownImage.gameObject.SetActive(true);

        anim.SetTrigger("Skill");

        yield return new WaitForSeconds(1.683f);

        isDoingAction = false;
        isHardLocked = false;
        isInvulnerable = false;
    }

    private void HandleSkillCooldown()
    {
        if (skillCooldownTimer > 0f)
        {
            skillCooldownTimer -= Time.deltaTime;

            if (skillCooldownImage != null)
            {
                skillCooldownImage.fillAmount = skillCooldownTimer / 8f;
            }
        }
        else if (skillCooldownTimer <= 0f && skillCooldownImage != null)
        {
            skillCooldownImage.gameObject.SetActive(false);
        }
        else if (skillCooldownImage != null)
        {
            skillCooldownImage.fillAmount = 0f;
        }
    }

    private IEnumerator HandleUltimate()
    {
        if (stamina < 30 || isDoingAction || anim == null || ultimateCooldownTimer > 0f) yield break;

        isDoingAction = true;
        isInvulnerable = true;
        isHardLocked = true;
        stamina -= 30;
        staminaRegenDelay = 1f;
        staminaRegenTimer = 0f;
        ultimateCooldownTimer = 20f;
        if (ultimateCooldownImage) ultimateCooldownImage.gameObject.SetActive(true);

        anim.SetTrigger("Ultimate");

        yield return new WaitForSeconds(2.35f);

        isDoingAction = false;
        isInvulnerable = false;
        isHardLocked = false;
    }

    private void HandleUltimateCooldown()
    {
        if (ultimateCooldownTimer > 0f)
        {
            ultimateCooldownTimer -= Time.deltaTime;

            if (ultimateCooldownImage != null)
            {
                ultimateCooldownImage.fillAmount = ultimateCooldownTimer / 20f;
            }
        }
        else if (ultimateCooldownTimer <= 0f && ultimateCooldownImage != null)
        {
            ultimateCooldownImage.gameObject.SetActive(false);
        }
        else if (ultimateCooldownImage != null)
        {
            ultimateCooldownImage.fillAmount = 0f;
        }
    }

    private IEnumerator HandleHealing()
    {
        if (isDead || isDoingAction || healingCooldownTimer > 0f) yield break;

        audioCollection.PlaySFX(audioCollection.heal);
        int healAmount = Mathf.CeilToInt(maxHealth * healingPercent);
        Heal(healAmount);

        // === Hapus semua debuff kecuali Burning ===
        isWeakened = false;
        isPoisoned = false;
        isSlowed = false;
        isKnockbacked = false;
        weakenTimer = 0f;
        poisonTimer = 0f;
        slowTimer = 0f;
        knockbackCooldown = 0f;
        // Burning dibiarkan tetap aktif

        // === Tambahkan invulnerability 3 detik ===
        StartCoroutine(TemporaryInvulnerability(3f));

        healingCooldownTimer = 7f;
        if (healingCooldownImage) healingCooldownImage.gameObject.SetActive(true);

        yield return null;
    }
    private IEnumerator TemporaryInvulnerability(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
        if (!isBurning && spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    private void HandleHealingCooldown()
    {
        if (healingCooldownTimer > 0f)
        {
            healingCooldownTimer -= Time.deltaTime;

            if (healingCooldownImage != null)
            {
                healingCooldownImage.fillAmount = healingCooldownTimer / 7;
            }
        }
        else if (healingCooldownTimer <= 0f && healingCooldownImage != null)
        {
            healingCooldownImage.gameObject.SetActive(false);
        }
        else if (healingCooldownImage != null)
        {
            healingCooldownImage.fillAmount = 0f;
        }
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

        if (staminaRegenTimer >= 1.55f)
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
            anim?.SetInteger("State", 2);
            isDoingAction = true;
            isDead = true;
            movementInput = Vector2.zero;
            return;
        }

        if (isDoingAction || isDead) return;

        if (movementInput != Vector2.zero)
            anim?.SetInteger("State", 1);
        else
            anim?.SetInteger("State", 0);
    }

    public int GetHealth() => health;
    public int GetStamina() => stamina;
    public void SetHealth(int value) => health = Mathf.Clamp(value, 0, maxHealth);

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || isDead) return;

        SetHealth(health - amount);

        if (health <= 0)
        {
            isDead = true;
            isDoingAction = true;
            anim?.SetInteger("State", 2);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || isDead) return;
        SetHealth(health + amount);
    }

    public void SetStamina(int value) => stamina = Mathf.Clamp(value, 0, 100);

    public void ApplyWeaken()
    {
        if (isWeakened)
        {
            weakenTimer = 15f;
            return;
        }

        isWeakened = true;
        weakenTimer = 15f;
        StartCoroutine(WeakenEffect());
    }

    private IEnumerator WeakenEffect()
    {
        while (weakenTimer > 0f)
        {
            weakenTimer -= Time.deltaTime;
            yield return null;
        }

        isWeakened = false;
    }

    public bool IsDebuffActive(string debuffName)
    {
        switch (debuffName)
        {
            case "Weaken": return isWeakened;
            case "Burning": return isBurning;
            case "Poisoned": return isPoisoned;
            case "Slowed": return isSlowed;
            case "Knockbacked": return isKnockbacked;
            default: return false;
        }
    }

    public void ApplyBurning()
    {
        if (isBurning)
        {
            burnTimer = 5f;
            return;
        }

        isBurning = true;
        burnTimer = 5f;
        StartCoroutine(BurningEffect());
    }

    private IEnumerator BurningEffect()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.red;

        while (burnTimer > 0f)
        {
            int dmg = Mathf.CeilToInt(health * 0.03f);
            TakeDamage(dmg);
            burnTimer -= 1f;
            yield return new WaitForSeconds(1f);
        }

        isBurning = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    public void ApplyPoisoned()
    {
        if (isPoisoned)
        {
            poisonTimer = 15f;
            return;
        }

        isPoisoned = true;
        poisonTimer = 15f;
        slowTimer = Mathf.Max(slowTimer, 8f);
        StartCoroutine(PoisonedEffect());
    }

    private IEnumerator PoisonedEffect()
    {
        if (spriteRenderer != null) spriteRenderer.color = new Color(0.5f, 0f, 0.5f);

        moveSpeed = originalSpeed * 0.85f;

        while (poisonTimer > 0f)
        {
            if (health > Mathf.CeilToInt(maxHealth * 0.15f))
            {
                int dmg = Mathf.CeilToInt(health * 0.05f);
                TakeDamage(dmg);
            }
            poisonTimer -= 1f;
            yield return new WaitForSeconds(1f);
        }

        moveSpeed = originalSpeed;
        isPoisoned = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    public void ApplySlowed()
    {
        if (isSlowed)
        {
            slowTimer = 8f;
            return;
        }

        isSlowed = true;
        slowTimer = 8f;
        StartCoroutine(SlowedEffect());
    }

    private IEnumerator SlowedEffect()
    {
        if (spriteRenderer != null) spriteRenderer.color = new Color(0.25f, 0.25f, 1f);
        moveSpeed = originalSpeed * 0.5f;

        while (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            yield return null;
        }

        moveSpeed = originalSpeed;
        isSlowed = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    public void ApplyKnockbacked(Transform source)
    {
        if (isKnockbacked || knockbackCooldown > 0f) return;

        Vector2 knockDirection = ((Vector2)(transform.position - source.position)).normalized;

        StartCoroutine(KnockbackEffect(knockDirection));
    }

    private IEnumerator KnockbackEffect(Vector2 direction)
    {
        isKnockbacked = true;
        isHardLocked = true;
        knockbackCooldown = 2.5f;

        Vector2 knockbackTarget = rb.position + direction * 5f;
        rb.MovePosition(knockbackTarget);

        yield return new WaitForSeconds(0.8f);

        isKnockbacked = false;
        isHardLocked = false;
    }

    private void HandleKnockbackCooldown()
    {
        if (knockbackCooldown > 0f)
            knockbackCooldown -= Time.deltaTime;
    }
     private void UpdateDebuffUI()
    {
        if (debuffText == null) return;

        string result = "";

        if (isWeakened) result += "<color=#AAAAAA>Weakened</color>\n";
        if (isBurning) result += "<color=#FF4444>Burning</color>\n";
        if (isPoisoned) result += "<color=#44FF44>Poisoned</color>\n";
        if (isSlowed) result += "<color=#4488FF>Slowed</color>\n";
        if (isKnockbacked) result += "<color=#FFD700>Knockbacked</color>\n";

        debuffText.text = result.TrimEnd('\n');
    }
}
