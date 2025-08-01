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
            if (player.InternalSpeedMultiplier > 0.5)
            {
                animator.CrossFade(runTurn180Hash, 0.1f);
                player.InternalSpeedMultiplier = 0.5f;
            }
            else
            {
                animator.CrossFade(walkTurn180Hash, 0.1f);
            }
        }
        protected override void ChangeVelocity()
        {
            player.HorizontalVelocity = Vector3.zero;
            if (MovementInput.magnitude > 0)
            {
                player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, player.Acceleration * Time.deltaTime);
            }
            else
            {
                player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, 0, player.Deceleration * Time.deltaTime);

            }
        }
        protected override void Rotate()
        {
            return;
        }
    }
}
