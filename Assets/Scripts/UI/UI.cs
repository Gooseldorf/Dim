using System;
using PlayerMovement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI interactionText = default;
    [SerializeField] private TextMeshProUGUI lorePaperText = default;
    [SerializeField] private GameObject keypad = default;
    [SerializeField] private Image spawnChanceImage;
    [SerializeField] private TextMeshProUGUI ammoText = default;
    [SerializeField] private TextMeshProUGUI enemyCounter;
    [SerializeField] private GameObject exit;
    private int numberOfEnemies;
    private int enemiesLeft;
    public static Action Level2Completed;
    
    

    private void OnEnable()
    {
        Level2Handler.Level2started += Level2UI;
        FirstPersonController.OnDamage += UpdateHealthText;
        FirstPersonController.OnHeal += UpdateHealthText;
        LorePaperScript.Reading += UpdateLorePaperText;
        KeyScript.KeyOnFocus += UpdateInteractionText;
        KeyPad.KeyPadInteracted += ShowKeyPad;
        Trigger.SpawnChanceUpdate += UpdateSpawnChance;
        Weapon.AmmoChanged += UpdateAmmoText;
        EnemyBehavior.EnemyDown += UpdateEnemyCounter;
    }

    private void OnDisable()
    {
        FirstPersonController.OnDamage -= UpdateHealthText;
        FirstPersonController.OnHeal -= UpdateHealthText;
        LorePaperScript.Reading -= UpdateLorePaperText;
        KeyScript.KeyOnFocus -= UpdateInteractionText;
        KeyPad.KeyPadInteracted -= ShowKeyPad;
        Trigger.SpawnChanceUpdate -= UpdateSpawnChance;
        Level2Handler.Level2started -= Level2UI;
        Weapon.AmmoChanged -= UpdateAmmoText;
        EnemyBehavior.EnemyDown -= UpdateEnemyCounter;


    }
    
    private void Start()
    {
        UpdateHealthText(100);
        numberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemiesLeft = numberOfEnemies+1;
        UpdateEnemyCounter();
    }

    private void Level2UI()
    {
        spawnChanceImage.enabled = false;
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

    private void UpdateSpawnChance(float spawnChance)
    {
        spawnChanceImage.fillAmount = spawnChance / 100;
    }

    private void UpdateAmmoText(int ammoLoad, int maxAmmo)
    {
        ammoText.text = $"Ammo : {ammoLoad}/{maxAmmo}";
    }

    private void UpdateEnemyCounter()
    {
        enemiesLeft -= 1;
        enemyCounter.text = $"Enemies: {enemiesLeft}/{numberOfEnemies}";
        if (enemiesLeft == 0)
        {
            Level2Completed?.Invoke();
            exit.SetActive(true);
        }
    }
}
