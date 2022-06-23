using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class DoorTest : MonoBehaviour
{
    private NavMeshObstacle Obstacle;
    public bool isOpen = false;
    [SerializeField] 
    private bool isRotatingDoor = false;
    [SerializeField] 
    private float speed = 1f;
    [Header("Rotation Configs")] 
    //[SerializeField] private float rotationAmount = 90f;
    [SerializeField] private float forwardDirection = 0;

    private Vector3 startRotation1;
    private Quaternion ClosedPosition;
    private Quaternion OpenedPosition;
    private Vector3 forward;

    private Coroutine animationCoroutine;

    private void Awake()
    {
        Obstacle = GetComponent<NavMeshObstacle>();
        Obstacle.carveOnlyStationary = false;
        Obstacle.carving = isOpen;
        Obstacle.enabled = isOpen;
        
        startRotation1 = transform.rotation.eulerAngles;
        ClosedPosition = transform.rotation;
        OpenedPosition = Quaternion.Euler(0, startRotation1.y + 90,0);
        forward = transform.right;
        
        print($"{OpenedPosition}, {ClosedPosition}");
    }

    public void Open()
    {
        if (!isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor)
            {
                //float dot = Vector3.Dot(forward, (userPosition - transform.position).normalized);
                //Debug.Log($"Dot: {dot.ToString("N3")}");
                animationCoroutine = StartCoroutine(DoRotationOpen());
            }

            Obstacle.enabled = true;
            Obstacle.carving = true;
        }
    }

    private IEnumerator DoRotationOpen()
    {
        //Quaternion startRotation = transform.rotation;
        //Quaternion endRotation = OpenedPosition;
        
            //endRotation = Quaternion.Euler(new Vector3(0, startRotation.y - 90,0));
        
        
           // endRotation = Quaternion.Euler(new Vector3(0, startRotation.y + 90,0));


        isOpen = true;
        float time = 0;
        while (ClosedPosition != OpenedPosition)
        {
            transform.rotation = Quaternion.Slerp(ClosedPosition,OpenedPosition,time);
            yield return null;
            time += Time.deltaTime;
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor)
            {
                animationCoroutine = StartCoroutine(DoRotationClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        Obstacle.carving = false;
        Obstacle.enabled = false;
        
        Quaternion startRotation = OpenedPosition;
        Quaternion endRotation = ClosedPosition;
        
        isOpen = false;

        float time = 0;
        while (startRotation != endRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(startRotation1), time);
            yield return null;
            time += Time.deltaTime;
        }
    }
}
