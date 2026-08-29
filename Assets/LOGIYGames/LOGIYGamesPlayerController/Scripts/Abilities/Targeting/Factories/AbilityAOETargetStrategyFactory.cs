using System;
using UnityEngine;

[Serializable]
public class AbilityAOETargetStrategyFactory : AbilityTargetingStrategyFactory
{
    public GameObject aoePrefab;
    public float aoeRadius;
    public LayerMask groundLayerMask;
    public LayerMask targetLayerMask;
    public override AbilityTargetingStrategy Create()
    {
        return new AOETargeting() { aoePrefab = aoePrefab, aoeRadius = aoeRadius, groundLayerMask = groundLayerMask, targetLayerMask = targetLayerMask };
    }
}