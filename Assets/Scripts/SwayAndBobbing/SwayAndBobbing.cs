//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;
//namespace LOGIYGames
//{
//    public class SwayNBobScript : NetworkBehaviour
//    {


//        [SerializeField] CharacterModule Player;
//        PlayerInputsManager MovementInput;
//        CameraInputManager CameraInput;

//        [Header("Sway")]
//        public float step = 0.01f;
//        public float maxStepDistance = 0.06f;
//        Vector3 swayPos;

//        [Header("Sway Rotation")]
//        [SerializeField] float rotationStep = 4f;
//        [SerializeField] float maxRotationStep = 5f;
//        Vector3 swayEulerRot;

//        [SerializeField] float smooth = 10f;
//        float smoothRot = 12f;

//        [Header("Bobbing")]
//        [SerializeField] float speedCurve;
//        float curveSin { get => Mathf.Sin(speedCurve); }
//        float curveCos { get => Mathf.Cos(speedCurve); }

//        [SerializeField] Vector3 travelLimit = Vector3.one * 0.025f;
//        [SerializeField] Vector3 bobLimit = Vector3.one * 0.01f;
//        Vector3 bobPosition;

//        [SerializeField] float bobExaggeration;

//        [Header("Bob Rotation")]
//        [SerializeField] Vector3 multiplier;
//        Vector3 bobEulerRotation;
//        private void OnEnable()
//        {
//            MovementInput = PlayerInputsManager.Instance;
//            CameraInput = CameraInputManager.Instance;
//        }

//        void Update()
//        {
//            if (!IsOwner) return;
//            GetInput();

//            Sway();
//            SwayRotation();
//            BobOffset();
//            BobRotation();

//            CompositePositionRotation();
//        }


//        Vector2 walkInput;
//        Vector2 lookInput;

//        void GetInput()
//        {
//            walkInput.x = MovementInput.MovementInput.x;
//            walkInput.y = MovementInput.MovementInput.y;

//            lookInput.x = CameraInput.LookInput.x;
//            lookInput.y = CameraInput.LookInput.y;
//        }


//        void Sway()
//        {
//            Vector3 invertLook = lookInput * -step;
//            invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
//            invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

//            swayPos = invertLook;
//        }

//        void SwayRotation()
//        {
//            Vector2 invertLook = lookInput * -rotationStep;
//            invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
//            invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
//            swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
//        }

//        void CompositePositionRotation()
//        {
//            transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
//            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
//        }

//        void BobOffset()
//        {
//            speedCurve += Time.deltaTime * (Player.IsGrounded ? (MovementInput.MovementInput.x + MovementInput.MovementInput.y) * bobExaggeration : 1f) + 0.01f;

//            bobPosition.x = (curveCos * bobLimit.x * (Player.IsGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
//            bobPosition.y = (curveSin * bobLimit.y) - (MovementInput.MovementInput.y * travelLimit.y);
//            bobPosition.z = -(walkInput.y * travelLimit.z);
//        }

//        void BobRotation()
//        {
//            bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
//            bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
//            bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
//        }

//    }
//}