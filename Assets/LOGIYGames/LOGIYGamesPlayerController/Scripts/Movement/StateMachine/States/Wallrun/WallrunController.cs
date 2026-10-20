using LOGIYGames.Shared.Character.Events;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class WallrunController : MonoBehaviour
    {
        Vector3 normal;
        MovementWrapperBase _controller;
        Actor _character;
        [SerializeField] float baseSpeed;
        public bool AffectedByGravity;
        public float gravityMultoplier;
        private void Awake()
        {
            _character = GetComponent<Actor>();
            _controller = GetComponent<MovementWrapperBase>();
        }
        public void Enter()
        {
            if (!AffectedByGravity)
            {
                _controller.UseGravity = false;

            }
            _character.EventBus.Publish(new WallrunEnterEvent
            {
                IsRightSide = _character.Sensors.IsObstacleLegsRight ? true : false
            });
            GetComponent<Animator>().SetBool("IsWallRunning", true);
            _controller.GravityMultiplier = gravityMultoplier;
            _controller.ChangeVerticalVelocity(0);
        }
        public void Exit()
        {
            _controller.UseGravity = true;
            _controller.GravityMultiplier = 1;
            GetComponent<Animator>().SetBool("IsWallRunning", false);
        }
        public void LogicUpdate()
        {
            normal = _character.Sensors.IsObstacleLegsRight ? _character.Sensors.LegsRightHit.normal : _character.Sensors.LegsLeftHit.normal;

            if (_character.Sensors.IsObstacleLegsRight)
            {
                _controller.ChangeHorizontalVelocity((Vector3.Cross(-normal, _character.transform.up).normalized * _character.RuntimeMovement.CurrentSpeed + -normal) * baseSpeed);
            }
            else
            {
                _controller.ChangeHorizontalVelocity((Vector3.Cross(normal, _character.transform.up).normalized * _character.RuntimeMovement.CurrentSpeed + -normal) * baseSpeed );
            }

        }
        public void Rotate()
        {
            if (_character.Sensors.IsObstacleLegsRight)
            {

                _character.RotateToDirection(Vector3.Cross(-normal, _character.transform.up), _character.RuntimeMovement.TurnSmoothTime);
            }
            else
            {
                _character.RotateToDirection(Vector3.Cross(normal, _character.transform.up), _character.RuntimeMovement.TurnSmoothTime);
            }
        }
        public bool CanEnter()
        {
            return (_character.Sensors.IsObstacleLegsLeft
                || _character.Sensors.IsObstacleLegsRight)
                && !_character.Sensors.IsGrounded
                && _character.Input.MovementInput.y > 0
                && !_character.Sensors.IsObstacleLegsFront
                && _character.transform.InverseTransformDirection(_controller.Velocity).z > 2
                && _controller.Velocity.y > -4;
        }
    }
}
