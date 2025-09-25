using UnityEngine;

public class DamageDealt : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 10;
    public string targetTag = "Enemy";

    private PlayerBehavior playerBehavior;
    private EnemyBehavior enemyBehavior;

    void Awake()
    {
        playerBehavior = GetComponentInParent<PlayerBehavior>();
        enemyBehavior = GetComponentInParent<EnemyBehavior>();

        if (playerBehavior == null && enemyBehavior == null)
        {
            Debug.LogWarning($"{gameObject.name} tidak menemukan PlayerBehavior atau EnemyBehavior di parent!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag))
            return;

        if (targetTag == "Enemy")
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                int finalDamage = CalculateDamage(damageAmount, true);
                enemy.TakeDamage(finalDamage);
            }
        }
        else if (targetTag == "Player")
        {
            DamageTaken player = collision.GetComponent<DamageTaken>();
            if (player != null)
            {
                int finalDamage = CalculateDamage(damageAmount, false);
                player.ApplyDamage(finalDamage);
            }
        }
    }
    private int CalculateDamage(int baseDamage, bool isAttackingEnemy)
    {
        int result = baseDamage;

        if (isAttackingEnemy)
        {
            if (playerBehavior != null && playerBehavior.isWeakened)
            {
                result = Mathf.RoundToInt(baseDamage * 0.9f);
            }
        }
        else
        {
            if (enemyBehavior != null && enemyBehavior.stamina < 10)
            {
                result = Mathf.RoundToInt(baseDamage * 0.8f);
            }
        }

        return result;
    }
}
