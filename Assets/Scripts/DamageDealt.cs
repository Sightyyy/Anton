using UnityEngine;

public class DamageDealt : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 10;

    [Tooltip("Tag dari target yang akan terkena damage, misalnya: 'Enemy'")]
    public string targetTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            var enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
        }
    }
}
