using UnityEngine;

public abstract class AerialActionContext : ActionContextBase
{
    protected override void ChangeVelocity(Vector3 moveDirection)
    {
        Vector3 desiredVelocity = moveDirection * player.CurrentSpeed;
        player.HorizontalVelocity = Vector3.Lerp(
            player.HorizontalVelocity,
            desiredVelocity,
            Time.deltaTime * Acceleration);
    }
}
