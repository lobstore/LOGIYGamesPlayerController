using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LOGIYGames
{
    public interface IControlState
    {
       public CinemachineCameraController CinemachineCameraController { get; }
        Vector3 GetMovementDirection(
            CharacterModule player, Transform camera, 
            Vector2 input, Transform locktarget=null
            );
        void Rotate(CharacterModule player, Transform camera, float turnSmoothTime, Vector3 moveDirection);
        void EnterState();
        void ExitState();
    }
    public class ThirdPersonControlState : IControlState
    {
        public CinemachineCameraController CinemachineCameraController { get; private set; }
        public ThirdPersonControlState(CinemachineCameraController cinemachineCameraController)
        {
            CinemachineCameraController = cinemachineCameraController;
        }
        public void EnterState()
        {
            CinemachineCameraController.Priority = 10;
        }

        public void ExitState()
        {
            CinemachineCameraController.Priority = 0;
        }

        public Vector3 GetMovementDirection(CharacterModule player, Transform camera, Vector2 input, Transform locktarget = null)
        {
            Vector3 movement = new Vector3(input.x, 0, input.y);

            Vector3 cam = camera.forward;
            return Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement;
        }

        public void Rotate(CharacterModule player, Transform camera, float turnSmoothTime, Vector3 moveDirection)
        {
            if (moveDirection.magnitude > 0)
            {
                // Рассчитываем угол поворота по направлению движения
                var targetAngle = Mathf.Atan2(player.HorizontalVelocity.x, player.HorizontalVelocity.z) * Mathf.Rad2Deg;

                // Плавно поворачиваем объект в сторону этого угла
                player.Rotate(Quaternion.Euler(0f, targetAngle, 0f), turnSmoothTime);
            }
        }
    }
    public class FirstPersonCotrolState : IControlState
    {
        public CinemachineCameraController CinemachineCameraController { get; private set; }
        public FirstPersonCotrolState(CinemachineCameraController cinemachineCameraController)
        {
            CinemachineCameraController = cinemachineCameraController;
        }
        public void EnterState()
        {
            CinemachineCameraController.Priority = 10;
        }

        public void ExitState()
        {
            CinemachineCameraController.Priority = 0;
        }

        public Vector3 GetMovementDirection(CharacterModule player, Transform camera, Vector2 input, Transform locktarget = null)
        {
            return player.transform.right * input.x + player.transform.forward * input.y;
        }

        public void Rotate(CharacterModule player, Transform camera, float turnSmoothTime, Vector3 moveDirection = default)
        {
            var targetAngle = camera.eulerAngles.y;
            player.Rotate(Quaternion.Euler(0f, targetAngle, 0f), turnSmoothTime);
        }
    }

    public class CameraManager : MonoBehaviour
    {
        public bool IsLockedOn { get; private set; }
        public IControlState CurrentControlState { get; private set; }
        IControlState FPSControlState;
        IControlState TPSControlState;
        IControlState TDSControlState;
        public static CameraManager Instance { get; private set; }
        List<IControlState> controlStates = new();
        public CinemachineCameraController CurentCameraController { get; private set; }
        [SerializeField] CinemachineCameraController FPSCameraController;
        [SerializeField] CinemachineCameraController TPSCameraController;
        [SerializeField] CinemachineCameraController TDSCameraController;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            if (Instance==null)
            {
                Instance = this;
            }
            
            InitControllersViews();
        }
        public void SetTargetTo(Transform Follow, Transform LookAt)
        {
            foreach (var cam in controlStates)
            {
                cam.CinemachineCameraController.CameraFollowTarget = Follow;
                cam.CinemachineCameraController.CameraLookAtTarget = LookAt;
            }
        }

        private void InitControllersViews()
        {

            TPSControlState = new ThirdPersonControlState(TPSCameraController);
            FPSControlState = new FirstPersonCotrolState(FPSCameraController);
            TDSControlState = new ThirdPersonControlState(TDSCameraController);
            controlStates.Add(FPSControlState);
            controlStates.Add(TPSControlState);
            controlStates.Add(TDSControlState);
        }

        private void ChangeState(IControlState nextstate)
        {
            if (CurrentControlState!=null)
            {
                CurrentControlState.ExitState();

            }
            CurrentControlState = nextstate;
            CurrentControlState.EnterState();
        }
        public void SetTPView()
        {
            //CurentCameraController = TPSCameraController;
            //SetPriorVirtualCamera(CurentCameraController);
            IsLockedOn = false;
            ChangeState(TPSControlState);
        }

        public void SetFPView()
        {
            //CurentCameraController = FPSCameraController;
            //SetPriorVirtualCamera(CurentCameraController);
            IsLockedOn = true;
            ChangeState(FPSControlState);
        }
        public void SetTDView()
        {
            //CurentCameraController = TDSCameraController;
            //SetPriorVirtualCamera(CurentCameraController);
            ChangeState(TDSControlState);
        }
        int index = 0;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeState(controlStates[index++%controlStates.Count]);
            }
        }
    }
}
