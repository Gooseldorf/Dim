using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveSpace : MonoBehaviour
{
    private GameObject enemy;
    private bool _canTrigger; 

    private void OnEnable()
    {
        EventManager.OnMonsterTrigger += SetActive;
    }

    private void OnDisable()
    {
        EventManager.OnMonsterTrigger -= SetActive;
    }
    
    private void SetActive()
    {
        _canTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_canTrigger)
        {
            _canTrigger = false;
            EventManager.SafeSpaceTrigger();
        }
    }
}
