using UnityEngine;

public abstract class AerialActionContext : ActionContextBase
{

    protected override void ChangeVelocity()
    {
        
        Vector3 desiredVelocity = moveDirection * player.CurrentSpeed;
        if (MovementInput.magnitude > 0)
        {
            player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, player.Acceleration * Time.deltaTime);
            player.HorizontalVelocity = Vector3.Lerp(
            player.HorizontalVelocity,
            desiredVelocity,
            Time.deltaTime * Acceleration);
        }
        else
        {
            player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, 0, player.Deceleration * Time.deltaTime);

            player.HorizontalVelocity = Vector3.Lerp(
            player.HorizontalVelocity,
            Vector3.zero,
            Time.deltaTime * player.Deceleration);
        }

    }
}
