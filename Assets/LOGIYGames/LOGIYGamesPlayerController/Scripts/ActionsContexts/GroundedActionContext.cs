using UnityEngine;
namespace LOGIYGames
{
    public abstract class GroundedActionContext : ActionContextBase
    {
        [SerializeField]
        private bool useAutoCalculatedPlayerSpeedMultiplier = false;
        [Tooltip("Used if UseAutoCalculatedPlayerSpeedMultiplier is On")]
        [Range(0, 1)]
        [SerializeField] protected float slopeAffectRate;
        Vector3 projectedVelocity;

        protected override void ChangeVelocity()
        {
            base.ChangeVelocity();
            if (useAutoCalculatedPlayerSpeedMultiplier)
            {
                CalculateSlopeSpeedMultiplier();
            }
        }
        private void CalculateSlopeSpeedMultiplier()
        {
            projectedVelocity = Vector3.ProjectOnPlane(
            Vector3.down,
            Sensors.BelowHit.normal
            );
            // Вычисляем косинус угла между направлением движения и направлением склона
            float dot = Vector3.Dot(Character.HorizontalVelocity, projectedVelocity);

            // Теперь множитель скорости зависит от направления движения:
            // - dot > 0: движение вниз по склону — ускорение
            // - dot < 0: движение в гору — замедление
            // - dot ≈ 0: движение перпендикулярно склону — без изменений


            // Итоговый множитель скорости:
            var targetMultiplier = Mathf.Clamp(1f + dot * slopeAffectRate, 0.5f, 1.5f);
            Character.ExternalSpeedMultiplier = Mathf.Lerp(
            Character.ExternalSpeedMultiplier,
            targetMultiplier,
            Time.deltaTime * Character.Acceleration);
        }
    }
}