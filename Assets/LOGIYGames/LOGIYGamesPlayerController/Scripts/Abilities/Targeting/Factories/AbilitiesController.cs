using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Extensions;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AbilityTargetingController))]
public class AbilitiesController : MonoBehaviour
{
    [SerializeField] private List<AbilitySO> abilities;
    public List<Ability> Abilities { get; private set; } = new List<Ability>();

    private AbilityTargetingController targetingManager;

    private Actor Character;

    private void Awake()
    {
        Character = GetComponent<Actor>();
        targetingManager = gameObject.GetOrAddComponent<AbilityTargetingController>();
        foreach (var ability in abilities)
        {
            Abilities.Add(new Ability(ability));
        }
    }

    void Update()
    {
        if (Character.Input.AbilityPressed)
        {
            Cast(Abilities[0]);
        }
    }

    public void Cast(Ability ability)
    {
        if (!ability.CooldownTimer.IsRunning)
        {

            ability.Target(targetingManager);
        }
    }
}