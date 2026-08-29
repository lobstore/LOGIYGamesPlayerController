using Alchemy.Hierarchy;
using Alchemy.Inspector;
using LOGIYGames.Shared.Extensions;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public enum CameraPerspectiveType
    {
        FirstPerson,
        ThirdPersonFreeLook,
        ThirdPersonLookForward,
        LockOn,
        Top_Down
    }
    public class CameraManager : PersistentSingleton<CameraManager>
    {
        [ReadOnly][SerializeField] List<CinemachineCamera> cinemachineCameraControllers = new();
        public CinemachineCamera CurrentCameraController { get; private set; }
        [SerializeField] CinemachineCamera FirstPersonCameraController;
        [SerializeField] CinemachineCamera ThirdPersonCameraController;
        [SerializeField] CinemachineCamera TopDownCameraController;
        [SerializeField] bool isActionFP;
        CinemachineCamera instance_FirstPersonCameraController;
        CinemachineCamera instance_ThirdPersonCameraController;
        CinemachineCamera instance_TopDownCameraController;

        [SerializeField] InputActionAsset inputActions;
        public PlayerCameraInputReader CameraInput { get; private set; }
        [SerializeField] private CameraPerspectiveType currentCameraPerspectiveType;
        public UnityEvent OnCameraPerspectiveChanged { get; private set; } = new();
        public CameraPerspectiveType CurrentCameraPerspectiveType
        {
            get { return currentCameraPerspectiveType; }
            set
            {
                currentCameraPerspectiveType = value;
                OnCameraPerspectiveChanged.Invoke();
                ResetCameraView();
            }
        }
        public void DisableAllCameras()
        {
            foreach (var item in cinemachineCameraControllers)
            {
                item.enabled = false;
            }
        }
        public void EnableAllCameras()
        {
            foreach (var item in cinemachineCameraControllers)
            {
                item.enabled = true;
            }
        }
        private void Initialize()
        {
            CameraInput = new(inputActions);
            PlayerManager.Instance.OnCharacterChanged.AddListener((_) => ResetCameraView());
            var holder = new GameObject("PlayerVirtualCams_Runtime");
            holder.GetOrAddComponent<HierarchyHeader>();
            instance_FirstPersonCameraController = Instantiate(FirstPersonCameraController, holder.transform);
            instance_ThirdPersonCameraController = Instantiate(ThirdPersonCameraController, holder.transform);
            instance_TopDownCameraController = Instantiate(TopDownCameraController, holder.transform);
            cinemachineCameraControllers.Add(instance_FirstPersonCameraController);
            cinemachineCameraControllers.Add(instance_ThirdPersonCameraController);
            cinemachineCameraControllers.Add(instance_TopDownCameraController);
        }
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
        private void Start()
        {
            CameraInput.Enable();
            ResetCameraView();
        }

        public void ResetCameraView()
        {
            SetTargetTo(PlayerManager.Instance.CurrentCharacter.CameraTarget);

            switch (CurrentCameraPerspectiveType)
            {
                case CameraPerspectiveType.FirstPerson:
                    Set1stView();
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    Set3rdFreeLookView();
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    Set3rdLookForwardView();
                    break;
                case CameraPerspectiveType.Top_Down:
                    SetTopDownView();
                    break;
                default:
                    break;
            }

        }
        public void SetTargetTo(CameraTarget cameraTarget)
        {
            foreach (var cam in cinemachineCameraControllers)
            {
                cam.Target = cameraTarget;
            }
        }
        public void SetLookAtTo(Transform lookAt)
        {
            foreach (var cam in cinemachineCameraControllers)
            {
                cam.LookAt = lookAt;
            }
        }
        public void SetFollowTo(Transform tracking)
        {
            foreach (var cam in cinemachineCameraControllers)
            {
                cam.Follow = tracking;
            }
        }
        void SetPriorVirtualCamera(CinemachineCamera cameraController)
        {
            foreach (var controller in cinemachineCameraControllers)
            {
                if (controller != cameraController)
                {
                    controller.Priority = 0;
                    continue;
                }
                else
                {
                    controller.Priority = 10;
                }

            }

        }
        public void Set3rdFreeLookView()
        {
            CurrentCameraController = instance_ThirdPersonCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void Set1stView()
        {
            CurrentCameraController = instance_FirstPersonCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void SetTopDownView()
        {
            CurrentCameraController = instance_TopDownCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void Set3rdLookForwardView()
        {
            CurrentCameraController = instance_ThirdPersonCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
        }

    }
}
