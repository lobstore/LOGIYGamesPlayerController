using UnityEngine;

public abstract class GroundedActionContext : ActionContextBase
{
    [Range(0,1)]
    [SerializeField] protected float slopeAffectRate;
    Vector3 projectedVelocity;

    protected override void ChangeVelocity()
    {
        base.ChangeVelocity();

        CalculateSlopeSpeedMultiplier();
    }
    protected override void DebugInfo()
    {
        base.DebugInfo();
        Debug.Log(projectedVelocity); 
    }
    private void CalculateSlopeSpeedMultiplier()
    {
        projectedVelocity = Vector3.ProjectOnPlane(
        Vector3.down,
        sensors.BelowHit.normal
        );
        // Вычисляем косинус угла между направлением движения и направлением склона
        float dot = Vector3.Dot(player.HorizontalVelocity, projectedVelocity);

        // Теперь множитель скорости зависит от направления движения:
        // - dot > 0: движение вниз по склону — ускорение
        // - dot < 0: движение в гору — замедление
        // - dot ≈ 0: движение перпендикулярно склону — без изменений


        // Итоговый множитель скорости:
        var targetMultiplier = Mathf.Clamp(1f + dot * slopeAffectRate, 0.5f, 1.5f);
        player.ExternalSpeedMultiplier = Mathf.Lerp(
        player.ExternalSpeedMultiplier,
        targetMultiplier,
        Time.deltaTime * player.Acceleration);
    }
}
