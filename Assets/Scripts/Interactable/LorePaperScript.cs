using System;
using System.Collections;
using System.Collections.Generic;
using PlayerMovement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LorePaperScript : Interactable
{
    private FirstPersonController _player;
    public static Action<string> Reading;
    private bool _playerIsReading;
    [SerializeField] private Text lorePaperText;
    

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }
    
    
    public override void OnInteract()
    {
    }

    public override void OnFocus()
    {
        if (_player.IsZooming)
            Reading?.Invoke(lorePaperText.text);
    }

    public override void OnLoseFocus()
    {
        Reading?.Invoke("");
    }
}
