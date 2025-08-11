using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyBehavior))]
public class DemonSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float skillCooldown = 12f;       // Cooldown antar skill
    [Range(0f, 1f)] public float skillChance = 0.35f; // 35% peluang skill keluar

    [Header("Prefab Settings")]
    public GameObject[] randomPrefabs;      // Untuk spawn acak
    public GameObject playerTargetPrefab;   // Prefab yang spawn di lokasi player

    private Rigidbody2D rb;
    private EnemyBehavior enemyBehavior;
    private bool isSkillOnCooldown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        // Step 0: Freeze 1 detik
        rb.simulated = false;
        yield return new WaitForSeconds(1f);
        rb.simulated = true;

        int healAmount = Mathf.RoundToInt(enemyBehavior.maxHealth * 0.03f);
        enemyBehavior.currentHealth = Mathf.Min(enemyBehavior.currentHealth + healAmount, enemyBehavior.maxHealth);


        // Step 2: Spawn 5–7 prefab random di sekitar Demon
        int spawnCount = Random.Range(5, 8);
        for (int i = 0; i < spawnCount; i++)
        {
            if (randomPrefabs.Length > 0)
            {
                GameObject prefab = randomPrefabs[Random.Range(0, randomPrefabs.Length)];
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
        }

        // Step 3: Spawn prefab di lokasi player terus menerus selama 8 detik
        float timer = 0f;
        while (timer < 8f)
        {
            if (playerTargetPrefab != null && enemyBehavior.player != null)
            {
                Instantiate(playerTargetPrefab, enemyBehavior.player.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.5f); // jarak antar spawn
            timer += 0.5f;
        }

        isSkillOnCooldown = false;
    }
}
