using UnityEngine;
using UnityEngine.InputSystem;

namespace KittyTerror.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonStateMachineController : MonoBehaviour
    {

        //comentario muestra


        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float crouchSpeed = 3.2f;
        [SerializeField] private float gravity = -20f;

        [Header("Crouch")]
        [SerializeField] private float crouchHeight = 1.1f;
        [SerializeField] private float crouchCameraLocalY = 1f;

        [Header("Camera")]
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float stickSensitivity = 180f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        [Header("Vitals")]
        [SerializeField] private int maxLives = 3;
        [SerializeField] private int currentLives = 3;

        [Header("Debug / Events")]
        [SerializeField] private CatStateMachineController externalAttackCat;
        [SerializeField] private bool enableCrouchKeyAttackTrigger = true;
        [SerializeField] private Key externalAttackKey = Key.K;

        private CharacterController _characterController;
        private PlayerStateMachine _stateMachine;
        private InputSystem_Actions _inputActions;

        private IdleState _idleState;
        private MoveState _moveState;
        private CrouchState _crouchState;

        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _crouchHeld;

        private float _pitch;
        private float _verticalVelocity;
        private bool _movementLocked;
        private float _movementLockUntil;

        private float _standingHeight;
        private Vector3 _standingCenter;
        private float _standingCameraLocalY;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _inputActions = new InputSystem_Actions();

            _idleState = new IdleState(this);
            _moveState = new MoveState(this);
            _crouchState = new CrouchState(this);
            _stateMachine = new PlayerStateMachine(_idleState);

            _standingHeight = _characterController.height;
            _standingCenter = _characterController.center;
            currentLives = Mathf.Clamp(currentLives, 0, maxLives);
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        private void OnDestroy()
        {
            _inputActions?.Dispose();
        }

        private void Start()
        {
            if (cameraPivot == null)
            {
                cameraPivot = GetComponentInChildren<Camera>()?.transform;
            }

            if (cameraPivot != null)
            {
                _standingCameraLocalY = cameraPivot.localPosition.y;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            RefreshMovementLock();
            ReadInput();
            TryTriggerExternalCatAttack();
            SetCrouch(_crouchHeld);
            RotateFromLook();

            if (!_movementLocked)
            {
                _stateMachine.Tick();
            }
            ApplyGravity();
        }

        private void ReadInput()
        {
            _moveInput = _inputActions.Player.Move.ReadValue<Vector2>().normalized;
            _lookInput = _inputActions.Player.Look.ReadValue<Vector2>();
            _crouchHeld = _inputActions.Player.Crouch.IsPressed();

            if (_movementLocked)
            {
                _moveInput = Vector2.zero;
                _crouchHeld = false;
            }
        }

        private void RotateFromLook()
        {
            if (_lookInput.sqrMagnitude <= 0.0001f) return;

            bool usingMouse = Mouse.current != null && Mouse.current.wasUpdatedThisFrame;
            float lookMultiplier = usingMouse ? mouseSensitivity : stickSensitivity * Time.deltaTime;

            float yaw = _lookInput.x * lookMultiplier;
            float pitchDelta = _lookInput.y * lookMultiplier;

            transform.Rotate(Vector3.up * yaw);

            _pitch = Mathf.Clamp(_pitch - pitchDelta, minPitch, maxPitch);
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            _verticalVelocity += gravity * Time.deltaTime;
            _characterController.Move(Vector3.up * (_verticalVelocity * Time.deltaTime));
        }

        public bool HasMovementInput => _moveInput.sqrMagnitude > 0.01f;
        public bool WantsToCrouch => _crouchHeld;
        public int CurrentLives => currentLives;
        public bool IsMovementLocked => _movementLocked;

        public void SetCameraPivot(Transform pivot)
        {
            cameraPivot = pivot;
        }

        /*
         * FSM de locomoción:
         *
         *                (WantsToCrouch)
         *        +------------------------------+
         *        |                              v
         *    +-------+   (HasMovementInput)   +-------+
         *    | Idle  | ---------------------> | Move  |
         *    +-------+ <--------------------- +-------+
         *        ^         (!HasMovementInput)    |
         *        |                                |
         *        +-------------+------------------+
         *                      |
         *                      | (WantsToCrouch)
         *                      v
         *                   +--------+
         *                   | Crouch |
         *                   +--------+
         *                      |
         *                      +--> Move (si hay input)
         *                      +--> Idle (si no hay input)
         *
         *
         *
         */

        public void EvaluateStateTransitions()
        {
            if (WantsToCrouch)
            {
                _stateMachine.ChangeState(_crouchState);
                return;
            }

            if (HasMovementInput)
            {
                _stateMachine.ChangeState(_moveState);
            }
            else
            {
                _stateMachine.ChangeState(_idleState);
            }
        }

        public void MoveCharacter(float speed)
        {
            if (_movementLocked) return;

            Vector3 direction = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            _characterController.Move(direction * (speed * Time.deltaTime));
        }

        public void ForceMove(Vector3 worldDisplacement)
        {
            _characterController.Move(worldDisplacement);
        }

        public void LockMovementForSeconds(float duration)
        {
            if (duration <= 0f) return;

            _movementLocked = true;
            _movementLockUntil = Mathf.Max(_movementLockUntil, Time.time + duration);
        }

        public void UnlockMovement()
        {
            _movementLocked = false;
            _movementLockUntil = 0f;
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0 || currentLives <= 0) return;

            currentLives = Mathf.Max(0, currentLives - amount);
        }

        public void HealToFull()
        {
            currentLives = maxLives;
        }

        private void RefreshMovementLock()
        {
            if (!_movementLocked) return;

            if (Time.time >= _movementLockUntil)
            {
                UnlockMovement();
            }
        }

        private void TryTriggerExternalCatAttack()
        {
            if (!enableCrouchKeyAttackTrigger || externalAttackCat == null)
            {
                return;
            }

            if (!_crouchHeld)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current[externalAttackKey].wasPressedThisFrame)
            {
                externalAttackCat.TriggerExternalAttack();
            }
        }

        private void SetCrouch(bool shouldCrouch)
        {
            _characterController.height = shouldCrouch ? crouchHeight : _standingHeight;
            _characterController.center = shouldCrouch
                ? new Vector3(_standingCenter.x, crouchHeight * 0.5f, _standingCenter.z)
                : _standingCenter;

            if (cameraPivot != null)
            {
                Vector3 localPos = cameraPivot.localPosition;
                localPos.y = shouldCrouch ? crouchCameraLocalY : _standingCameraLocalY;
                cameraPivot.localPosition = localPos;
            }
        }

        private interface IPlayerState
        {
            void Enter();
            void Tick();
            void Exit();
        }

        private class PlayerStateMachine
        {
            private IPlayerState _currentState;

            public PlayerStateMachine(IPlayerState initialState)
            {
                _currentState = initialState;
                _currentState.Enter();
            }

            public void ChangeState(IPlayerState newState)
            {
                if (_currentState == newState) return;

                _currentState.Exit();
                _currentState = newState;
                _currentState.Enter();
            }

            public void Tick()
            {
                _currentState.Tick();
            }
        }

        private class IdleState : IPlayerState
        {
            private readonly FirstPersonStateMachineController _controller;

            public IdleState(FirstPersonStateMachineController controller)
            {
                _controller = controller;
            }

            public void Enter() { }

            public void Tick()
            {
                _controller.EvaluateStateTransitions();
            }

            public void Exit() { }
        }

        private class MoveState : IPlayerState
        {
            private readonly FirstPersonStateMachineController _controller;

            public MoveState(FirstPersonStateMachineController controller)
            {
                _controller = controller;
            }

            public void Enter() { }

            public void Tick()
            {
                _controller.MoveCharacter(_controller.moveSpeed);
                _controller.EvaluateStateTransitions();
            }

            public void Exit() { }
        }

        private class CrouchState : IPlayerState
        {
            private readonly FirstPersonStateMachineController _controller;

            public CrouchState(FirstPersonStateMachineController controller)
            {
                _controller = controller;
            }

            public void Enter()
            {
                _controller.SetCrouch(true);
            }

            public void Tick()
            {
                _controller.MoveCharacter(_controller.crouchSpeed);
                _controller.EvaluateStateTransitions();
            }

            public void Exit() { }
        }

    }
}
