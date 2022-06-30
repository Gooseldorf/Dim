using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DoorScript : Interactable
{
    private NavMeshObstacle _obstacle;
    public bool isOpen = false;
    public bool isLocked = false;
    [SerializeField] private bool isRotatingDoor = true;
    [SerializeField] private float speed = 1f;

    private Vector3 _startRotation1;
    private Quaternion _closedPosition;
    private Quaternion _openedPosition;

    private Coroutine _animationCoroutine;

    [SerializeField] private AudioClip[] doorSounds;
    private AudioSource _doorAudioSource;
    
    private void Awake()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
        _obstacle.carveOnlyStationary = false;
        _obstacle.carving = isOpen;
        _obstacle.enabled = isOpen;
        
        _startRotation1 = transform.rotation.eulerAngles;
        _closedPosition = transform.rotation;
        _openedPosition = Quaternion.Euler(0, _startRotation1.y + 90,0);
        _doorAudioSource = GetComponent<AudioSource>();
    }

    public void Open()
    {
        if (isLocked)
        {
            if (CheckKey())
            {
                _doorAudioSource.PlayOneShot(doorSounds[3]);
                isLocked = false;
            }
            else
            {
                _doorAudioSource.PlayOneShot(doorSounds[2]);
            }
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
        if (gameObject.TryGetComponent<DoorScript>(out DoorScript door))
        {
            if (door.isOpen)
            {
                door.Close();
            }
            else
            {
                door.Open();
            }
        }
    }

    public override void OnFocus()
    {
    }
    
    public override void OnLoseFocus()
    {
    }

  
}
