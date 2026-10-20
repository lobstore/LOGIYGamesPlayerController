using Alchemy.Inspector;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using RealStep;
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
        Vector3 normal;
        public void Enter()
        {
            normal = Character.Sensors.LegsFrontHit.normal;
            GetComponent<FootIK>().enabled = false;
        }
        public void Exit()
        {
            GetComponent<FootIK>().enabled = true;
        }
        public bool CanExecute(JumpStateData jumpStateData)
        {
            return JumpCount < MaxJumpCount && Character.StaminaController.TryUse(Cost);
        }
        public void Jump(JumpType jumpType, float verticalForce, float planarForce)
        {
            Direction direction;
            switch (jumpType)
            {
                case JumpType.GroundJump:
                    direction = GetInputRelativeDirection();
                    break;
                case JumpType.BracedJump:
                    direction = GetInputRelativeDirection();
                    break;
                case JumpType.WallRunJump:
                    direction = Character.Sensors.IsObstacleLegsLeft ? Direction.Right : Direction.Left;
                    break;
                case JumpType.Roll:
                    direction = GetInputRelativeDirection();
                    break;
                case JumpType.Dash:
                    direction = GetInputRelativeDirection();
                    break;
                case JumpType.Slip:
                    direction = GetInputRelativeDirection();
                    break;
                default:
                    direction = GetInputRelativeDirection();
                    break;
            }
            Character.EventBus.Publish(new JumpPerformedEvent
            {
                jumpType = jumpType,
                planarForce = planarForce,
                verticalForce = verticalForce,
                direction = direction
            });
            Motor.ResetGravity();
            switch (jumpType)
            {
                case JumpType.GroundJump:
                    if (Character.Input.MovementInput.magnitude > 0)
                    {
                        Motor.AddImpulse(Character.RuntimeMovement.TargetDirection * planarForce + Character.transform.up * verticalForce);

                    }
                    else
                    {
                        Motor.AddImpulse(Character.transform.up * verticalForce);
                    }
                    break;
                case JumpType.BracedJump:
                    if (Character.Input.MovementInput.magnitude > 0)
                    {
                        switch (direction)
                        {
                            case Direction.Left:
                                Motor.AddImpulse(-Character.transform.right * planarForce + Character.transform.up * verticalForce / 2);
                                break;
                            case Direction.Right:
                                Motor.AddImpulse(Character.transform.right * planarForce + Character.transform.up * verticalForce / 2);
                                break;
                            case Direction.Forward:
                                Motor.AddImpulse(Character.transform.up * verticalForce);
                                break;
                            case Direction.Backward:
                                Motor.AddImpulse(-Character.transform.up * verticalForce);
                                break;
                            case Direction.NoMovement:
                                Motor.AddImpulse(Character.Sensors.LegsFrontHit.normal * planarForce + verticalForce * Character.transform.up);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Motor.AddImpulse(Character.Sensors.LegsFrontHit.normal * planarForce + verticalForce * Character.transform.up);
                    }


                    break;
                case JumpType.WallRunJump:
                    Vector3 normal = Character.Sensors.IsObstacleLegsLeft ? Character.Sensors.LegsLeftHit.normal : Character.Sensors.LegsRightHit.normal;
                    Motor.AddImpulse(normal * planarForce + Character.transform.up * verticalForce);
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

        private Direction GetInputRelativeDirection()
        {
            Vector2 input = Character.Input.MovementInput;

            if (input.sqrMagnitude < 0.0001f)
                return Direction.NoMovement;

            Vector3 sightForward = Character.Input.LookForward;
            Vector3 sightRight = Character.Input.LookRight;

            sightForward.y = 0f;
            sightRight.y = 0f;

            sightForward.Normalize();
            sightRight.Normalize();

            // Направление движения в мировых координатах
            Vector3 moveDirection =
                sightForward * input.y +
                sightRight * input.x;

            moveDirection.Normalize();

            // Проекция направления движения на оси персонажа
            float forward = Vector3.Dot(transform.forward, moveDirection);
            float right = Vector3.Dot(transform.right, moveDirection);

            if (Mathf.Abs(forward) > Mathf.Abs(right))
            {
                return forward > 0f
                    ? Direction.Forward
                    : Direction.Backward;
            }

            return right > 0f
                ? Direction.Right
                : Direction.Left;
        }
        public void Rotate()
        {
            Character.RotateToDirection(normal, Character.RuntimeMovement.TurnSmoothTime);
        }
    }
}
