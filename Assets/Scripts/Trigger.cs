using System;
using System.Collections;
using System.Collections.Generic;
using PlayerMovement;
using UnityEngine;
using Random = UnityEngine.Random;

public class Trigger : MonoBehaviour
{
    //private Action _setTriggerInactive;
    
    [SerializeField] private GameObject enemy;
    public GameObject spawner;
    [SerializeField] public bool canTrigger = true;
    [SerializeField] [Range(0f, 100f)] private float spawnChance = 50f;
    [SerializeField] [Range(0f, 1f)] private float flashLightOffMultiplier;
    [SerializeField] [Range(0f, 1f)] private float walkMultiplier;
    [SerializeField] [Range(0f, 1f)] private float crouchMultiplier;
    private Collider _player;
    private FirstPersonController _playerScript;
    private float _finalSpawnChance;
    private float _speedMultiplier;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
        _playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        _speedMultiplier = _playerScript.IsSprinting ? 1f : _playerScript.isCrouching ? crouchMultiplier : walkMultiplier;
        if (_playerScript.flashLightOn)
            _finalSpawnChance = _speedMultiplier * spawnChance;
        else if (!_playerScript.flashLightOn)
            _finalSpawnChance = _speedMultiplier * flashLightOffMultiplier * spawnChance;
    }

    private void OnEnable()
    {
        //EventManager.OnMonsterTrigger += SetInactive;
        //EventManager.OnSafeSpaceTrigger += SetActive;
    }
    private void OnDisable()
    {
        //EventManager.OnMonsterTrigger -= SetInactive;
        //EventManager.OnSafeSpaceTrigger -= SetActive;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (canTrigger && other == _player)
        {
            if (Random.value < _finalSpawnChance / 100f)
            {
                enemy.SetActive(true);
                Instantiate(enemy, spawner.transform.position, spawner.transform.rotation);
                EventManager.MonsterTrigger();
                SetInactive();
                StartCoroutine(TriggerCooling());
            }
        }
    }

    private IEnumerator TriggerCooling()
    {
        yield return new WaitForSeconds(10);
        SetActive();
        
    }

    private void SetInactive()
    {
        canTrigger = false;
    }

    private void SetActive()
    {
        canTrigger = true;
    }
}
