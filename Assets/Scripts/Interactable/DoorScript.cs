using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.VisualScripting;
public class DoorScript : Interactable
{
    private NavMeshObstacle _obstacle;
    public bool isOpen = false;
    public bool isLocked = false;
    public bool isOnFocus;
    [SerializeField] private bool isRotatingDoor = true;
    [SerializeField] private float speed = 1f;

    private Vector3 _startRotation1;
    private Quaternion _closedPosition;
    private Quaternion _openedPosition;

    private Coroutine _animationCoroutine;

    
    private void Awake()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
        _obstacle.carveOnlyStationary = false;
        _obstacle.carving = isOpen;
        _obstacle.enabled = isOpen;
        
        _startRotation1 = transform.rotation.eulerAngles;
        _closedPosition = transform.rotation;
        _openedPosition = Quaternion.Euler(0, _startRotation1.y + 90,0);
    }

    public void Open()
    {
        if (isLocked)
        {
            if (CheckKey())
            {
                isLocked = false;
            }
        }
        else
        {
            print("locked");
        }
        if (!isOpen && !isLocked)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            if (isRotatingDoor)
            {
                _animationCoroutine = StartCoroutine(DoRotationOpen());
            }

            _obstacle.enabled = true;
            _obstacle.carving = true;
        }
    }

    private IEnumerator DoRotationOpen()
    {
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
        if (isOpen)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            if (isRotatingDoor)
            {
                _animationCoroutine = StartCoroutine(DoRotationClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        _obstacle.carving = false;
        _obstacle.enabled = false;
        
        Quaternion startRotation = _openedPosition;
        Quaternion endRotation = _closedPosition;
        
        isOpen = false;

        float time = 0;
        while (startRotation != endRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(_startRotation1), time);
            yield return null;
            time += Time.deltaTime;
        }
    }

    private bool CheckKey()
    {
        if (gameObject.CompareTag("QuestDoor/QuestDoor1"))
            return GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().hasKeyFromQuestDoor1;
        else if (gameObject.CompareTag("QuestDoor/QuestDoor2"))
            return GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().hasKeyFromQuestDoor2;
        else if (gameObject.CompareTag("QuestDoor/QuestDoor3"))
            return GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().hasKeyFromQuestDoor3;
        else 
            return false;
    }
    public override void OnInteract()
    {
        print($"INTERACTION!");
        if (gameObject.TryGetComponent<DoorScript>(out DoorScript door))
        {
            if (door.isOpen)
            {
                door.Close();
                print($"CLOSING!");

            }
            else
            {
                door.Open();
                print($"OPENING!");

            }
        }
    }

    public override void OnFocus()
    {
        isOnFocus = true;
    }
    public override void OnLoseFocus()
    {
        isOnFocus = false;
    }
}
