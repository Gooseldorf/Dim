using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveSpace : MonoBehaviour
{
    private GameObject enemy;
    
    private void OnTriggerEnter(Collider other)
    {
        EventManager.SafeSpaceTrigger();

    }
}
