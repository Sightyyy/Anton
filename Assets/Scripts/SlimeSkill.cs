using UnityEngine;

public class SlimeSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public GameObject[] skillPrefabs;
    public float skillCooldown = 20f;
    public int spawnCount = 4;
    public float spawnRadius = 3f;

    private float skillTimer;

    private void Update()
    {
        skillTimer += Time.deltaTime;

        if (skillTimer >= skillCooldown)
        {
            UseSkill();
            skillTimer = 0f;
        }
    }

    private void UseSkill()
    {
        if (skillPrefabs.Length == 0) return;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = skillPrefabs[Random.Range(0, skillPrefabs.Length)];
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            Instantiate(prefab, randomPos, Quaternion.identity);
        }
    }
}
