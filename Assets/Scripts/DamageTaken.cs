using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    [Tooltip("Script PlayerBehavior harus terpasang pada objek ini.")]
    private PlayerBehavior playerBehavior;
    private Animator animator;

    private float invulnTime = 0.2f; // waktu kebal setelah kena hit
    private float lastDamageTime;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        animator = GetComponentInChildren<Animator>();

        if (playerBehavior == null)
        {
            Debug.LogError("PlayerBehavior not found on this object!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found in children!");
        }
    }

    public void ApplyDamage(int amount)
    {
        if (playerBehavior == null) return;

        if (Time.time - lastDamageTime < invulnTime)
        {
            // Debug.Log("Damage diabaikan (invulnerable cooldown).");
            return;
        }

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Player Skill") || stateInfo.IsName("Player Ultimate"))
            {
                // Debug.Log("Damage diabaikan (sedang skill/ultimate).");
                return;
            }
        }
        int finalDamage = amount;
        if (playerBehavior.isInvulnerable)
        {
            finalDamage = 0;
        }
        else if (playerBehavior.isWeakened)
        {
            finalDamage = Mathf.RoundToInt(amount * 1.25f);
        }

        playerBehavior.SetHealth(playerBehavior.health - finalDamage);
        lastDamageTime = Time.time;
        // Debug.Log($"Player took {amount} damage. Current HP: {playerBehavior.health}");
    }
}
