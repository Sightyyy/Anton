using UnityEngine;

public class DebuffGiver : MonoBehaviour
{
    [System.Flags]
    public enum DebuffType
    {
        None = 0,
        Weaken = 1 << 0,
        Burning = 1 << 1,
        Poisoned = 1 << 2,
        Slowed = 1 << 3,
        Knockbacked = 1 << 4
    }

    [Header("Debuff Settings")]
    public DebuffType debuffsToApply;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBehavior player = collision.GetComponent<PlayerBehavior>();
            if (player != null)
            {
                ApplyDebuffsToPlayer(player);
            }
        }
    }

    private void ApplyDebuffsToPlayer(PlayerBehavior player)
    {
        if ((debuffsToApply & DebuffType.Weaken) != 0)
        {
            Debug.Log("Applying Weaken");
            // player.ApplyWeaken();
        }
        if ((debuffsToApply & DebuffType.Burning) != 0)
        {
            Debug.Log("Applying Burning");
            // player.ApplyBurning();
        }
        if ((debuffsToApply & DebuffType.Poisoned) != 0)
        {
            Debug.Log("Applying Poisoned");
            // player.ApplyPoisoned();
        }
        if ((debuffsToApply & DebuffType.Slowed) != 0)
        {
            Debug.Log("Applying Slowed");
            // player.ApplySlowed();
        }
        if ((debuffsToApply & DebuffType.Knockbacked) != 0)
        {
            Debug.Log("Applying Knockbacked");
            // player.ApplyKnockbacked(transform);
        }
    }
}
