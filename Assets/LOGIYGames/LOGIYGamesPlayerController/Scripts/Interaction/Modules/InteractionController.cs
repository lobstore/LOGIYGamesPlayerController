using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    public class InteractionController : MonoModuleBase
    {
        private Actor owner;

        public enum InteractorColliderType
        {
            Sphere,
            Box,
            Capsule
        }

        [Header("Settings")]
        [SerializeField]
        private InteractorColliderType colliderType;

        [SerializeField]
        private LayerMask interactMask;

        [SerializeField]
        private Transform interactPoint;

        [Header("Sphere")]
        [SerializeField]
        private float interactRadius = 2f;

        [Header("Box")]
        [Tooltip("Половина размеров бокса")]
        [SerializeField]
        private Vector3 boxHalfExtents = Vector3.one;

        [Header("Capsule")]
        [SerializeField]
        private float capsuleRadius = 0.5f;

        [SerializeField]
        private float capsuleHeight = 2f;

        [Header("Offset")]
        [Tooltip("Локальное смещение относительно Interact Point")]
        [SerializeField]
        private Vector3 offsetCenter;

        [Header("Events")]
        public UnityEvent<IReadOnlyList<InteractionAction>> OnActionsShown = new();

        public UnityEvent OnActionsHidden = new();

        public IInteractable CurrentInteractable { get; private set; }

        private Vector3 CastCenter =>
            interactPoint.TransformPoint(offsetCenter);

        private Quaternion CastRotation =>
            interactPoint.rotation;


        [SerializeField] InteractionActionView interactionActionView;

        public override void Initialize()
        {
            base.Initialize();

            owner = GetComponent<Actor>();
            OnActionsShown.AddListener((_) =>
            {
                PlayerManager.Instance.PlayerInput.Disable();
                CameraManager.Instance.CameraInput.Disable();
                CameraManager.Instance.SetLookAtTo(CurrentInteractable as Transform);
            });
            OnActionsHidden.AddListener(() =>
            {
                PlayerManager.Instance.PlayerInput.Enable();
                CameraManager.Instance.CameraInput.Enable();
                CameraManager.Instance.ResetCameraView();
            });
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            Scan();

            if (owner.Input.InteractPressed)
            {
                OnInteractPressed();
            }
            if (GameManager.Instance.GameInput.EscapePressed)
            {
                OnInteractReleased();
            }
            if (owner.Input.InteractHeld)
            {
                OnInteractHeld();
            }

        }

        private void OnInteractPressed()
        {
            /*
             * Если игрок просто нажал кнопку,
             * выполняем действие с максимальным Priority.
             */

            TryInteract();
        }

        private void OnInteractHeld()
        {
            /*
             * Открываем меню только один раз.
             */

            ShowAvailableActions();
        }

        private void OnInteractReleased()
        {
            HideActionSelection();
        }

        public void TryInteract()
        {
            if (!TryGetAvailableActions(out IReadOnlyList<InteractionAction> actions))
                return;

            /*
             * GetActions() уже сортирует по Priority.
             *
             * Поэтому actions[0] — самое приоритетное действие.
             */

            actions.First().Execute(CreateContext());
        }

        private void ShowAvailableActions()
        {
            if (!TryGetAvailableActions(out IReadOnlyList<InteractionAction> actions))
            {
                return;
            }

            interactionActionView.Show(actions, new InteractionContext { Interactor = this });
            OnActionsShown?.Invoke(actions);
        }

        private void HideActionSelection()
        {
            interactionActionView.Hide();
            OnActionsHidden?.Invoke();
        }

        private bool TryGetAvailableActions(out IReadOnlyList<InteractionAction> actions)
        {
            actions = null;

            if (CurrentInteractable == null)
                return false;

            InteractionContext context = CreateContext();

            actions = CurrentInteractable.GetActions(context);

            return actions != null && actions.Count > 0;
        }

        private InteractionContext CreateContext()
        {
            return new InteractionContext
            {
                Interactor = this
            };
        }

        private void Scan()
        {
            Collider[] hits = GetInteractColliders();

            IInteractable closest = null;
            float closestDistance = float.MaxValue;

            InteractionContext context = CreateContext();

            foreach (Collider hit in hits)
            {
                if (hit == null)
                    continue;

                if (!hit.TryGetComponent(out IInteractable interactable))
                {
                    continue;
                }

                float distance = Vector3.Distance(
                    CastCenter,
                    hit.ClosestPoint(CastCenter));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }

            if (CurrentInteractable == closest)
                return;

            CurrentInteractable?.OnFocusLost();

            CurrentInteractable = closest;

            CurrentInteractable?.OnFocusGained();
        }

        private Collider[] GetInteractColliders()
        {
            switch (colliderType)
            {
                case InteractorColliderType.Sphere:

                    return Physics.OverlapSphere(
                        CastCenter,
                        interactRadius,
                        interactMask);

                case InteractorColliderType.Box:

                    return Physics.OverlapBox(
                        CastCenter,
                        boxHalfExtents,
                        CastRotation,
                        interactMask);

                case InteractorColliderType.Capsule:

                    return GetCapsuleColliders();

                default:

                    return System.Array.Empty<Collider>();
            }
        }

        private Collider[] GetCapsuleColliders()
        {
            Vector3 center = CastCenter;

            Vector3 direction =
                CastRotation * Vector3.up;

            float halfHeight =
                Mathf.Max(
                    0f,
                    capsuleHeight * 0.5f - capsuleRadius);

            Vector3 point1 =
                center + direction * halfHeight;

            Vector3 point2 =
                center - direction * halfHeight;

            return Physics.OverlapCapsule(
                point1,
                point2,
                capsuleRadius,
                interactMask);
        }

        private void OnDrawGizmosSelected()
        {
            if (interactPoint == null)
                return;

            switch (colliderType)
            {
                case InteractorColliderType.Sphere:

                    Gizmos.color = Color.green;

                    Gizmos.DrawWireSphere(
                        CastCenter,
                        interactRadius);

                    break;

                case InteractorColliderType.Box:

                    Gizmos.color = Color.red;

                    Matrix4x4 oldMatrix = Gizmos.matrix;

                    Gizmos.matrix = Matrix4x4.TRS(
                        CastCenter,
                        CastRotation,
                        Vector3.one);

                    Gizmos.DrawWireCube(
                        Vector3.zero,
                        boxHalfExtents * 2f);

                    Gizmos.matrix = oldMatrix;

                    break;

                case InteractorColliderType.Capsule:

                    DrawCapsuleGizmo();

                    break;
            }
        }

        private void DrawCapsuleGizmo()
        {
            Gizmos.color = Color.blue;

            Vector3 center = CastCenter;

            Vector3 direction =
                CastRotation * Vector3.up;

            float halfHeight =
                Mathf.Max(
                    0f,
                    capsuleHeight * 0.5f - capsuleRadius);

            Vector3 point1 =
                center + direction * halfHeight;

            Vector3 point2 =
                center - direction * halfHeight;

            Gizmos.DrawWireSphere(
                point1,
                capsuleRadius);

            Gizmos.DrawWireSphere(
                point2,
                capsuleRadius);

            Vector3 right =
                CastRotation * Vector3.right;

            Vector3 forward =
                CastRotation * Vector3.forward;

            Gizmos.DrawLine(
                point1 + right * capsuleRadius,
                point2 + right * capsuleRadius);

            Gizmos.DrawLine(
                point1 - right * capsuleRadius,
                point2 - right * capsuleRadius);

            Gizmos.DrawLine(
                point1 + forward * capsuleRadius,
                point2 + forward * capsuleRadius);

            Gizmos.DrawLine(
                point1 - forward * capsuleRadius,
                point2 - forward * capsuleRadius);
        }
    }
}