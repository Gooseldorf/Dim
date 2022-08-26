using System;
using PlayerMovement;

public class KeyScript : Interactable
{
    public static Action<string> KeyOnFocus;
    private Inventory _playerInventory;
    private FirstPersonController _playerController;

    private void Awake()
    {
        _playerController = FindObjectOfType<FirstPersonController>();
    }
    public override void OnInteract()
    {
        switch (gameObject.name)
        {
            case "KeyFromQuestDoor1":
                _playerController.Inventory.HasKeyFromQuestDoor1 = true;
                gameObject.SetActive(false);
                break;
            case "KeyFromQuestDoor2":
                _playerController.Inventory.HasKeyFromQuestDoor2 = true;
                gameObject.SetActive(false);
                break;
            case "KeyFromQuestDoor3":
                _playerController.Inventory.HasKeyFromQuestDoor3 = true;
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnFocus()
    {
        if (_playerController.IsZooming)
            KeyOnFocus?.Invoke("Take"); 
    }
    public override void OnLoseFocus()
    {
        KeyOnFocus?.Invoke("");
    }
}
