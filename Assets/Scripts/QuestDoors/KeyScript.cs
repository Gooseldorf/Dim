using System;
using System.Collections;
using System.Collections.Generic;
using PlayerMovement;
using UnityEngine;

public class KeyScript : Interactable
{
    public static Action<string> KeyOnFocus;
    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    public override void OnInteract()
    {
        switch (gameObject.name)
        {
            case "KeyFromQuestDoor1":
                _player.GetComponent<Inventory>().hasKeyFromQuestDoor1 = true;
                gameObject.SetActive(false);
                break;
            case "KeyFromQuestDoor2":
                _player.GetComponent<Inventory>().hasKeyFromQuestDoor2 = true;
                gameObject.SetActive(false);
                break;
            case "KeyFromQuestDoor3":
                _player.GetComponent<Inventory>().hasKeyFromQuestDoor3 = true;
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnFocus()
    {
        KeyOnFocus?.Invoke("Take"); 
    }
    public override void OnLoseFocus()
    {
        KeyOnFocus?.Invoke("");
    }
}
