using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Trigger : MonoBehaviour
{
    //private Action _setTriggerInactive;
    
    private GameObject enemy;
    public GameObject spawner;
    [SerializeField] public bool CanTrigger = true;
    [SerializeField] [Range(0f, 100f)] private float spawnChance = 50f;

    private void OnEnable()
    {
        EventManager.OnMonsterTrigger += SetInactive;
        EventManager.OnSafeSpaceTrigger += SetActive;
    }
    private void OnDisable()
    {
        EventManager.OnMonsterTrigger -= SetInactive;
        EventManager.OnSafeSpaceTrigger -= SetActive;
    }

    private void Awake()
    {
        enemy = GameObject.Find("Ghoul");
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (CanTrigger)
        {
            if (Random.value < spawnChance / 100f)
            {
                enemy.SetActive(true);
                Instantiate(enemy, spawner.transform.position, spawner.transform.rotation);
                EventManager.MonsterTrigger();
            }
        }
    }

    private void SetInactive()
    {
        CanTrigger = false;
    }

    private void SetActive()
    {
        CanTrigger = true;
    }
}
