using UnityEngine;

namespace LOGIYGames
{
    public class TurnActionContext : GroundedActionContext
    {
        private int speedHash = Animator.StringToHash("Speed");
        private int walkTurn180Hash = Animator.StringToHash("Walk Turn 180");
        private int runTurn180Hash = Animator.StringToHash("Run Turn 180");

        public override void EnterState()
        {
            base.EnterState();

            Character.InternalSpeedMultiplier = 0.5f;
        }
        protected override void ChangeVelocity()
        {
            Character.HorizontalVelocity = Vector3.zero;
            if (MovementInput.magnitude > 0)
            {
                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
            }
            else
            {
                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);

            }
        }
        protected override void Rotate()
        {
            return;
        }
    }
}
