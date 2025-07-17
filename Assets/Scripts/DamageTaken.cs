using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    [Tooltip("Script PlayerBehavior harus terpasang pada objek ini.")]
    private PlayerBehavior playerBehavior;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        if (playerBehavior == null)
        {
            Debug.LogError("PlayerBehavior not found on this object!");
        }
    }

    public void ApplyDamage(int amount)
    {
        if (playerBehavior != null)
        {
            playerBehavior.SetHealth(playerBehavior.health - amount);
            Debug.Log($"Player took {amount} damage. Current HP: {playerBehavior.health}");
        }
    }
}
