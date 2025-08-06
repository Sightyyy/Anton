using UnityEngine;

public class DamageDealt : MonoBehaviour
{
    public int damageAmount = 10;
    public string targetTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag))
            return;

        if (targetTag == "Enemy")
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
        }
        else if (targetTag == "Player")
        {
            PlayerBehavior player = collision.GetComponent<PlayerBehavior>();
            if (player != null)
            {
                int finalDamage = damageAmount;

                if (player.isWeakened)
                {
                    finalDamage = Mathf.FloorToInt(damageAmount * 0.9f);
                }

                player.SetHealth(player.GetHealth() - finalDamage);
            }
        }
    }
}
