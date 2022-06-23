using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtention : CinemachineExtension
{
    [SerializeField] private float clampAngle = 80;
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;
    
    private InputManager inputManager;
    private Vector3 startingRotation;
    protected override void Awake()
    {
        inputManager = InputManager.Instance;
        startingRotation = transform.localRotation.eulerAngles;
        base.Awake();
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Aim)
        {
            Vector2 deltaInput = inputManager.GetMouseDelta();
            startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
            startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
            startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);
            state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
            
        }
    }
}
