using System.Collections;
using UnityEngine;
using KittyTerror.Events;

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

        [Header("Visual")]
        [SerializeField] private GameObject catVisualPrefab;
        [SerializeField] private string resourcesModelPath = "Models/Cat";
        [SerializeField] private bool autoAlignVisualToCapsuleBottom = true;
        [SerializeField] private Vector3 visualLocalPosition = Vector3.zero;
        [SerializeField] private Vector3 visualLocalRotation = Vector3.zero;
        [SerializeField] private Vector3 visualLocalScale = Vector3.one;

        [Header("Detection")]
        [SerializeField] private float raycastHeight = 0.7f;
        [SerializeField] private float guardDistance = 6f;
        [SerializeField] private float attackDistance = 2.5f;
        [SerializeField] private LayerMask raycastMask = ~0;

        [Header("Guard Pose")]
        [SerializeField] private float guardPitch = 45f;
        [SerializeField] private float guardYawOffset = 0f;
        [SerializeField] private float guardRoll = 0f;
        [SerializeField] private float guardRotateSpeed = 8f;

        [Header("Flee")]
        [SerializeField] private float fleeDuration = 1.5f;
        [SerializeField] private float fleeSpeed = 10f;

        [Header("Attack")]
        [SerializeField] private float attachDistanceFromCamera = 0.6f;
        [SerializeField] private float attachVerticalOffset = -0.1f;
        [SerializeField] private bool keepAttackOnFloorPlane = true;
        [SerializeField] private bool attachAbovePlayerDuringAttack = true;
        [SerializeField] private bool attachToCameraDuringAttack = true;
        [SerializeField] private float attackAboveOffset = 1.1f;
        [SerializeField] private float attackExtraHeight = 0.9f;
        [SerializeField] private float minTotalAttachHeight = 1.8f;
        [SerializeField] private float attackForwardOffset = 0.05f;
        [SerializeField] private float attackFollowSmoothing = 20f;
        [SerializeField] private bool attackLookUp = true;
        [SerializeField] private float attackJumpDuration = 0.18f;
        [SerializeField] private float attackJumpArcHeight = 1.4f;
        [SerializeField] private float minAttackJumpArcHeight = 1.1f;
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
        private GameObject _visualInstance;

        private void Awake()
        {
            EnsureVisualModel();
            _spawnRotation = transform.rotation;
            _idleRotation = transform.rotation;
            _catColliders = GetComponentsInChildren<Collider>(true);
        }

        private void EnsureVisualModel()
        {
            if (HasExternalVisual())
            {
                return;
            }

            if (catVisualPrefab == null)
            {
                catVisualPrefab = Resources.Load<GameObject>(resourcesModelPath);
            }

            if (catVisualPrefab == null)
            {
                Debug.LogWarning($"[{nameof(CatStateMachineController)}] No se encontró prefab visual en Resources/{resourcesModelPath} para {name}.", this);
                return;
            }

            _visualInstance = Instantiate(catVisualPrefab, transform);
            _visualInstance.name = "CatModel";
            Transform visualTransform = _visualInstance.transform;
            visualTransform.localPosition = visualLocalPosition;
            visualTransform.localRotation = Quaternion.Euler(visualLocalRotation);
            visualTransform.localScale = visualLocalScale;

            if (autoAlignVisualToCapsuleBottom)
            {
                AlignVisualToCapsuleBottom();
            }

            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer != null)
            {
                rootRenderer.enabled = false;
            }
        }

        private bool HasExternalVisual()
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                MeshRenderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (renderer.gameObject != gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        private void AlignVisualToCapsuleBottom()
        {
            if (_visualInstance == null)
            {
                return;
            }

            CapsuleCollider capsule = GetComponent<CapsuleCollider>();
            if (capsule == null)
            {
                return;
            }

            if (capsule.direction != 1)
            {
                Debug.LogWarning($"[{nameof(CatStateMachineController)}] El CapsuleCollider de {name} no está en eje Y. Se omite auto-align vertical.", this);
                return;
            }

            if (!TryGetVisualBottomLocalY(out float visualBottomLocalY))
            {
                return;
            }

            float capsuleBottomLocalY = capsule.center.y - (capsule.height * 0.5f);
            float offsetY = capsuleBottomLocalY - visualBottomLocalY;

            Vector3 localPos = _visualInstance.transform.localPosition;
            localPos.y += offsetY;
            _visualInstance.transform.localPosition = localPos;
        }

        private bool TryGetVisualBottomLocalY(out float minLocalY)
        {
            minLocalY = float.MaxValue;
            Renderer[] renderers = _visualInstance.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                Bounds bounds = renderer.bounds;
                Vector3 ext = bounds.extents;
                Vector3 center = bounds.center;

                for (int x = -1; x <= 1; x += 2)
                {
                    for (int y = -1; y <= 1; y += 2)
                    {
                        for (int z = -1; z <= 1; z += 2)
                        {
                            Vector3 worldCorner = center + Vector3.Scale(ext, new Vector3(x, y, z));
                            Vector3 localCorner = transform.InverseTransformPoint(worldCorner);
                            if (localCorner.y < minLocalY)
                            {
                                minLocalY = localCorner.y;
                            }
                        }
                    }
                }
            }

            return minLocalY != float.MaxValue;
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
            // Guardia: X fijo y Y siguiendo al jugador en el plano del piso.
            Quaternion lookToPlayerOnFloor = Quaternion.LookRotation(flatDirectionToPlayer, Vector3.up);
            float targetYaw = lookToPlayerOnFloor.eulerAngles.y + guardYawOffset;
            Quaternion threateningPose = Quaternion.Euler(guardPitch, targetYaw, guardRoll);

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
                    float lockedY = cachedPosition.y;
                    Vector3 initialAttachPoint = GetAttackAttachPoint(lockedY);
                    yield return JumpToAttachPoint(initialAttachPoint);
                }

                if (_playerController != null)
                {
                    _playerController.LockMovementForSeconds(lockPlayerMovementDuration);
                    _playerController.ApplyDamage(damagePerAttack);
                    EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("cat_attack"));
                }

                yield return PushPlayerBackOverTime();

                float elapsed = 0f;
                while (elapsed < attachDuration)
                {
                    elapsed += Time.deltaTime;

                    if (playerCamera != null)
                    {
                        float lockedY = cachedPosition.y;
                        Vector3 attachPoint = GetAttackAttachPoint(lockedY);
                        float smoothing = Mathf.Max(0.01f, attackFollowSmoothing);
                        transform.position = Vector3.Lerp(transform.position, attachPoint, Time.deltaTime * smoothing);

                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            GetAttackLookRotation(),
                            Time.deltaTime * smoothing);
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

        private Vector3 GetAttackAttachPoint(float lockedY)
        {
            if (attachAbovePlayerDuringAttack)
            {
                Transform anchor = attachToCameraDuringAttack && playerCamera != null ? playerCamera : player;
                if (anchor == null)
                {
                    return GetCameraAttachPoint();
                }

                float totalAttachHeight = Mathf.Max(minTotalAttachHeight, attackAboveOffset + attackExtraHeight);

                return anchor.position +
                       anchor.up * totalAttachHeight +
                       anchor.forward * attackForwardOffset;
            }

            return keepAttackOnFloorPlane
                ? GetCameraAttachPointOnFloor(lockedY)
                : GetCameraAttachPoint();
        }

        private Vector3 GetAttackLookDirection()
        {
            if (attackLookUp)
            {
                return Vector3.up;
            }

            return keepAttackOnFloorPlane ? GetCameraForwardOnFloor() : playerCamera.forward;
        }

        private Quaternion GetAttackLookRotation()
        {
            Vector3 lookDirection = GetAttackLookDirection();
            if (lookDirection.sqrMagnitude <= 0.0001f)
            {
                return transform.rotation;
            }

            if (attackLookUp)
            {
                Vector3 upHint = playerCamera != null ? playerCamera.forward : Vector3.forward;
                if (upHint.sqrMagnitude <= 0.0001f)
                {
                    upHint = Vector3.forward;
                }

                upHint.Normalize();
                if (Mathf.Abs(Vector3.Dot(upHint, Vector3.up)) > 0.98f)
                {
                    upHint = Vector3.forward;
                }

                return Quaternion.LookRotation(Vector3.up, upHint);
            }

            return Quaternion.LookRotation(lookDirection, Vector3.up);
        }

        private IEnumerator JumpToAttachPoint(Vector3 destination)
        {
            float duration = Mathf.Max(0.01f, attackJumpDuration);
            float arcHeight = Mathf.Max(minAttackJumpArcHeight, attackJumpArcHeight);

            Vector3 start = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                Vector3 point = Vector3.Lerp(start, destination, t);
                float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
                point.y += arc;

                transform.position = point;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    GetAttackLookRotation(),
                    Time.deltaTime * Mathf.Max(0.01f, attackFollowSmoothing));

                yield return null;
            }

            transform.position = destination;
        }

        private Vector3 GetCameraAttachPointOnFloor(float lockedY)
        {
            Vector3 cameraForwardOnFloor = GetCameraForwardOnFloor();
            Vector3 attachPoint = playerCamera.position + cameraForwardOnFloor * attachDistanceFromCamera;
            attachPoint.y = lockedY + attachVerticalOffset;
            return attachPoint;
        }

        private Vector3 GetCameraForwardOnFloor()
        {
            Vector3 cameraForward = playerCamera.forward;
            cameraForward.y = 0f;

            if (cameraForward.sqrMagnitude <= 0.0001f)
            {
                cameraForward = player.forward;
                cameraForward.y = 0f;
            }

            return cameraForward.sqrMagnitude > 0.0001f ? cameraForward.normalized : transform.forward;
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

        public void Flee()
            {
                Debug.Log($"[Cat] Flee() llamado en {name}");
                EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("cat_flee"));
                    
                // Detenemos cualquier otra acción (como el ataque)
                StopAllCoroutines();
                _attackInProgress = false;
                    
                // Iniciamos la secuencia de huida/destrucción
                StartCoroutine(FleeSequence());
            }

        private IEnumerator FleeSequence()
        {
            Debug.Log("[Cat] Iniciando secuencia de huida");
            SetState(CatState.Idle);
            StopGuardAudio();

            Vector3 fleeDirection = -transform.forward + Vector3.right * Random.Range(-1f, 1f);
            fleeDirection.y = 0;
            fleeDirection.Normalize();

            Debug.Log($"[Cat] Huyendo en dirección: {fleeDirection}");

            float elapsed = 0;

            while (elapsed < fleeDuration)
            {
                elapsed += Time.deltaTime;
                transform.position += fleeDirection * (Time.deltaTime * fleeSpeed);
                yield return null;
            }

            Debug.Log("[Cat] Huida completada, destruyendo");
            Destroy(gameObject);
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
