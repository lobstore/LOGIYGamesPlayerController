using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class ProjectileTargeting : ObjectStrategy
    {
        private float projectileSpeed;
        public ProjectileTargeting(AbilityVFXData vFXData, float projectileSpeed) : base(vFXData)
        {
            this.projectileSpeed = projectileSpeed;
            this.vFXData = vFXData;
        }

        protected override void InitializeObject()
        {
            prefab.GetComponent<ProjectileAbilityController>().Initialize(new AbilityContext { Source = AbilityController.gameObject }, vFXData.vfxLifetime, projectileSpeed);
        }
    }
}
