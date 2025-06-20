using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
namespace LOGIYGames
{
    [RequireComponent(typeof(CameraTarget))]
    public class CinemachineCameraSwitcher : NetworkBehaviour
    {
        [SerializeField] InputReader Input;
        private Camera MainCamera;
        private List<CinemachineCameraController> cameraControllers = new List<CinemachineCameraController>();
        public CinemachineCameraController CurentCameraController { get; private set; }
        CinemachineCameraController FPSCameraController;
        CinemachineCameraController TPSCameraController;
        CinemachineCameraController TDSCameraController;
        [field: SerializeField] public Animator PlayerAnimator { get; private set; }
        public bool IsFP { get; private set; } = false;
        public bool IsTargetLocked { get; private set; } = false;
        int index = 0;
        private void Awake()
        {
            MainCamera = Camera.main;
        }

        private void InitControllersViews()
        {
            FPSCameraController = VirtualCamProvider.Instance.FirstPlayerVirtualCameraPrefab.GetComponent<CinemachineCameraController>();
            TPSCameraController = VirtualCamProvider.Instance.ThirdPlayerVirtualCameraPrefab.GetComponent<CinemachineCameraController>();
            TDSCameraController = VirtualCamProvider.Instance.TopDownPlayerVirtualCameraPrefab.GetComponent<CinemachineCameraController>();
            cameraControllers.Add(FPSCameraController);
            cameraControllers.Add(TPSCameraController);
            cameraControllers.Add(TDSCameraController);
            SetCameraTarget();

            SetFPView();

        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            Input.InteractEvent.AddListener(SwitchPOV);
            InitControllersViews();
        }
        private void OnDisable()
        {
            Input.InteractEvent.RemoveListener(SwitchPOV);
        }
        override public void OnDestroy()
        {
            Input.InteractEvent.RemoveListener(SwitchPOV);
        }
        public override void OnNetworkDespawn()
        {

            Input.InteractEvent.RemoveListener(SwitchPOV);
        }
        private void SwitchPOV(CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            } 
            if (!IsOwner)
            {
                return;
            }
            index = (index + 1) % cameraControllers.Count;
            switch (index)
            {
                case 0:
                    SetFPView();

                    break;
                case 1:
                    SetTPView();

                    break;
                case 2:
                    SetTDView();

                    break;
                default:
                    break;
            }

        }

        private void SetTPView()
        {
            CurentCameraController = TPSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
            PlayerAnimator.SetBool("IsFocusing", false);
            IsFP = false;
        }
        void SetPriorVirtualCamera(CinemachineCameraController cameraController)
        {
            foreach (var controller in cameraControllers)
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
        private void SetFPView()
        {
            CurentCameraController = FPSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
            PlayerAnimator.SetBool("IsFocusing", true);
            IsFP = true;
        }
        private void SetTDView()
        {
            CurentCameraController = TDSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
            PlayerAnimator.SetBool("IsFocusing", false);
            IsFP = false;
        }

        private void SetCameraTarget()
        {
            var target = GetComponent<CameraTarget>();
            foreach (var cam in cameraControllers)
            {
                cam.CameraFollowTarget = target.Follow;
                cam.CameraLookAtTarget = target.LookAt;
            }

        }
    }
}