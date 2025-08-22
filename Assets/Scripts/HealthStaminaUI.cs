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
        healthSlider.maxValue = 1000;

        staminaSlider.minValue = 0;
        staminaSlider.maxValue = 100;
    }

    private void Update()
    {
        if (player == null) return;

        healthSlider.value = player.GetHealth();
        staminaSlider.value = player.GetStamina();
    }
}
