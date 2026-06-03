using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class ProjectileTargeting : TargetingStrategy
    {
        List<IEffect> effects;
        private float projectileSpeed;
        AbilityVFXData vFXData;
        public ProjectileTargeting(List<IEffect> effects, AbilityVFXData vFXData, float projectileSpeed)
        {
            this.effects = effects;
            this.projectileSpeed = projectileSpeed;
            this.vFXData = vFXData;
        }

        public override void Cancel()
        {

        }

        public override void Start(AbilityContext context)
        {
            context.Effects = effects;

            Transform source = context.Source.transform;

            Vector3 spawnPosition = source.TransformPoint(vFXData.vfxPositionOffset);
            Quaternion spawnRotation =
                source.rotation * vFXData.vfxRotationOffset;

            GameObject obj = GameObject.Instantiate(
                vFXData.vfxPrefab,
                spawnPosition,
                spawnRotation);
            obj.GetComponent<Transform>().localScale = vFXData.vfxScale;

            obj.GetComponent<ProjectileAbilityController>()
                .Initialize(context, projectileSpeed);
        }

        public override void Update()
        {

        }
    }
}
