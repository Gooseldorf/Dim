using System;
using PlayerMovement;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;

    private void OnEnable()
    {
        FirstPersonController.OnDamage += UpdateHealthText;
        FirstPersonController.OnHeal += UpdateHealthText;

    }

    private void OnDisable()
    {
        FirstPersonController.OnDamage -= UpdateHealthText;
        FirstPersonController.OnHeal -= UpdateHealthText;
    }

    private void Start()
    {
        UpdateHealthText(100);
    }

    private void UpdateHealthText(float currentHealth)
    {
        healthText.text = $"HEALTH {currentHealth}";
    }
}
