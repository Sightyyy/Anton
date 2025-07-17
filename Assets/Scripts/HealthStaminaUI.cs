using UnityEngine;
using UnityEngine.UI;

public class HealthStaminaUI : MonoBehaviour
{
    public PlayerBehavior player;
    public Slider healthSlider;
    public Slider staminaSlider;

    private void Update()
    {
        if (player == null) return;
        healthSlider.value = player.GetHealth();
        staminaSlider.value = player.GetStamina();
    }
}
