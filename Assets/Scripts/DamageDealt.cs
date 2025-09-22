using UnityEngine;

public class DamageDealt : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 10;
    public string targetTag = "Enemy";

    private PlayerBehavior playerBehavior;

    void Awake()
    {
        // Cari PlayerBehavior dari parent (misalnya kalau script ini ada di weapon/collider child)
        playerBehavior = GetComponentInParent<PlayerBehavior>();
        if (playerBehavior == null)
        {
            Debug.LogWarning($"{gameObject.name} tidak menemukan PlayerBehavior di parent!");
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
                int finalDamage = CalculateDamage(damageAmount);
                enemy.TakeDamage(finalDamage);
            }
        }
    }

    private int CalculateDamage(int baseDamage)
    {
        int result = baseDamage;

        if (playerBehavior != null && playerBehavior.isWeakened)
        {
            result = Mathf.RoundToInt(baseDamage * 0.9f);
        }
        return result;
    }
}
