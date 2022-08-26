using System;
using PlayerMovement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactionText = default;
    [SerializeField] private TextMeshProUGUI lorePaperText = default;
    [SerializeField] private GameObject keypad = default;
    [SerializeField] private Image spawnChanceImage;
    [SerializeField] private TextMeshProUGUI ammoText = default;
    [SerializeField] private TextMeshProUGUI enemyCounter;
    [SerializeField] private GameObject exit;
    private int _numberOfEnemies;
    private int _enemiesLeft;
    private bool _isLevel2;
    public static Action Level2Completed;
    
    

    private void OnEnable()
    {
        Level2Handler.Level2started += Level2UI;
        LorePaperScript.Reading += UpdateLorePaperText;
        KeyScript.KeyOnFocus += UpdateInteractionText;
        KeyPad.KeyPadInteracted += ShowKeyPad;
        Trigger.SpawnChanceUpdate += UpdateSpawnChance;
        Weapon.AmmoChanged += UpdateAmmoText;
        EnemyBehavior.EnemyDown += UpdateEnemyCounter;
        DoorScript.DoorIsLocked += UpdateInteractionText;
    }

    private void OnDisable()
    {
        LorePaperScript.Reading -= UpdateLorePaperText;
        KeyScript.KeyOnFocus -= UpdateInteractionText;
        KeyPad.KeyPadInteracted -= ShowKeyPad;
        Trigger.SpawnChanceUpdate -= UpdateSpawnChance;
        Level2Handler.Level2started -= Level2UI;
        Weapon.AmmoChanged -= UpdateAmmoText;
        EnemyBehavior.EnemyDown -= UpdateEnemyCounter;
        DoorScript.DoorIsLocked -= UpdateInteractionText;

    }
    
    private void Level2UI()
    {
        _isLevel2 = true;
        _numberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        _enemiesLeft = _numberOfEnemies+1;
        spawnChanceImage.enabled = false;
        UpdateEnemyCounter();
    }
   
    public void UpdateInteractionText(string action)
    {
        interactionText.text = action;
    }
    public void UpdateLorePaperText(string text)
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
        if (_isLevel2)
        {
            _enemiesLeft -= 1;
            enemyCounter.text = $"Enemies: {_enemiesLeft}/{_numberOfEnemies}";
            if (_enemiesLeft == 0)
            {
                Level2Completed?.Invoke();
                exit.SetActive(true);
            }
        }
    }
}
