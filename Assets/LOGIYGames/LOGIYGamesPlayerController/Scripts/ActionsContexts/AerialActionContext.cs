using UnityEngine;
namespace LOGIYGames
{
    public abstract class AerialActionContext : ActionContextBase
    {

        protected override void ChangeVelocity()
        {

            Vector3 desiredVelocity = moveDirection * Character.CurrentSpeed;
            if (MovementInput.magnitude > 0)
            {
                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(
                Character.HorizontalVelocity,
                desiredVelocity,
                Time.deltaTime * Acceleration);
            }
            else
            {
                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);

                Character.HorizontalVelocity = Vector3.Lerp(
                Character.HorizontalVelocity,
                Vector3.zero,
                Time.deltaTime * Character.Deceleration);
            }

        }
    }
}