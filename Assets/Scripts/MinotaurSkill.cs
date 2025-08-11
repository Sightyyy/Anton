using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyBehavior))]
public class MinotaurSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float skillCooldown = 10f;      // Cooldown antar skill
    [Range(0f, 1f)] public float skillChance = 0.3f; // 30% peluang skill keluar setiap cooldown

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyBehavior enemyBehavior;

    private bool isSkillOnCooldown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyBehavior = GetComponent<EnemyBehavior>();
    }

    private void Start()
    {
        StartCoroutine(SkillLoop());
    }

    private IEnumerator SkillLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(skillCooldown);

            if (Random.value <= skillChance && !isSkillOnCooldown)
            {
                StartCoroutine(UseSkill());
            }
        }
    }

    private IEnumerator UseSkill()
    {
        isSkillOnCooldown = true;

        // Ambil stats original (asumsinya EnemyBehavior pakai int)
        int originalDamage = enemyBehavior.contactDamage;
        int originalDefense = enemyBehavior.defense;

        // Step 1: Freeze + warna merah + damage x2 + defense -75%
        rb.simulated = false;
        spriteRenderer.color = Color.red;
        enemyBehavior.contactDamage = Mathf.RoundToInt(originalDamage * 2f);
        enemyBehavior.defense = Mathf.RoundToInt(originalDefense * 0.25f); // 75% berkurang

        yield return new WaitForSeconds(1f); // durasi freeze

        rb.simulated = true; // aktifkan kembali rigidbody
        spriteRenderer.color = Color.white; // kembalikan warna

        // Defense tetap rendah selama 15 detik
        yield return new WaitForSeconds(15f);
        enemyBehavior.contactDamage = originalDamage;
        enemyBehavior.defense = originalDefense;

        isSkillOnCooldown = false;
    }
}
