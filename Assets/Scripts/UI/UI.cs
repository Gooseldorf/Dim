using System;
using PlayerMovement;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI interactionText = default;
    [SerializeField] private TextMeshProUGUI lorePaperText = default;
    [SerializeField] private GameObject keypad = default;
    

    private void OnEnable()
    {
        FirstPersonController.OnDamage += UpdateHealthText;
        FirstPersonController.OnHeal += UpdateHealthText;
        LorePaperScript.Reading += UpdateLorePaperText;
        KeyScript.KeyOnFocus += UpdateInteractionText;
        KeyPad.KeyPadInteracted += ShowKeyPad;
    }

    private void OnDisable()
    {
        FirstPersonController.OnDamage -= UpdateHealthText;
        FirstPersonController.OnHeal -= UpdateHealthText;
        LorePaperScript.Reading -= UpdateLorePaperText;
        KeyScript.KeyOnFocus -= UpdateInteractionText;
        KeyPad.KeyPadInteracted -= ShowKeyPad;
    }

    private void Start()
    {
        UpdateHealthText(100);
    }
    private void UpdateHealthText(float currentHealth)
    {
        healthText.text = $"HEALTH {currentHealth}";
    }
    private void UpdateInteractionText(string action)
    {
        interactionText.text = action;
    }
    private void UpdateLorePaperText(string text)
    {
        lorePaperText.text = text;
    }

    private void ShowKeyPad()
    {
        keypad.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideKeyPad()
    {
        keypad.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
