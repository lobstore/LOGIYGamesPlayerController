using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace LOGIYGames
{
    [RequireComponent(typeof(CameraTarget))]
    public class PlayerCameraManager : NetworkBehaviour
    {
        [SerializeField] LayerMask CameraRenderFps;
        [SerializeField] LayerMask CameraRenderTps;
        private Camera MainCamera;
        private List<CameraController> cameraControllers = new List<CameraController>();
        public CameraController CurentCameraController { get; private set; }
        CameraController FPSCameraController;
        CameraController TPSCameraController;
        CameraController TDSCameraController;
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
            FPSCameraController = VirtualCamProvider.Instance.FirstPlayerVirtualCameraPrefab.GetComponent<CameraController>();
            TPSCameraController = VirtualCamProvider.Instance.ThirdPlayerVirtualCameraPrefab.GetComponent<CameraController>();
            TDSCameraController = VirtualCamProvider.Instance.TopDownPlayerVirtualCameraPrefab.GetComponent<CameraController>();
            cameraControllers.Add(FPSCameraController);
            cameraControllers.Add(TPSCameraController);
            cameraControllers.Add(TDSCameraController);
            SetCameraTarget();

            SetFPView();

        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            PlayerInputsManager.Instance.Interacted.AddListener(SwitchPOV);
            InitControllersViews();
        }
        private void OnDisable()
        {
            PlayerInputsManager.Instance.Interacted.RemoveListener(SwitchPOV);
        }
        override public void OnDestroy()
        {
            PlayerInputsManager.Instance.Interacted.RemoveListener(SwitchPOV);
        }
        public override void OnNetworkDespawn()
        {

            PlayerInputsManager.Instance.Interacted.RemoveListener(SwitchPOV);
        }
        private void SwitchPOV()
        {
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
            MainCamera.cullingMask = CameraRenderTps;
            CurentCameraController = TPSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
            PlayerAnimator.SetBool("IsFocusing", false);
            IsFP = false;
        }
        void SetPriorVirtualCamera(CameraController cameraController)
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
            MainCamera.cullingMask = CameraRenderFps;
            CurentCameraController = FPSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
            PlayerAnimator.SetBool("IsFocusing", true);
            IsFP = true;
        }
        private void SetTDView()
        {
            MainCamera.cullingMask = CameraRenderTps;
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