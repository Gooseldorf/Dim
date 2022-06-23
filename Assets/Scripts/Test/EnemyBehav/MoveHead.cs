using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveHead : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float sightRange = 5;

    [SerializeField] private bool playerInSightRange;
    
    
    [SerializeField] Transform obj1; // первая часть турели (Y)
    //[SerializeField] Transform obj2; // вторая часть турели (X)
    [SerializeField] Transform target; // цель
    [SerializeField] float speed; // скорость

    

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(obj1.rotation, Quaternion.LookRotation(target.position - obj1.position), 10 * Time.deltaTime * speed);
        transform.rotation = Quaternion.Euler(0, obj1.eulerAngles.y, transform.eulerAngles.z);

        //obj2.rotation = Quaternion.RotateTowards(obj2.rotation, Quaternion.LookRotation(target.position - obj2.position), 10 * Time.deltaTime * speed);
       // obj2.rotation = Quaternion.Euler(obj2.eulerAngles.x, obj1.eulerAngles.y, transform.eulerAngles.z);  
    
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, 7);
        
        //if(playerInSightRange) Look();
            
    }

    // private void Look()
    // {
    //     // The object whose rotation we want to match.
    //
    //     // Angular speed in degrees per sec.
    //     float speed = 10;
    //     
    //     Quaternion dir = Quaternion.Euler
    //         // The step size is equal to speed times frame time.
    //         var step = speed * Time.deltaTime;
    //
    //         // Rotate our transform a step closer to the target's.
    //         transform.rotation = Quaternion.RotateTowards(transform.rotation, player.rotation, step);
    //     
    // }
}
