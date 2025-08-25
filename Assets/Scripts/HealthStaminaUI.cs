using UnityEngine;
using UnityEngine.UI;

public class HealthStaminaUI : MonoBehaviour
{
    public PlayerBehavior player;
    public Slider healthSlider;
    public Slider staminaSlider;

    private void Start()
    {
        if (player == null) return;

        healthSlider.minValue = 0;
        healthSlider.maxValue = 1;

        staminaSlider.minValue = 0;
        staminaSlider.maxValue = 100;
    }

    private void Update()
{
    if (player == null) return;

    float currentHp = player.health;
    float maxHp = player.maxHealth;

    float hpPercent = (maxHp > 0) ? currentHp / maxHp : 0f;

    healthSlider.value = hpPercent;

    staminaSlider.value = player.GetStamina();
}

}
