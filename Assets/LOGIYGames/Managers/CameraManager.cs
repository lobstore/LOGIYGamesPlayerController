using System.Collections.Generic;
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
        [SerializeField] List<CinemachineCameraController> cinemachineCameraControllers = new();
        public CinemachineCameraController CurrentCameraController { get; private set; }
        [SerializeField] CinemachineCameraController FirstPersonCameraController;
        [SerializeField] CinemachineCameraController ThirdPersonCameraController;
        [SerializeField] CinemachineCameraController TopDownCameraController;
        [SerializeField] CinemachineCameraController LockOnCameraController;

        CinemachineCameraController instance_FirstPersonCameraController;
        CinemachineCameraController instance_ThirdPersonCameraController;
        CinemachineCameraController instance_TopDownCameraController;
        CinemachineCameraController instance_LockOnCameraController;

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
            currentCameraPerspectiveType = defaultCameraPerspectiveType;
        }
        private void Start()
        {
            Initialize();
            CameraInput.Enable();
            UpdateCameraView();
            PlayerManager.Instance.OnTargetLocked.AddListener((evt) =>
            {
                if (evt)
                {
                    if (PlayerManager.Instance.CurrentCharacter.Targeting.CurrentTarget != null)
                    {
                        CurrentCameraPerspectiveType = CameraPerspectiveType.LockOn;
                    }
                    else
                    {
                        CurrentCameraPerspectiveType = defaultCameraPerspectiveType;
                    }
                }
                else
                {
                    CurrentCameraPerspectiveType = defaultCameraPerspectiveType;
                    CurrentCameraController.CameraFollowTarget = PlayerManager.Instance.CurrentCharacter.CameraFollow;
                    CurrentCameraController.CameraLookAtTarget = PlayerManager.Instance.CurrentCharacter.CameraLookAt;
                }

            });
        }

        private void UpdateCameraView()
        {
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
        public void SetTargetTo(Transform Follow, Transform LookAt)
        {
            foreach (var cam in cinemachineCameraControllers)
            {
                cam.CameraFollowTarget = Follow;
                cam.CameraLookAtTarget = LookAt;
            }
        }

        void SetPriorVirtualCamera(CinemachineCameraController cameraController)
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
            CurrentCameraController.CameraFollowTarget = PlayerManager.Instance.CurrentCharacter.CameraFollow;
            CurrentCameraController.CameraLookAtTarget = PlayerManager.Instance.TargetGroup.transform;
        }

    }
}
