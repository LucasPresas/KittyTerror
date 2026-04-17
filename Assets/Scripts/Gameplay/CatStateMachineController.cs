using System.Collections;
using UnityEngine;

namespace KittyTerror.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class CatStateMachineController : MonoBehaviour
    {
        private enum CatState
        {
            Idle,
            Guard,
            Attack
        }

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform playerCamera;

        [Header("Audio")]
        [SerializeField] private AudioSource guardAudioSource;
        [SerializeField] private AudioSource attackAudioSource;

        [Header("Detection")]
        [SerializeField] private float raycastHeight = 0.7f;
        [SerializeField] private float guardDistance = 6f;
        [SerializeField] private float attackDistance = 2.5f;
        [SerializeField] private LayerMask raycastMask = ~0;

        [Header("Guard Pose")]
        [SerializeField] private float guardPitch = 45f;
        [SerializeField] private float guardRotateSpeed = 8f;

        [Header("Attack")]
        [SerializeField] private float attachDistanceFromCamera = 0.6f;
        [SerializeField] private float attachVerticalOffset = -0.1f;
        [SerializeField] private float attachDuration = 2f;
        [SerializeField] private float returnDuration = 0.75f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float playerPushBackDistance = 1.8f;
        [SerializeField] [Range(0f, 1f)] private float pushBackMultiplier = 0.5f;
        [SerializeField] private float pushDuration = 0.12f;
        [SerializeField] private float lockPlayerMovementDuration = 2f;
        [SerializeField] private int damagePerAttack = 1;
        [SerializeField] private bool ignorePlayerCollisionsDuringAttack = true;

        [SerializeField] private CatState _currentState = CatState.Idle;
        private Quaternion _idleRotation;
        private Quaternion _spawnRotation;

        private bool _attackInProgress;
        private float _attackCooldownUntil;
        private FirstPersonStateMachineController _playerController;
        private Collider[] _catColliders;
        private Collider[] _playerColliders;

        private void Awake()
        {
            _spawnRotation = transform.rotation;
            _idleRotation = transform.rotation;
            _catColliders = GetComponentsInChildren<Collider>(true);
        }

        private void Start()
        {
            TryResolveRuntimeReferences(out _);
        }

        private void Update()
        {
            if (_attackInProgress)
            {
                return;
            }

            if (!TryResolveRuntimeReferences(out _))
            {
                return;
            }

            bool seesPlayer = TryGetVisibleDistanceToPlayer(out float visibleDistance, out Vector3 flatDirectionToPlayer);

            if (!seesPlayer)
            {
                SetState(CatState.Idle);
                UpdateIdlePose();
                return;
            }

            if (visibleDistance <= attackDistance && Time.time >= _attackCooldownUntil)
            {
                StartCoroutine(AttackSequence());
                return;
            }

            if (visibleDistance <= guardDistance)
            {
                SetState(CatState.Guard);
                UpdateGuardPose(flatDirectionToPlayer);
            }
            else
            {
                SetState(CatState.Idle);
                UpdateIdlePose();
            }
        }

        private void OnDisable()
        {
            StopGuardAudio();
            SetCollisionWithPlayerIgnored(false);
        }

        public void TriggerExternalAttack()
        {
            if (_attackInProgress)
            {
                return;
            }

            if (!TryResolveRuntimeReferences(out string reason))
            {
                return;
            }

            StartCoroutine(AttackSequence());
        }

        private bool TryGetVisibleDistanceToPlayer(out float distance, out Vector3 flatDirection)
        {
            distance = float.MaxValue;
            flatDirection = Vector3.forward;

            Vector3 origin = transform.position + Vector3.up * raycastHeight;
            Vector3 target = player.position + Vector3.up * raycastHeight;
            Vector3 direction = target - origin;

            float maxDistance = Mathf.Max(guardDistance, attackDistance);
            if (direction.sqrMagnitude > maxDistance * maxDistance)
            {
                return false;
            }

            if (direction.sqrMagnitude <= 0.0001f)
            {
                return true;
            }

            Vector3 normalizedDirection = direction.normalized;

            if (!Physics.Raycast(origin, normalizedDirection, out RaycastHit hit, maxDistance, raycastMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            if (!BelongsToPlayer(hit.transform))
            {
                return false;
            }

            distance = hit.distance;
            flatDirection = player.position - transform.position;
            flatDirection.y = 0f;
            if (flatDirection.sqrMagnitude <= 0.0001f)
            {
                flatDirection = transform.forward;
            }
            else
            {
                flatDirection.Normalize();
            }

            return true;
        }

        private bool BelongsToPlayer(Transform hitTransform)
        {
            return hitTransform == player || hitTransform.IsChildOf(player);
        }

        private void UpdateIdlePose()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _idleRotation, Time.deltaTime * guardRotateSpeed);
            StopGuardAudio();
        }

        private void UpdateGuardPose(Vector3 flatDirectionToPlayer)
        {
            Quaternion lookToPlayer = Quaternion.LookRotation(flatDirectionToPlayer, Vector3.up);
            Quaternion threateningPose = lookToPlayer * Quaternion.Euler(-guardPitch, 0f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, threateningPose, Time.deltaTime * guardRotateSpeed);
            PlayGuardAudio();
        }

        private IEnumerator AttackSequence()
        {
            _attackInProgress = true;
            _attackCooldownUntil = Time.time + attackCooldown;
            SetState(CatState.Attack);

            Vector3 cachedPosition = transform.position;
            Quaternion cachedRotation = transform.rotation;
            StopGuardAudio();
            PlayAttackAudio();

            SetCollisionWithPlayerIgnored(true);

            try
            {
                if (playerCamera != null)
                {
                    transform.position = GetCameraAttachPoint();
                }

                if (_playerController != null)
                {
                    _playerController.LockMovementForSeconds(lockPlayerMovementDuration);
                    _playerController.ApplyDamage(damagePerAttack);
                }

                yield return PushPlayerBackOverTime();

                float elapsed = 0f;
                while (elapsed < attachDuration)
                {
                    elapsed += Time.deltaTime;

                    if (playerCamera != null)
                    {
                        Vector3 attachPoint = GetCameraAttachPoint();
                        transform.position = Vector3.Lerp(transform.position, attachPoint, Time.deltaTime * 20f);
                        transform.rotation = Quaternion.LookRotation(playerCamera.forward, Vector3.up);
                    }

                    yield return null;
                }

                elapsed = 0f;
                Vector3 startReturn = transform.position;
                Quaternion startReturnRotation = transform.rotation;

                while (elapsed < returnDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / returnDuration);

                    transform.position = Vector3.Lerp(startReturn, cachedPosition, t);
                    transform.rotation = Quaternion.Slerp(startReturnRotation, cachedRotation, t);
                    yield return null;
                }

                transform.position = cachedPosition;
                transform.rotation = cachedRotation;
            }
            finally
            {
                _idleRotation = _spawnRotation;
                SetCollisionWithPlayerIgnored(false);
                _attackInProgress = false;
                SetState(CatState.Idle);
            }
        }

        private bool TryResolveRuntimeReferences(out string reason)
        {
            if (player == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    player = playerObject.transform;
                }
            }

            if (player == null)
            {
                reason = "No se encontró objeto con tag 'Player'.";
                return false;
            }

            if (playerCamera == null)
            {
                // Priorizar cámara del jugador para evitar inconsistencias si hay varias MainCamera activas.
                playerCamera = player.GetComponentInChildren<Camera>()?.transform;
            }

            if (playerCamera == null)
            {
                playerCamera = Camera.main != null ? Camera.main.transform : null;
            }

            if (_playerController == null)
            {
                _playerController = player.GetComponent<FirstPersonStateMachineController>();
            }

            if (_catColliders == null || _catColliders.Length == 0)
            {
                _catColliders = GetComponentsInChildren<Collider>(true);
            }

            if (_playerColliders == null || _playerColliders.Length == 0)
            {
                _playerColliders = player.GetComponentsInChildren<Collider>(true);
            }

            reason = string.Empty;
            return true;
        }

        private Vector3 GetCameraAttachPoint()
        {
            return playerCamera.position +
                   playerCamera.forward * attachDistanceFromCamera +
                   playerCamera.up * attachVerticalOffset;
        }

        private IEnumerator PushPlayerBackOverTime()
        {
            if (player == null)
            {
                yield break;
            }

            Vector3 referenceForward = playerCamera != null ? playerCamera.forward : player.forward;
            Vector3 pushDirection = -referenceForward;
            pushDirection.y = 0f;

            if (pushDirection.sqrMagnitude <= 0.0001f)
            {
                yield break;
            }

            pushDirection.Normalize();

            float finalPush = playerPushBackDistance * pushBackMultiplier;
            if (finalPush <= 0.0001f)
            {
                yield break;
            }

            float duration = Mathf.Max(0.02f, pushDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float dt = Time.deltaTime;
                elapsed += dt;
                float step = (finalPush / duration) * dt;

                if (_playerController != null)
                {
                    _playerController.ForceMove(pushDirection * step);
                }
                else
                {
                    CharacterController characterController = player.GetComponent<CharacterController>();
                    if (characterController != null)
                    {
                        characterController.Move(pushDirection * step);
                    }
                    else
                    {
                        player.position += pushDirection * step;
                    }
                }

                yield return null;
            }
        }

        private void PlayGuardAudio()
        {
            if (guardAudioSource == null || guardAudioSource.isPlaying) return;

            guardAudioSource.loop = true;
            guardAudioSource.Play();
        }

        private void StopGuardAudio()
        {
            if (guardAudioSource != null && guardAudioSource.isPlaying)
            {
                guardAudioSource.Stop();
            }
        }

        private void PlayAttackAudio()
        {
            if (attackAudioSource == null) return;

            attackAudioSource.loop = false;
            attackAudioSource.Play();
        }

        private void SetCollisionWithPlayerIgnored(bool shouldIgnore)
        {
            if (!ignorePlayerCollisionsDuringAttack || _catColliders == null || _playerColliders == null)
            {
                return;
            }

            for (int i = 0; i < _catColliders.Length; i++)
            {
                Collider catCollider = _catColliders[i];
                if (catCollider == null) continue;

                for (int j = 0; j < _playerColliders.Length; j++)
                {
                    Collider playerCollider = _playerColliders[j];
                    if (playerCollider == null) continue;

                    Physics.IgnoreCollision(catCollider, playerCollider, shouldIgnore);
                }
            }
        }

        private void SetState(CatState state)
        {
            _currentState = state;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, guardDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
        }
    }
}
