using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
public class AbilitySO : ScriptableObject
{
    public Sprite Icon;

    public float Cooldown;

    [Header("Animations")]
    public AnimationData CastingAnimationData;

    [Header("Effects")]
    [SerializeReference] public List<EffectFactory> effects = new();

    [Header("Targeting")]
    [SerializeReference] public AbilityTargetingStrategyFactory targetingStrategy;
}
