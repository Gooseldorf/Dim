using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorTrigger : MonoBehaviour
{
   [SerializeField] private DoorScript door;
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent) && !door.isLocked)
      {
         if (!door.isOpen)
         {
            door.Open();
         }
      }
   }
}
