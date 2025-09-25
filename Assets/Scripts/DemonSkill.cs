using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyBehavior))]
public class DemonSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float skillCooldown = 12f;

    [Header("Prefab Settings")]
    public GameObject[] laserPrefabs; 
    private Transform playerTargetPrefab;

    private Rigidbody2D rb;
    private EnemyBehavior enemyBehavior;
    private bool isSkillOnCooldown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyBehavior = GetComponent<EnemyBehavior>();
        playerTargetPrefab = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (playerTargetPrefab == null)
            Debug.LogWarning("Player Target Prefab belum di-assign di Inspector!");
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
            StartCoroutine(UseSkill());
        }
    }

    private IEnumerator UseSkill()
    {
        isSkillOnCooldown = true;

        int chosenSkill = Random.Range(0, 16);
        Debug.Log("random number got = " + chosenSkill);

        if (chosenSkill >= 0 && chosenSkill <= 3)
        {
            Debug.Log("use skill 1");
            rb.simulated = false;
            yield return new WaitForSeconds(1f);
            rb.simulated = true;

            int healAmount = Mathf.RoundToInt(enemyBehavior.maxHealth * 0.03f);
            enemyBehavior.currentHealth = Mathf.Min(enemyBehavior.currentHealth + healAmount, enemyBehavior.maxHealth);
        }
        else if (chosenSkill >= 4 && chosenSkill <= 12)
        {
            Debug.Log("use skill 2");
            rb.simulated = false;
            int spawnCount = Random.Range(15, 16);
            float radius = 12.5f;
            float randomOffset = 3f;

            for (int i = 0; i < spawnCount; i++)
            {
                if (laserPrefabs.Length > 0)
                {
                    float angle = i * (2f * Mathf.PI / spawnCount);
                    Vector2 basePos = (Vector2)transform.position + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                    Vector2 spawnPos = basePos + Random.insideUnitCircle * randomOffset;
                    GameObject prefab = laserPrefabs[Random.Range(0, laserPrefabs.Length)];
                    Instantiate(prefab, spawnPos, Quaternion.identity);
                }
            }
            yield return new WaitForSeconds(1f);
            rb.simulated = true;
        }
        else if (chosenSkill >= 13 && chosenSkill <= 15)
        {
            Debug.Log("use skill 3");
            rb.simulated = false;
            yield return new WaitForSeconds(0.5f);
            rb.simulated = true;
            float timer = 0f;
            while (timer < 8f)
            {
                if (laserPrefabs != null && playerTargetPrefab != null)
                {
                    GameObject prefab = laserPrefabs[Random.Range(0, laserPrefabs.Length)];
                    Vector2 spawnPos = playerTargetPrefab.position;
                    Instantiate(prefab, spawnPos, Quaternion.identity);
                }
                yield return new WaitForSeconds(0.5f);
                timer += 0.5f;
            }
        }

        isSkillOnCooldown = false;
    }
}
