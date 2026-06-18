using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames
{
    public class ObjectStrategy : TargetingStrategy
    {
        protected AbilityVFXData vFXData;
        protected GameObject prefab;
        public ObjectStrategy(AbilityVFXData vFXData)
        {
            this.vFXData = vFXData;
        }

        public override void Start(Ability ability, AbilityController abilityController)
        {
            this.Ability = ability;
            this.AbilityController = abilityController;
            CreateObject();
            InitializeObject();
            Ability.SetCooldown();
        }
        protected virtual void CreateObject()
        {
            Transform source = AbilityController.transform;
            Vector3 spawnPosition = source.TransformPoint(vFXData.vfxPositionOffset);
            Quaternion spawnRotation =
                source.rotation * vFXData.vfxRotationOffset;

            prefab = GameObject.Instantiate(
                vFXData.vfxPrefab,
                spawnPosition,
                spawnRotation);
            prefab.GetComponent<Transform>().localScale = vFXData.vfxScale;
        }
        protected virtual void InitializeObject()
        {
            prefab.GetComponent<ObjectAbilityController>().Initialize(new AbilityContext { Source = AbilityController.gameObject, Ability = Ability }, vFXData.vfxLifetime);
        }
    }
}
