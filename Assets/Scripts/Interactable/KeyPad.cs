using System;
using PlayerMovement;
using UnityEngine;

public class KeyPad : Interactable
{
    private FirstPersonController _player;

    [SerializeField] private int correctPassword = 1408;
    private string _playerInput;
    
    [SerializeField] private AudioClip[] keyPadSounds;
    private AudioSource _keyPadAudioSource;
    
    
    public static Action KeyPadInteracted;
    public static Action LevelCompleted;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        _keyPadAudioSource = gameObject.GetComponent<AudioSource>();
    }
    public override void OnInteract()
    {
        KeyPadInteracted?.Invoke();
        _player.CanMove = false;
    }

    public void PlayerKeyPadInput(int number)
    {
        _playerInput += number.ToString();
        if (_playerInput.Length > correctPassword.ToString().Length)
        {
            _keyPadAudioSource.PlayOneShot(keyPadSounds[1]);
           Clear();
        }
    }
    public void Clear()
    {
        _playerInput = "";
    }

    public void CheckPassword()
    {
        if (!int.TryParse(_playerInput, out var parsedPlayerInput)) return;
        
        if (parsedPlayerInput == correctPassword) 
        {
            _keyPadAudioSource.PlayOneShot(keyPadSounds[0]);
            Clear();
            LevelCompleted?.Invoke();
        }
        else
        {
            _keyPadAudioSource.PlayOneShot(keyPadSounds[1]);
            Clear();
        }
    }
    public override void OnFocus()
    {
    }

    public override void OnLoseFocus()
    {
    }
}


