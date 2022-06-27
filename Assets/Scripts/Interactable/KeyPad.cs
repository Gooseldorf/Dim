using System;
using System.Collections;
using System.Collections.Generic;
using PlayerMovement;
using UnityEngine;
using UnityEngine.UI;

public class KeyPad : Interactable
{
    public static Action KeyPadInteracted;
    private FirstPersonController _player;
    public string correctPassword = "1408";
    private string _playerInput;
    [SerializeField] private AudioClip[] keyPadSounds;
    private AudioSource _keyPadAudioSource;
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
        if (_playerInput.Length >= 5)
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
        if (_playerInput == correctPassword)
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
