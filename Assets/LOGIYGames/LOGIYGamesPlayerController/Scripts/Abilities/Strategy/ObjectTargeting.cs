using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class ObjectTargeting : TargetingStrategy
    {
        protected List<IEffect> effects;
        protected AbilityVFXData vFXData;
        public ObjectTargeting(List<IEffect> effects, AbilityVFXData vFXData)
        {
            this.effects = effects;
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

            obj.GetComponent<ObjectAbilityController>()
                .Initialize(context, vFXData.vfxLifetime);
        }

        public override void Update()
        {
      
        }
    }
}
