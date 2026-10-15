using Alchemy.Inspector;
using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class JumpController : MonoBehaviour
    {
        [field: SerializeField][field: ReadOnly] public int JumpCount { get; private set; }
        [field: SerializeField] public int MaxJumpCount { get; private set; }
        MovementWrapperBase Motor;
        Actor Character;
        [SerializeField] float Cost;
        private void Awake()
        {
            Motor = GetComponent<MovementWrapperBase>();
            Character = GetComponent<Actor>();

        }
        public void ResetJumps()
        {
            JumpCount = 0;
        }
        private void Update()
        {
            ResetJumps();
        }
        public bool CanExecute(JumpStateData jumpStateData)
        {
            return JumpCount < MaxJumpCount && Character.StaminaController.TryUse(Cost);
        }
        public void Jump(JumpType jumpType, float verticalForce, float planarForce)
        {

            Motor.ResetGravity();
            switch (jumpType)
            {
                case JumpType.GroundJump:
                    Motor.AddImpulse(Character.RuntimeMovement.TargetDirection * planarForce + Character.transform.up * verticalForce);
                    break;
                case JumpType.HangJump:
                    Motor.AddImpulse(Character.Sensors.LegsFrontHit.normal * planarForce + verticalForce * Character.transform.up);
                    break;
                case JumpType.WallRunJump:
                    break;
                case JumpType.Roll:
                    if (Character.Input.MovementInput.magnitude > 0)
                    {
                        Motor.AddImpulse(Character.RuntimeMovement.TargetDirection * planarForce + Character.transform.up * verticalForce);
                    }
                    else
                    {
                        Motor.AddImpulse(Character.transform.forward * planarForce + Character.transform.up * verticalForce);
                    }
                    break;
                case JumpType.Dash:
                    Motor.AddImpulse(Character.RuntimeMovement.TargetDirection * planarForce + Character.transform.up * verticalForce);
                    break;
                case JumpType.Slip:
                    Motor.AddImpulse(Character.transform.forward * planarForce);
                    break;
                default:
                    break;

            }
            JumpCount++;
        }
        public void Tick()
        {

        }
    }
}
