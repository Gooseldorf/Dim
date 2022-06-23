using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    private void OnCrouch()
    {
        HandleCrouch();
    }
    [SerializeField]private CharacterController controller;
    [SerializeField] private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && controller.isGrounded;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Crouch params")] 
    [SerializeField] private float crouchingHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private IEnumerator CrouchStand()
    {
        duringCrouchAnimation = true;
        float timeElapsed = 0;
        float targetHight = isCrouching ? standingHeight : crouchingHeight;
        float currentHight = controller.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHight, targetHight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;
        duringCrouchAnimation = false;
    }



}
