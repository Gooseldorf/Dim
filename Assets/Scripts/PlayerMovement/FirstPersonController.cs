using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PlayerMovement
{
    public class FirstPersonController : MonoBehaviour
    {
        public bool CanMove { get; private set; } = true; 
        public bool IsSprinting => canSprint && Input.GetKey(sprintKey);
        private bool ShouldJump => Input.GetKeyDown(jumpKey) && _characterController.isGrounded && !IsSliding;
        private bool ShouldCrouch => Input.GetKeyDown(crouchKey) &&_characterController.isGrounded && !_duringCrouchAnimation;
        private bool ToggleFlashLight => Input.GetKeyDown(flashLightKey);
    
        [Header("Functional parameters")] 
        [SerializeField] private bool canSprint = true;
        [SerializeField] private bool canJump = true;
        [SerializeField] private bool canCrouch = true;
        [SerializeField] private bool canUseHeadBob = true;
        [SerializeField] private bool willSlideOnSlopes = true;
        [SerializeField] private bool canZoom = true;
        [SerializeField] private bool canInteract = true;
        [SerializeField] private bool useFootsteps = true;
    

        [Header("Controls")] 
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private KeyCode flashLightKey = KeyCode.F;

        [Header("Health")] 
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float timeBeforeRegeneration = 3f;
        [SerializeField] private float healthValueIncrement = 1f;
        [SerializeField] private float healthTimeIncrement = 0.1f;
        private float _currentHealth;
        private Coroutine _healthRegeneration;
        private bool _canRegenerate;
        public static Action<float> OnTakeDamage;
        public static Action<float> OnDamage;
        public static Action<float> OnHeal;
        
        [Header("Moving")] 
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float sprintSpeed = 6.0f;
        [SerializeField] private float crouchSpeed = 1.5f;
        [SerializeField] private float slopeSpeed = 5.0f;
        private CharacterController _characterController;
        private Vector3 _moveDirection;
        private Vector2 _currentInput;
        private float _rotationX;

        [Header("Look")] 
        [SerializeField, Range(1,10)] private float sensitivityX = 2.0f;
        [SerializeField, Range(1, 10)] private float sensitivityY = 2.0f;
        [SerializeField, Range(1,180)] private float upperLookLimit = 90.0f;
        [SerializeField, Range(1,180)] private float lowerLookLimit = 90.0f;
        private Camera _playerCamera;


        [Header("Jump")] 
        [SerializeField] private float jumpForce = 8.0f;
        [SerializeField] private float gravity = 30f;
    
        [Header("Crouch")] 
        [SerializeField] private float crouchingHeight = 0.5f;
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float timeToCrouch = 0.25f;
        [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
        [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
        public bool isCrouching;
        private bool _duringCrouchAnimation;

        [Header("Headbob")] 
        [SerializeField] private float walkBobSpeed = 14f;
        [SerializeField] private float walkBobAmount = 0.05f;
        [SerializeField] private float sprintBobSpeed = 18f;
        [SerializeField] private float sprintBobAmount = 0.1f;
        [SerializeField] private float crouchBobSpeed = 8f;
        [SerializeField] private float crouchBobAmount = 0.025f;
        private float _defaultYpos = 0;
        private float _timer;

        [Header("Zoom")] 
        [SerializeField] private float timeToZoom = 0.3f;
        [SerializeField] private float zoomFOV = 30f;
        private float _defaultFOV;
        private Coroutine _zoomRoutine;
        
        [Header("Interaction")] 
        [SerializeField] private Vector3 interactionRayPoint = default;
        [SerializeField] private float interactionDistance = default;
        [SerializeField] private LayerMask interactionLayer = default;
        private Interactable _currentInteractable;
        
        [Header("Footsteps")] 
        [SerializeField] private float baseStepSpeed = 0.5f;
        [SerializeField] private float crouchStepMultiplier = 1.5f;
        [SerializeField] private float sprintStepMultiplier = 0.6f;
        [SerializeField] private AudioSource footstepAudioSource = default;
        [SerializeField] private AudioClip[] grassClips = default;
        [SerializeField] private AudioClip[] stoneClips = default;
        private float _footstepTimer = 0;
        private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier :
            IsSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

        [Header("Audio")] 
        [SerializeField] private AudioClip onDamage;
        [SerializeField] private AudioClip onHit;
        [SerializeField] private AudioClip heartbeat;
        [SerializeField] private AudioClip breathing;
        [SerializeField] private AudioClip[] flashLight = default;
        [SerializeField] private float heartbeatOffset = 0.7f;
        private float _heartbeatTimer = 0;
        private float _breathTimer = 0;

        //Post Processing
        private PostProcessVolume _postProcessVolume;
        
        //Flashlight
        public bool canUseFlashlight = true;
        private Light _flashLight;
        public bool flashLightOn;
        private float _maxBlinkingSpeed = 0.1f;
        
        //UI
        private GameObject _crossHair;
        
        
        //SLIDING
        private Vector3 _hitPointNormal;
        private bool IsSliding
        {
            get
            {
                if (_characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
                {
                    _hitPointNormal = slopeHit.normal;
                    return Vector3.Angle(_hitPointNormal, Vector3.up) > _characterController.slopeLimit;
                }
                else
                {
                    return false;
                }
            }
        }


        private void OnEnable()
        {
            OnTakeDamage += ApplyDamage;
        }

        private void OnDisable()
        {
            OnTakeDamage -= ApplyDamage;
        }

        void Awake()
        {
            _playerCamera = GetComponentInChildren<Camera>();
            _characterController = GetComponent<CharacterController>();
            _defaultYpos = _playerCamera.transform.localPosition.y;
            _defaultFOV = _playerCamera.fieldOfView;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _currentHealth = maxHealth;
            _postProcessVolume = _playerCamera.GetComponent<PostProcessVolume>();
            _flashLight = GetComponentInChildren<Light>();
            _flashLight.enabled = false;
            flashLightOn = false;
            _crossHair = GameObject.Find("CrossHair");
            _crossHair.SetActive(false);
        }

        private void Start()
        {
            StartCoroutine(FlashLightBlinking());
        }

        void Update()
        {
            if (CanMove)
            {
                HandleMouseLook();
                HandleMoveInput();
                if(_currentHealth<maxHealth)
                    PlayRegenEffects();
                if (useFootsteps)
                    HandleFootSteps();
                if(canJump)
                    HandleJump();
                if (canCrouch)
                    HandleCrouch();
                if (canUseHeadBob)
                    HandleHeadBob();
                if (canZoom)
                    HandleZoom();
                if (canInteract)
                {
                    HandleInteractionCheck();
                    HandleInteractionInput();
                }
                if(canUseFlashlight)
                    HandleFlashLight();
                
                ApplyFinalMovements();
            }
        }
        
        

        private void HandleMoveInput()
        {
            _currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

            float moveDirectionY = _moveDirection.y;
            _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + (transform.TransformDirection(Vector3.right) * _currentInput.y);
            _moveDirection.y = moveDirectionY;
            
        }

        private void HandleMouseLook()
        {
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityY;
            _rotationX = Mathf.Clamp(_rotationX, -upperLookLimit, lowerLookLimit);
            _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX,0,0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivityX,0);
        }

        private void HandleJump()
        {
            if (ShouldJump)
                _moveDirection.y = jumpForce;
        }

        private void HandleCrouch()
        {
            if (ShouldCrouch)
            {
                StartCoroutine(CrouchStand());
            }
        }
        
        private void HandleHeadBob()
        {
            if(!_characterController.isGrounded) return;

            if (Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f)
            {
                _timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
                _playerCamera.transform.localPosition = new Vector3(
                    _playerCamera.transform.localPosition.x,
                    _defaultYpos + Mathf.Sin(_timer) *
                    (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                    _playerCamera.transform.localPosition.z);
            }
        }

        private void HandleZoom()
        {
            if (Input.GetKeyDown(zoomKey))
            {
                if (_zoomRoutine != null)
                {
                    StopCoroutine(_zoomRoutine);
                    _zoomRoutine = null;
                }
                _crossHair.SetActive(true);
                _zoomRoutine = StartCoroutine(ToggleZoom(true));
            }
            if (Input.GetKeyUp(zoomKey))
            {
                if (_zoomRoutine != null)
                {
                    StopCoroutine(_zoomRoutine);
                    _zoomRoutine = null;
                }
                _crossHair.SetActive(false);
                _zoomRoutine = StartCoroutine(ToggleZoom(false));
            }
        }
        
        private void HandleInteractionCheck()
        {
            if (Physics.Raycast(_playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit,
                    interactionDistance))
            {
                if (hit.collider.gameObject.layer == 11 && (_currentInteractable == null || hit.collider.gameObject.GetInstanceID() != _currentInteractable.GetInstanceID()))
                {
                    hit.collider.TryGetComponent (out _currentInteractable);
                    if(_currentInteractable)
                        _currentInteractable.OnFocus();
                }
            }
            else if (_currentInteractable)
            {
                _currentInteractable.OnLoseFocus();
                _currentInteractable = null;
            }
        }

        private void HandleInteractionInput()
        {
            if (Input.GetKeyDown(interactKey) && _currentInteractable != null && Physics.Raycast(
                    _playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance,
                    interactionLayer))
            {
                _currentInteractable.OnInteract();
            }
        }
        
        private void HandleFootSteps()
        {
            if (!_characterController.isGrounded) return;
            if(_currentInput == Vector2.zero) return;

            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer <= 0)
            {
                if (Physics.Raycast(_characterController.transform.position, Vector3.down, out RaycastHit hit, 4))
                {
                    switch (hit.collider.tag)
                    {
                        case "Footsteps/stone":
                            print("stone");
                            footstepAudioSource.PlayOneShot(stoneClips[Random.Range(0, stoneClips.Length-1)]);
                            break;
                        case "Footsteps/grass":
                            footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length-1)]);
                            break;
                        default: 
                            break;
                    }
                }
                _footstepTimer = GetCurrentOffset;
            }
        }
        
        private void ApplyFinalMovements()
        {
            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= gravity * Time.deltaTime;
            }

            if (willSlideOnSlopes && IsSliding)
                _moveDirection += new Vector3(_hitPointNormal.x, -_hitPointNormal.y, _hitPointNormal.z);

            _characterController.Move(_moveDirection * (Time.deltaTime * slopeSpeed));
        }

        private void ApplyDamage(float damage)
        {
            footstepAudioSource.PlayOneShot(onHit);
            footstepAudioSource.PlayOneShot(onDamage);
            if(flashLightOn)
                StartCoroutine(FlashLightBlinking());
            _canRegenerate = false;
            
            _currentHealth -= damage;
            OnDamage?.Invoke(_currentHealth);
            
            if(_currentHealth <= 0)
                KillPlayer();
            
            StartCoroutine(RegenerateHealth());
        }
        private void KillPlayer()
        {
            _currentHealth = 0;
            if (_healthRegeneration != null)
                StopCoroutine(RegenerateHealth());
            print("DEAD");
        }
        private void PlayRegenEffects()
        {
            if (_currentHealth != 0) 
                _postProcessVolume.weight = (maxHealth - _currentHealth) / 100;
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0)
            {
                footstepAudioSource.PlayOneShot(heartbeat);
                _heartbeatTimer = heartbeatOffset;
            }
            HardBreathing();
        }
        private void HandleFlashLight()
        {
            if (ToggleFlashLight && flashLightOn)
            {
                footstepAudioSource.PlayOneShot(flashLight[1]);
                _flashLight.enabled = false;
                flashLightOn = false;
            }
            else if (ToggleFlashLight && !flashLightOn)
            {
                footstepAudioSource.PlayOneShot(flashLight[0]);
                _flashLight.enabled = true;
                flashLightOn = true;

                
            }
        }
        private void HardBreathing()
        {
            _breathTimer -= Time.deltaTime;
            if (_breathTimer <= 0)
            {
                footstepAudioSource.PlayOneShot(breathing);
                _breathTimer = breathing.length;
            }
        }
        

        //--------------- Coroutines
        private IEnumerator CrouchStand()
        {
            if(isCrouching && Physics.Raycast(_playerCamera.transform.position,Vector3.up,1.0f))
                yield break;
        
            _duringCrouchAnimation = true;
            
            float timeElapsed = 0;
            float targetHeight = isCrouching ? standingHeight : crouchingHeight;
            float currentHeight = _characterController.height;
            Vector3 targetCenter = isCrouching ?  standingCenter : crouchingCenter;
            Vector3 currentCenter = _characterController.center;

            while (timeElapsed < timeToCrouch)
            {
                _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
                _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _characterController.height = targetHeight;
            _characterController.center = targetCenter;
            isCrouching = !isCrouching;
        
            _duringCrouchAnimation = false;
        }
        private IEnumerator ToggleZoom(bool isEnter)
        {
            float targetFOV = isEnter ? zoomFOV : _defaultFOV;
            float startingFOV = _playerCamera.fieldOfView;
            float timeElapsed = 0;

            while (timeElapsed < timeToZoom)
            {
                _playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _playerCamera.fieldOfView = targetFOV;
            _zoomRoutine = null;
        }
        private IEnumerator RegenerateHealth()
        {
            canSprint = false;
            yield return new WaitForSeconds(timeBeforeRegeneration);

            _canRegenerate = true;
            if (_canRegenerate)
            {
                WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

                while (_currentHealth < maxHealth && _canRegenerate)
                {
                    _currentHealth += healthValueIncrement;
                    
                    if (_currentHealth > maxHealth)
                        _currentHealth = maxHealth;
                    OnHeal?.Invoke(_currentHealth);

                    yield return timeToWait;
                }

                if (Math.Abs(_currentHealth - maxHealth) < 0.1f)
                    canSprint = true;
                _healthRegeneration = null;
                
            }
        }
        private IEnumerator FlashLightBlinking()
        {
            for (int i = 0; i < 7; i++)
            {
                _flashLight.enabled = false;
                yield return new WaitForSeconds(Random.Range(0,_maxBlinkingSpeed));
                
                _flashLight.enabled = true;
                yield return new WaitForSeconds(Random.Range(0,_maxBlinkingSpeed));
            }
            _flashLight.enabled = false;
        }
    }
}
