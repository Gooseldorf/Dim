using System;
using System.Collections;
using PlayerMovement;
using UnityEngine;
using UnityEngine.AI;

public class DoorScript : Interactable
{
    public static Action <string> DoorIsLocked;
    private NavMeshObstacle _obstacle;
    public bool isOpen = false;
    public bool isLocked = false;

    private Vector3 _startRotation;
    private Quaternion _closedPosition;
    private Quaternion _openedPosition;

    private Coroutine _animationCoroutine;

    [SerializeField] private AudioClip[] doorSounds;
    private AudioSource _doorAudioSource;
    
    private FirstPersonController _playerController;
    
    private void Awake()
    {
        
        _startRotation = transform.rotation.eulerAngles;
        _closedPosition = transform.rotation;
        _openedPosition = Quaternion.Euler(0, _startRotation.y + 90,0);
        _doorAudioSource = GetComponent<AudioSource>();
        _playerController = FindObjectOfType<FirstPersonController>();
        
        _obstacle = GetComponent<NavMeshObstacle>();
        _obstacle.carveOnlyStationary = true;
        _obstacle.carving = true;
        if (isLocked)
            _obstacle.enabled = true;
        else
            _obstacle.enabled = false;
    }

    public void Open()
    {
        if (isLocked)
        {
            if (CheckKey())
            {
                _doorAudioSource.PlayOneShot(doorSounds[3]);
                isLocked = false;
                DoorIsLocked?.Invoke("Used a key");

            }
            else
            {
                _doorAudioSource.PlayOneShot(doorSounds[2]);
                DoorIsLocked?.Invoke("Need a key");
            }
        }

        if (isOpen || isLocked) return;
        
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        
        _animationCoroutine = StartCoroutine(DoRotationOpen());
        _obstacle.enabled = true;
    }

    private IEnumerator DoRotationOpen()
    {
        _doorAudioSource.PlayOneShot(doorSounds[0]);
        isOpen = true;
        float time = 0;
        while (_closedPosition != _openedPosition)
        {
            transform.rotation = Quaternion.Slerp(_closedPosition,_openedPosition,time);
            yield return null;
            time += Time.deltaTime;
        }
    }

    public void Close()
    {
        _doorAudioSource.PlayOneShot(doorSounds[1]);
        if (!isOpen) return;
        
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(DoRotationClose());
        _obstacle.enabled = false;
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = _openedPosition;
        Quaternion endRotation = _closedPosition;
        
        isOpen = false;

        float time = 0;
        while (startRotation != endRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(_startRotation), time);
            yield return null;
            time += Time.deltaTime;
        }
    }

    private bool CheckKey()
    {
        switch (gameObject.tag)
        {
            case "QuestDoor/QuestDoor1":
                return _playerController.Inventory.HasKeyFromQuestDoor1;
            case "QuestDoor/QuestDoor2":
                return _playerController.Inventory.HasKeyFromQuestDoor2;
            case "QuestDoor/QuestDoor3":
                return _playerController.Inventory.HasKeyFromQuestDoor3;
            default: 
                return false;
        }

    }
    public override void OnInteract()
    {
        if (!gameObject.TryGetComponent<DoorScript>(out DoorScript door)) return;
        
        if (door.isOpen)
        {
            door.Close();
        }
        else
        {
            door.Open();
        }
    }

    public override void OnFocus()
    {
    }
    
    public override void OnLoseFocus()
    {
        DoorIsLocked?.Invoke("");
    }

  
}
