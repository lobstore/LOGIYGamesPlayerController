using Alchemy.Hierarchy;
using Alchemy.Inspector;
using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Extensions;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
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
        [ReadOnly] [SerializeField] List<CinemachineCamera> cinemachineCameraControllers = new();
        public CinemachineCamera CurrentCameraController { get; private set; }
        [SerializeField] CinemachineCamera FirstPersonCameraController;
        [SerializeField] CinemachineCamera ThirdPersonCameraController;
        [SerializeField] CinemachineCamera TopDownCameraController;
        [SerializeField] CinemachineCamera LockOnCameraController;

        CinemachineCamera instance_FirstPersonCameraController;
        CinemachineCamera instance_ThirdPersonCameraController;
        CinemachineCamera instance_TopDownCameraController;
        CinemachineCamera instance_LockOnCameraController;

        [SerializeField] InputActionAsset inputActions;
        public PlayerCameraInputReader CameraInput { get; private set; }



        [SerializeField] private CameraPerspectiveType defaultCameraPerspectiveType;
        [SerializeField] private CameraPerspectiveType currentCameraPerspectiveType;

        public CameraPerspectiveType CurrentCameraPerspectiveType
        {
            get { return currentCameraPerspectiveType; }
            set
            {
                currentCameraPerspectiveType = value;
                UpdateCameraView();
            }
        }
        private void Initialize()
        {
            CameraInput = new(inputActions);
            instance_FirstPersonCameraController = Instantiate(FirstPersonCameraController, null);
            instance_ThirdPersonCameraController = Instantiate(ThirdPersonCameraController, null);
            instance_TopDownCameraController = Instantiate(TopDownCameraController, null);
            instance_LockOnCameraController = Instantiate(LockOnCameraController, null);
            cinemachineCameraControllers.Add(instance_FirstPersonCameraController);
            cinemachineCameraControllers.Add(instance_ThirdPersonCameraController);
            cinemachineCameraControllers.Add(instance_TopDownCameraController);
            cinemachineCameraControllers.Add(instance_LockOnCameraController);
            var holder = new GameObject("VirtualCameras_Runtime");
            holder.GetOrAddComponent<HierarchyHeader>();
            foreach (var item in cinemachineCameraControllers)
            {
                item.gameObject.transform.SetParent(holder.transform);
            }
            currentCameraPerspectiveType = defaultCameraPerspectiveType;
        }
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
        private void Start()
        {

            CameraInput.Enable();
            //PlayerManager.Instance.OnTargetLocked.AddListener((evt) =>
            //{
            //    if (evt)
            //    {
            //        if (PlayerManager.Instance.CurrentCharacter.Targeting.CurrentTarget != null)
            //        {
            //            CurrentCameraPerspectiveType = CameraPerspectiveType.LockOn;
            //        }
            //        else
            //        {
            //            CurrentCameraPerspectiveType = defaultCameraPerspectiveType;
            //        }
            //    }
            //    else
            //    {
            //        CurrentCameraPerspectiveType = defaultCameraPerspectiveType;
            //        CurrentCameraController.CameraFollowTarget = PlayerManager.Instance.CurrentCharacter.CameraTarget.CameraFollow;
            //        CurrentCameraController.CameraLookAtTarget = PlayerManager.Instance.CurrentCharacter.CameraTarget.CameraLookAt;
            //    }

            //});
        }

        private void UpdateCameraView()
        {
            switch (CurrentCameraPerspectiveType)
            {
                case CameraPerspectiveType.FirstPerson:
                    SetTargetTo(PlayerManager.Instance.CurrentCharacter.FPVCameraTarget);
                    Set1stView();
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    SetTargetTo(PlayerManager.Instance.CurrentCharacter.TPVCameraTarget);
                    Set3rdFreeLookView();
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    Set3rdLookForwardView();
                    break;
                case CameraPerspectiveType.Top_Down:
                    SetTopDownView();
                    break;
                case CameraPerspectiveType.LockOn:
                    SetLockOnView();
                    break;
                default:
                    break;
            }
        }

        int index = 0;
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    index++;
            //    index = index % cinemachineCameraControllers.Count;
            //    if (index == 0)
            //    {
            //        CameraPerspectiveType = CameraPerspectiveType.ThirdPersonFreeLook;
            //    }
            //    else if (index == 1)
            //    {
            //        CameraPerspectiveType = CameraPerspectiveType.FirstPerson;
            //    }
            //    else if (index == 2)
            //    {
            //        CameraPerspectiveType = CameraPerspectiveType.Top_Down;
            //    }
            //    else if (index == 3)
            //    {
            //        CameraPerspectiveType = CameraPerspectiveType.ThirdPersonLookForward;
            //    }
            //}
            UpdateCameraView();

        }
        public void SetTargetTo(CameraTarget cameraTarget)
        {
            foreach (var cam in cinemachineCameraControllers)
            {
                cam.Target = cameraTarget;
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
        public void SetLockOnView()
        {
            CurrentCameraController = instance_LockOnCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
            CurrentCameraController.Target.TrackingTarget = PlayerManager.Instance.CurrentCharacter.TPVCameraTarget.TrackingTarget;
            //CurrentCameraController.CameraLookAtTarget = PlayerManager.Instance.TargetGroup.transform;
        }

    }
}
