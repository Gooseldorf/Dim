using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorTrigger : MonoBehaviour
{
   [SerializeField] private DoorScript door;
   private int _agentsInRange = 0;

   private void OnTriggerEnter(Collider other)
   {
      if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
      {
         _agentsInRange++;
         if (!door.isOpen)
         {
            door.Open();
         }
      }
   }
}
