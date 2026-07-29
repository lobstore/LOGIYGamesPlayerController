using LOGIYGames;
using UnityEngine;

[CreateAssetMenu(fileName = "New DamageOverTimeEffectData", menuName = "Abilities/Effects/DamageOverTimeEffectData")]
public class DamageOverTimeEffectData : EffectData
{
    public DamageData BaseDamage;
    public float Duration;
    public float Interval;
}