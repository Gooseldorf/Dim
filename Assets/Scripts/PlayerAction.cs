using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Users;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private TextMeshPro UseText;

    [SerializeField] private Transform Camera;
    [SerializeField] private float maxUseDistance = 1f;
    [SerializeField] private LayerMask UseLayers;

    void OnUse()
    {
        if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, maxUseDistance, UseLayers))
        {
            if (hit.collider.TryGetComponent<DoorTest>(out DoorTest door))
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
    }

    private void Update()
    {
        if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, maxUseDistance, UseLayers) &&
            hit.collider.TryGetComponent<DoorTest>(out DoorTest door))
        {
            if (door.isOpen)
            {
                UseText.SetText("Close");
            }
            else
            {
                UseText.SetText("Open");
            }
            UseText.gameObject.SetActive(true);
            UseText.transform.position = hit.point - (hit.point - Camera.position).normalized * 0.8f;
            UseText.transform.rotation = Quaternion.LookRotation((hit.point - Camera.position).normalized);
        }
        else
        {
            UseText.gameObject.SetActive(false);
        }
    }
}
