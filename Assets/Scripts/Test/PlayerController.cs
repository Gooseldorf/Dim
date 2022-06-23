using System.Collections;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    
    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    
    [Header("Zoom parameters")] 
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomRoutine;
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private InputManager inputManager;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        MoveAndLook();
        Jump();
        Zoom();
    }

    private void MoveAndLook()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * walkSpeed);
    }

    private void Jump()
    {
        if (inputManager.HasJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void Zoom()
    {
        //if (inputManager.IsZooming())
            print("test");
        if (inputManager.ReverseZooming())
            print("reverse");
    }

    // private void Zoom()
    // {
    //     if (inputManager.Zooming().)
    //     {
    //         if (zoomRoutine != null)
    //         {
    //             StopCoroutine(zoomRoutine);
    //             zoomRoutine = null;
    //         }
    //
    //         zoomRoutine = StartCoroutine(ToggleZoom(true));
    //     }
    //     if (Input.GetKeyUp(zoomKey))
    //     {
    //         if (zoomRoutine != null)
    //         {
    //             StopCoroutine(zoomRoutine);
    //             zoomRoutine = null;
    //         }
    //
    //         zoomRoutine = StartCoroutine(ToggleZoom(false));
    //     }
    // }
    //
    // private IEnumerator ToggleZoom(bool isEnter)
    // {
    //     float targetFOV = isEnter ? zoomFOV : defaultFOV;
    //     float startingFOV = _playerCamera.fieldOfView;
    //     float timeElapsed = 0;
    //
    //     while (timeElapsed < timeToZoom)
    //     {
    //         _playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }
    //
    //     _playerCamera.fieldOfView = targetFOV;
    //     zoomRoutine = null;
    // }

}
