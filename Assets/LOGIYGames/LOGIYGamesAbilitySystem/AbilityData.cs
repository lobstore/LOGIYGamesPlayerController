using LOGIYGames;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/AbilityData")]
public class AbilityData : ScriptableObject
{
    public string label;

    [Header("UI")]
    public Sprite abilityIcon;

    public float coolDown;
    public AudioClip abilitySfx;
    public GameObject abilityVfx;


    [SerializeReference] public List<AbilityEffect> effects;

    public void Apply(GameObject caster, GameObject target)
    {
        foreach (var effect in effects)
        {
            effect.Execute(caster, target);
        }
        AudioSource.PlayClipAtPoint(abilitySfx, caster.transform.position);
        var vfx = Instantiate(abilityVfx, caster.transform.position, Quaternion.identity);
        Destroy(vfx, 3f);
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(label)) label = name;
        if (effects == null) effects = new List<AbilityEffect>();
    }
}

[Serializable]
public abstract class AbilityEffect
{
    public abstract void Execute(GameObject caster, GameObject target);
}

[Serializable]
public class DamageEffect : AbilityEffect
{
    public int amount;

    public override void Execute(GameObject caster, GameObject target)
    {
        target.GetComponent<HealthModel>().CurrentHealth -= amount;
        Debug.Log($"{caster.name} dealt {amount} damage to {target.name}");
    }
}
[Serializable]
public class HealEffect : AbilityEffect
{
    public int amount;

    public override void Execute(GameObject caster, GameObject target)
    {
        target.GetComponent<HealthModel>().CurrentHealth += amount;
        Debug.Log($"{caster.name} dealt {amount} heal to {target.name}");
    }
}
[Serializable]
public class KnockbackEffect : AbilityEffect
{
    public float force;

    public override void Execute(GameObject caster, GameObject target)
    {
        var dir = (target.transform.position - caster.transform.position).normalized;
        var rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(dir * force, ForceMode.Impulse);
            Debug.Log($"{caster.name} knocked back {target.name} with force {force}");
        }
    }
}