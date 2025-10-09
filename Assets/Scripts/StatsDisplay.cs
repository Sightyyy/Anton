using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    [Header("Assign in Inspector (recommended)")]
    public Slider hpSlider;
    public TMP_Text hpPercentage;
    private int lastHpPercent = -1;

    void Awake()
    {
        if (hpSlider == null)
        {
            Slider[] sliders = GetComponentsInChildren<Slider>(true);
            if (sliders.Length > 0 && hpSlider == null) hpSlider = sliders[0];
        }

        if (hpPercentage == null)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length > 0 && hpPercentage == null) hpPercentage = texts[0];
        }

        if (hpSlider == null) Debug.LogWarning("StatsDisplay: hpSlider tidak di-assign!");
        if (hpPercentage == null) Debug.LogWarning("StatsDisplay: hpPercentage TMP tidak di-assign!");
    }

    void Update()
    {
        UpdateHPPercentage();
    }

    private void UpdateHPPercentage()
    {
        if (hpSlider == null || hpPercentage == null) return;
        float normalized = (hpSlider.maxValue > 0f) ? (hpSlider.value / hpSlider.maxValue) : 0f;
        int percent = Mathf.RoundToInt(normalized * 100f);

        if (percent != lastHpPercent)
        {
            lastHpPercent = percent;
            hpPercentage.text = percent.ToString() + "%";
        }
    }
}
