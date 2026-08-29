using System;
using UnityEngine;

[Serializable]
public class AbilityProjectileTargetStrategyFactory : AbilityTargetingStrategyFactory
{
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed;
    public override AbilityTargetingStrategy Create()
    {
        return new ProjectileTargeting() { projectilePrefab = ProjectilePrefab, projectileSpeed = ProjectileSpeed };
    }
}
