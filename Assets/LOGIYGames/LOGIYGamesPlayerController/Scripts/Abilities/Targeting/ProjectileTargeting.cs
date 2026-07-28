using LOGIYGames;
using System;
using UnityEngine;
[Serializable]
public class ProjectileTargeting : AbilityTargetingStrategy {
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    public override void Start(Ability ability, AbilityTargetingController targetingManager) {
        this.ability = ability;
        this.targetingManager = targetingManager;

        if (projectilePrefab != null) {
            Vector3 forw = new Vector3( targetingManager.Character.Input.LookForward.x, 0, targetingManager.Character.Input.LookForward.z);
            var forwardRotation = Quaternion.LookRotation(forw.normalized);
            var projectile = UnityEngine.Object.Instantiate(projectilePrefab, targetingManager.transform.position + Vector3.up * 1, forwardRotation);
            var context = new AbilityContext();
            context.Ability = ability;
            context.Source = targetingManager.Character.gameObject;
            projectile.GetComponent<ProjectileController>().Initialize(context, projectileSpeed);
        }
    }
}